using System;
using System.Collections.Generic;

namespace TaskManagement.Models
{
    public partial class JAccountTask
    {
        public int AccountId { get; set; }
        public int TaskId { get; set; }
        public DateTime UserAssignedDateTime { get; set; }
        public bool? TaskCreator { get; set; }
        public bool? TaskNotification { get; set; }
        // Navigation properties
        public Account Account { get; set; }
        public Task Task { get; set; }
    }
}
