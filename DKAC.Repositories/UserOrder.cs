using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Repositories
{
    public class UserOrder
    {
        public DateTime OrderDate { get; set; }
        public DateTime ShiftDate { get; set; }
        public int UserID { get; set; }
        public int ShiftID { get; set; }
        public int DepartmentID { get; set; }
        public int Shift1Quantity { get; set; }
        public int Shift2Quantity { get; set; }
        public int Shift3Quantity { get; set; }
        public string DepartmentName { get; set; }
        public string FullName { get; set; }
        public int TotalQuantity { get; set; }
        public List<int> UserIDs { get; set; }
        public List<int> ShiftIDs { get; set; }
        public List<int> Quantities { get; set; }
    }
}
