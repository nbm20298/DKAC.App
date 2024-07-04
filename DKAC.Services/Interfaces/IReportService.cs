using DKAC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Services.Interfaces
{
    public interface IReportService
    {
        IEnumerable<Orders> GetDepartments(int pageNumber, int pageSize, out List<string> departmentNames, out List<string> fullName, out List<string> shiftNames);
        IEnumerable<Orders> getOrderInDayUser(int UserID);
        IEnumerable<Orders> getOrderUserInDepartment(int DepartmentID);
        int countUserOrders();

        IEnumerable<UserOrder> GetDayUserOrders(int UserID); 
        IEnumerable<Departments> GetOrderUserInShiftDay(DateTime date, int shiftID);
        IEnumerable<Departments> GetOrderUserInShiftMonth();
        int checkUserOrderInDay(int UserID);
        int CheckUserInDepartmentOrderInDay(int departmetnID);
    }
}
