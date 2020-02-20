﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;
using TaskManagement.Models.ViewModels;

namespace TaskManagement.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskManagementContext _context;
        private string DeletedMessage { get; set; }
        private string errorMessage { get; set; }
        //Auth
        private bool accountLoggedIn { get; set; }
        public void AuthorizeUser()
        {
            if (HttpContext.Session.GetInt32("AccID") != null)
            {
                accountLoggedIn = true;
            }
            else
            {
                ViewBag.error = "Account is inaccessible";
                accountLoggedIn = false;
            }
        }
        TaskDetailsViewModel task_details_model = new TaskDetailsViewModel();

        public TasksController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: Projects/Create
        public IActionResult Create(int? id)
        {
            AuthorizeUser();
            ViewBag.ProjectID = id;
            ViewData["AccountsToChooseFrom"] = new SelectList(_context.Account.Where(a => a.AccountRoleId != 1), "AccountId", "AccountEmail");

            ViewData["TaskList"] = new SelectList(_context.TaskState, "TaskStateId", "TaskStateName");

            if (accountLoggedIn)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Unlogged = 1 });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskName,TaskDescription")] Models.Task task, [Bind("AccountId,TaskId")] JAccountTask junction, int ProjectID)
        {
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSqlCommand("CreateTask " +
                    "@p_task_name, " +
                    "@p_task_description, " +
                    "@p_project_id, " +
                    "@p_account_id, " +
                    "@p_assigned_employee_id",
                    new SqlParameter("@p_task_name", task.TaskName),
                    new SqlParameter("@p_task_description", task.TaskDescription),
                    new SqlParameter("@p_project_id", ProjectID),
                    new SqlParameter("@p_account_id", HttpContext.Session.GetInt32("AccID")),
                    new SqlParameter("@p_assigned_employee_id", junction.AccountId));
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = ProjectID });
            }
            ViewData["AccountsToChooseFrom"] = new SelectList(_context.Account, "AccountId", "AccountEmail");
            return View(task);
        }

        // Only render the View for Delete
        public async Task<IActionResult> Delete(int? id)
        {
            AuthorizeUser();

            if (id == null)
            {
                return NotFound();
            }

            Models.Task task = await _context.Task.Where(tsk => tsk.TaskId == id).FirstOrDefaultAsync();
            task.TaskTaskState = await _context.TaskState.Where(ts => ts.TaskStateId == task.TaskTaskStateId).FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }

            if (accountLoggedIn)
            {
                return View(task);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Unlogged = 1 });
            }
        }

        // Actually Delete Task
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int ProjectID)
        {
            var task = await _context.Task.FindAsync(id);
            var comments = _context.Comment.Where(t => t.CommentTaskId == id);
            var j_task_acc = _context.JAccountTask.Where(t => t.TaskId == id);
            HttpContext.Session.SetString("DeletedMessage", task.TaskName + " has been successfully deleted");

            _context.Comment.RemoveRange(comments);
            _context.JAccountTask.RemoveRange(j_task_acc);
            _context.Task.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Projects", new { id = ProjectID });
        }

        public async Task<IActionResult> Details(int? id)
        {
            AuthorizeUser();

            if (id == null)
            {
                return NotFound();
            }
            if (HttpContext.Session.GetString("DeletedMessage") != null)
            {
                DeletedMessage = HttpContext.Session.GetString("DeletedMessage");
                HttpContext.Session.Remove("DeletedMessage");
            }
            if (HttpContext.Session.GetString("errorMessage") != null)
            {
                errorMessage = HttpContext.Session.GetString("errorMessage");
                HttpContext.Session.Remove("errorMessage");
            }
            ViewBag.errorMessage = errorMessage;
            ViewBag.DeletedMessage = DeletedMessage;
            var task = await _context.Task.FirstOrDefaultAsync(m => m.TaskId == id);
            task.TaskTaskState = await _context.TaskState.FirstOrDefaultAsync(t => t.TaskStateId == task.TaskTaskStateId);
            ViewData["StateList"] = new SelectList(_context.TaskState, "TaskStateId", "TaskStateName", task.TaskTaskStateId);

            List<Account> assignees = await
                (from acc in _context.Account
                 join jt in _context.JAccountTask
                 on acc.AccountId equals jt.AccountId
                 where jt.TaskId == id
                 select acc).ToListAsync();

            List<Comment> comments = await _context.Comment.Where(c => c.CommentTaskId == id).ToListAsync();
            comments.ForEach(comm => comm.CommentAccount = _context.Account.Where(acc => acc.AccountId == comm.CommentAccountId).FirstOrDefault());

            task_details_model = new TaskDetailsViewModel
            {
                Task = task,
                Assignees = assignees,
                Comments = comments
            };

            if (accountLoggedIn)
            {
                return View(task_details_model);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Unlogged = 1 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeState(int TaskStateId, int TaskId)
        {
            var task = await _context.Task.FindAsync(TaskId);
            task.TaskTaskStateId = TaskStateId;
            _context.Update(task);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("DeletedMessage", "State has been changed successfully!");
            return RedirectToAction("Details", "Tasks", new { id = TaskId });
        }
    }
}