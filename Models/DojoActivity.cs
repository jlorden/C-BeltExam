using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models 
{
    public class DojoActivity 
    {
        [Key]
        public int ActivityId { get; set; }

        [FutureDate]
        [Required]
        [Display (Name = "Start Date of your activity : ")]
        public DateTime StartDate { get; set; }

        [Required (ErrorMessage = "Must Name Event")]

        [Display (Name = "Activity Name : ")]
        public string ActivityName { get; set; }

        [Required (ErrorMessage = "Must Have Duration")]
        public int Duration { get; set; }
        public User Coordinator { get; set; }
        [Display (Name = "Describe Event : ")]
        [Required(ErrorMessage="Must provide a description of event")]
        public string Description { get; set; }
        public int UserId { get; set; }
        public List<CalendarEvent> CalendarActivities { get; set; }
    }
}