using DKAC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Users AuthenticateUser(string Username, string Password);
        bool CreateUser(string FullName, string UserName, string Password, int DepartmentID, int ManagerID, int RoleID);
        int getDepartmentID(string UserName);
        string getFullName(int UserID);
        IEnumerable<Users> getAllUserInDepartment(int departmentID);
        string getRoleNameUser(int UserID);
        bool UpdatePassword(int userId, string oldPassword, string newPassword);
        Users GetUserById(int userID);
    }
}
