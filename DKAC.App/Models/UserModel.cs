using System.ComponentModel.DataAnnotations;

namespace DKAC.App.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int DepartmentId { get; set; }
        public int ManagerID { get; set; }
        public int RoleID { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
