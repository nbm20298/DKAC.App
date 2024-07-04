using DKAC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Services.Interfaces
{
    public interface IRegisterService
    {
        bool RegisterMeal(int userID, List<int> shiftIDs, List<int> quantities, DateTime shiftDate, int departmentID);
        bool RegisterMeals(List<int> userIDs, List<int> shiftIDs, List<int> quantities, DateTime shiftDate, int departmentID);
        bool EditRegisterUser(int UserID, List<int> shiftIDs, List<int> quantities, int departmentID);
        bool EditRegisterUsers(List<int> userIDs, List<int> shiftIDs, List<int> quantities, int departmentID);
    }
}
