using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Repositories
{
    public class Users
    {
        public int UserID { get; set; }
        [Column(TypeName = "Nvarchar")]
        [MaxLength(50)]
        public string FullName { get; set; }
        [ForeignKey("Departments")]
        public int DepartmentID { get; set; }
        [Column(TypeName = "Nvarchar")]
        [MaxLength(50)]
        [ForeignKey("Departments")]
        public int RoleID { get; set; }
        [Column(TypeName = "Nvarchar")]
        [MaxLength(50)]
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Departments Departments { get; set; }
        public List<Orders> Orders { get; set; }
        public Roles Roles { get; set; }
        
    }
}
