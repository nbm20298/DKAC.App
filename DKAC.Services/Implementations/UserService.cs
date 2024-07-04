using DKAC.Repositories;
using DKAC.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKAC.Services.Implementations
{
    public class UserService : IUserService
    {
        private string _connection;

        public UserService(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DbConnection");
        }
        public IEnumerable<Users> GetUsers(int pageNumber, int pageSize, out List<string> departmentNames, out List<string> roleNames)
        {
            List<Users> users = new List<Users>();
            departmentNames = new List<string>();
            roleNames = new List<string>();
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = @"
                                SELECT u.UserID, u.DepartmentID, u.RoleID, u.FullName, u.UserName, u.Password, d.DepartmentName, r.RoleName 
                                FROM Users u 
                                JOIN Departments d ON u.DepartmentID = d.DepartmentID
                                JOIN Roles r ON u.RoleID = r.RoleID
                                WHERE IsACtive = 1
                                ORDER BY u.UserID
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Users user = new Users
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                        RoleID = Convert.ToInt32(reader["RoleID"]),
                        FullName = reader["FullName"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        Password = reader["Password"].ToString(),
                    };

                    users.Add(user);
                    departmentNames.Add(reader["DepartmentName"].ToString());
                    roleNames.Add(reader["RoleName"].ToString());
                }
                reader.Close();
            }
            return users;
        }

        public void deleteUser(int userID) 
        {
            using(SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("UserId", userID);
                cmd.ExecuteNonQuery();
            }
        }
        public int countUser()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT COUNT(UserID) FROM Users";
                SqlCommand cmd = new SqlCommand(query, connection);
                count = (int)cmd.ExecuteScalar();

            }
            return count;
        }
    }
}
