namespace DKAC.App.Models
{
    public class UserOrderModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public List<ShiftOrder> ShiftOrders { get; set; } = new List<ShiftOrder>();
    }
}
