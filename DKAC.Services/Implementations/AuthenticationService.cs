using DKAC.Repositories;
using DKAC.Services.Helper;
using DKAC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DKAC.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private string _connection;


        public AuthenticationService(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DbConnection");
        }

        public Users AuthenticateUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT UserID, UserName, Password, Salt, DepartmentID, RoleID FROM Users WHERE UserName = @UserName AND IsActive = 1";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["Password"].ToString();
                            string storedSalt = reader["Salt"].ToString();
                            bool isPasswordValid = HashHelper.VerifyHash(password, storedSalt, storedHash);

                            if (isPasswordValid)
                            {
                                return new Users
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                    UserName = reader["UserName"].ToString(),
                                    RoleID = Convert.ToInt32(reader["RoleID"]),
                                };
                            }
                        }
                    }
                }
            }
            return null;
        }

        private bool CheckIfUserExists(string userName)
        {
            bool exists = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", userName);

                        int count = (int)command.ExecuteScalar();
                        exists = (count > 0);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return exists;
        }

        private bool CheckDepartmentManager(int DepartmentID)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string checkManagerQuery = "SELECT COUNT(*) FROM Departments WHERE DepartmentID = @DepartmentID AND ManagerID IS NOT NULL";
                using (SqlCommand checkCmd = new SqlCommand(checkManagerQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void InsertDepartmentManager(int DepartmentID, int ManagerID)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string updateDepartmentQuery = "UPDATE Departments SET ManagerID = @ManagerID WHERE DepartmentID = @DepartmentID";
                using (SqlCommand updateCmd = new SqlCommand(updateDepartmentQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("@ManagerID", ManagerID);
                    updateCmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        public bool CreateUser(string fullName, string username, string password, int departmentID, int managerID, int roleID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    string salt = HashHelper.GenerateSalt();
                    string passwordHash = HashHelper.ComputeHash(password, salt);
                    connection.Open();

                    if (CheckIfUserExists(username))
                    {
                        return false;
                    }

                    string userInsertQuery = "INSERT INTO Users (FullName, UserName, Password, Salt, DepartmentID, RoleID, IsActive) VALUES (@FullName, @UserName, @Password, @Salt, @DepartmentID, @RoleID, @IsActive); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand command = new SqlCommand(userInsertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", fullName);
                        command.Parameters.AddWithValue("@UserName", username);
                        command.Parameters.AddWithValue("@Password", passwordHash);
                        command.Parameters.AddWithValue("@Salt", salt);
                        command.Parameters.AddWithValue("@DepartmentID", departmentID);
                        command.Parameters.AddWithValue("@RoleID", roleID);
                        command.Parameters.AddWithValue("@IsActive", true);

                        int userID = Convert.ToInt32(command.ExecuteScalar());

                        if (!CheckDepartmentManager(departmentID))
                        {
                            InsertDepartmentManager(departmentID, userID);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return false;
            }
        }

        public int getDepartmentID(string UserName)
        {
            int departmentID = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();
                    string query = "SELECT DepartmentID FROM Users WHERE UserName = @UserName";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            departmentID = Convert.ToInt32(result);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return departmentID;
        }

        public string getFullName(int UserID)
        {
            string fullName = "";
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT FullName FROM Users WHERE UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                fullName = (string)cmd.ExecuteScalar();
            }
            return fullName;
        }

        public IEnumerable<Users> getAllUserInDepartment(int departmentID)
        {
            List<Users> users = new List<Users>();
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = @"SELECT * FROM Users WHERE DepartmentID = @DepartmentID AND IsACtive = 1";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Users user = new Users()
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                        RoleID = Convert.ToInt32(reader["RoleID"]),
                        FullName = reader["FullName"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        Password = reader["Password"].ToString(),
                    };
                    users.Add(user);
                }
                return users;
            }
        }

        public string getRoleNameUser(int UserID)
        {
            string roleName = "";
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                conn.Open();
                string query = @"SELECT r.RoleName FROM Users u JOIN Roles r ON r.RoleID = u.RoleID WHERE u.UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            roleName = reader["RoleName"].ToString();
                        }
                    }
                }
            }
            return roleName;
        }

        public bool UpdatePassword(int userId, string oldPassword, string newPassword)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT Password, Salt FROM Users WHERE UserID = @UserID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["Password"].ToString();
                            string storedSalt = reader["Salt"].ToString();
                            bool isPasswordValid = HashHelper.VerifyHash(oldPassword, storedSalt, storedHash);

                            if (isPasswordValid)
                            {
                                // Generate new salt and hash the new password
                                string newSalt = HashHelper.GenerateSalt();
                                string newHash = HashHelper.ComputeHash(newPassword, newSalt);

                                // Update the database with the new password and salt
                                reader.Close(); // Close the reader before executing another command

                                string updateQuery = "UPDATE Users SET Password = @NewPassword, Salt = @NewSalt WHERE UserID = @UserID";
                                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@NewPassword", newHash);
                                    updateCommand.Parameters.AddWithValue("@NewSalt", newSalt);
                                    updateCommand.Parameters.AddWithValue("@UserID", userId);

                                    int rowsAffected = updateCommand.ExecuteNonQuery();
                                    return rowsAffected > 0;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public Users GetUserById(int userId)
        {
            Users user = new Users();

            using (var connection = new SqlConnection(_connection))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Users WHERE UserID = @UserID", connection);
                command.Parameters.AddWithValue("@UserID", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Users
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            FullName = reader["FullName"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            Password = reader["Password"].ToString(),
                            DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                            RoleID = Convert.ToInt32(reader["RoleID"])
                        };
                    }
                }
            }
            return user;
        }
    }
}

