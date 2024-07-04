using DKAC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Services.Interfaces
{
    public interface IDepartmentService
    {
        IEnumerable<Departments> GetDepartments();
        IEnumerable<Departments> GetDepartments(int pageNumber, int pageSize, out List<string> managerName);
        string getDepartmentName(int DepartmentID);
        void deleteDepartment(int DepartmentID);
        void deleteManagerID(int DepartmentID);
        int countDepartments();
        bool createDepartment(string DepartmentName);
        int getManagerIDDepartment(int departmentID);
    }
}
