﻿using System;
using System.Collections.Generic;

namespace TaskManagement.Models
{
    public partial class Project
    {
        public Project()
        {
            Comment = new HashSet<Comment>();
        }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime ProjectCreatedDateTime { get; set; }
        public DateTime? ProjectDeadline { get; set; }
        public string ProjectDescription { get; set; }
        public int? ProjectCreatorAccountId { get; set; }
        public bool ProjectActive { get; set; }
        public DateTime? ProjectEndDateTime { get; set; }
        // Navigation properties
        public Account ProjectCreatorAccount { get; set; }
        public ICollection<Comment> Comment { get; set; }

        public int TotalTasks;
        public int DoneTasks;
        public int ProgressBar
        {
            get
            {
                if (TotalTasks == 0)
                {
                    return 0;
                }
                else
                {
                    return (DoneTasks * 100 / TotalTasks);
                }
            }
        }
    }
}
