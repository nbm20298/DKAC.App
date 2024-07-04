using DKAC.App.Interfaces;
using DKAC.Repositories;
using Microsoft.Data.SqlClient;

namespace DKAC.App.Helpers
{
    public class UserAccessor : IUserAccessor
    {
        private string _connection;
        private HttpContextAccessor _httpContextAccessor;
        public UserAccessor(IConfiguration configuration, HttpContextAccessor httpContextAccessor)
        {
            _connection = configuration.GetConnectionString("DbConnection");
            _httpContextAccessor = httpContextAccessor;
        }

        public Users GetUser()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var userClaims = _httpContextAccessor.HttpContext.User.Claims;

                if (userClaims != null)
                {
                    var userId = userClaims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                    var userName = userClaims.FirstOrDefault(c => c.Type == "UserName")?.Value;
                    var fullName = userClaims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                    var password = userClaims.FirstOrDefault(c => c.Type == "Password")?.Value;
                    var roleId = userClaims.FirstOrDefault(c => c.Type == "RoleID")?.Value;
                    var departmentId = userClaims.FirstOrDefault(c => c.Type == "DepartmentID")?.Value;

                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(roleId) && !string.IsNullOrEmpty(departmentId))
                    {
                        var user = new Users
                        {
                            UserID = int.Parse(userId),
                            FullName = fullName,
                            UserName = userName,
                            Password = password,
                            RoleID = int.Parse(roleId),
                            DepartmentID = int.Parse(departmentId)
                        };
                        return user;
                    }
                }
            }
            return null;
        }
    }
}
