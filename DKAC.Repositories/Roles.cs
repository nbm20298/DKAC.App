using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Repositories
{
    public class Roles
    {
        public int RoleID { get; set; }
        [Column(TypeName = "Nvarchar")]
        [MaxLength(50)]
        public string RoleName { get; set; }
        [Column(TypeName = "Nvarchar")]
        [MaxLength(100)]
        public string? Decription { get; set; }
        public List<Users> Users {  get; set; } 
    }
}
