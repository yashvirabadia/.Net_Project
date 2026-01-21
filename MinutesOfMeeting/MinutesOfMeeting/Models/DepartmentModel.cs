using System.ComponentModel.DataAnnotations;

namespace MinutesOfMeeting.Models
{
    public class DepartmentModel
    {
        [Required]
        public int departmentID { get; set; }

        [Required]
        public string departmentName { get; set; }

    }
}
