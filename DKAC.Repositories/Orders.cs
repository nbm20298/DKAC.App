using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Repositories
{
    public class Orders
    {
        public int OrderID { get; set; }
        [ForeignKey("Users")]
        public int UserID { get; set; }
        [ForeignKey("Shift")]
        public int ShiftID { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ShiftDate { get; set; }
        [Column(TypeName = "Time")]
        public TimeSpan TimeEdit { get; set; }
        [ForeignKey("Departments")]
        public int DepartmentID { get; set; }
        public Shift shifts { get; set; }
        public Departments departments { get; set; }
        public Users Users { get; set; }
    }
}
