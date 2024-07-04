using DKAC.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Services.Interfaces
{
    public interface IUserService
    {
        public IEnumerable<Users> GetUsers(int pageNumber, int pageSize, out List<string> departmentNames, out List<string> roleNames);
        void deleteUser(int userID);
        int countUser();
    }
}
