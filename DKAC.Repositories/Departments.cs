using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DKAC.Repositories
{
    public class Departments
    {
        public int DepartmentID  { get; set; }
        [Column(TypeName = "Nvarchar")]
        [MaxLength(50)]
        public string DepartmentName { get; set; }
        public int? ManagerID { get; set; }

        public List<Orders> Orders { get; set; }
        public List<UserOrder> UserOrders {  get; set; }
        public List<Users> Users { get; set; }
    }
}
