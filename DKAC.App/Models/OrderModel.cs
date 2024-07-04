using DKAC.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace DKAC.App.Models
{
    public class OrderModel
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public List<int> UserIDs { get; set; }
        public List<int> ShiftIDs { get; set; }
        public List<int> Quantities { get; set; }
        public int DepartmentID { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ShiftDate { get; set; }
        [Column(TypeName = "Time")]
        public TimeSpan TimeEdit { get; set; }
        public Departments Departments { get; set; }
        public List<Users> Users { get; set; }
        public List<ShiftOrder> ShiftOrders { get; set; } = new List<ShiftOrder>();
        public List<UserOrderModel> UserOrders { get; set; } = new List<UserOrderModel>();
    }
}
