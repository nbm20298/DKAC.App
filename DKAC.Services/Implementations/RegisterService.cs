using DKAC.Repositories;
using DKAC.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;

namespace DKAC.Services.Implementations
{
    public class RegisterService : IRegisterService
    {
        private string _connection;

        public RegisterService(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DbConnection");
        }

        //Đăng ký 1 user
        public bool RegisterMeal(int userID, List<int> shiftIDs, List<int> quantities, DateTime shiftDate, int departmentID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();
                    for (int i = 0; i < shiftIDs.Count; i++)
                    {
                        int shiftID = shiftIDs[i];
                        int quantity = quantities[i];

                        DateTime orderDate = DateTime.Now;
                        var checkTime = GetTimeEndByShiftID(shiftID);
                        TimeSpan currentTime = DateTime.Now.TimeOfDay;

                        if (checkTime > currentTime && quantity >= 1 && shiftDate.Date >= orderDate.Date)
                        {
                            string sql = "INSERT INTO Orders(UserID, ShiftID, Quantity, OrderDate, ShiftDate, DepartmentID) VALUES(@UserID, @ShiftID, @Quantity, @OrderDate, @ShiftDate, @DepartmentID)";
                            using (SqlCommand cmd = new SqlCommand(sql, connection))
                            {
                                cmd.Parameters.AddWithValue("UserID", userID);
                                cmd.Parameters.AddWithValue("ShiftID", shiftID);
                                cmd.Parameters.AddWithValue("Quantity", quantity);
                                cmd.Parameters.AddWithValue("OrderDate", orderDate);
                                cmd.Parameters.AddWithValue("ShiftDate", shiftDate);
                                cmd.Parameters.AddWithValue("DepartmentID", departmentID);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            return false;
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

        //Edit register
        public bool EditRegisterUser(int UserID, List<int> shiftIDs, List<int> quantities, int departmentID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();

                    for (int i = 0; i < shiftIDs.Count; i++)
                    {
                        int shiftID = shiftIDs[i];
                        int quantity = quantities[i];

                        var checkTime = GetTimeEndByShiftID(shiftID);
                        TimeSpan currentTime = DateTime.Now.TimeOfDay;
                        TimeSpan getTime = DateTime.Now.TimeOfDay;
                        if (checkTime > currentTime && quantity >= 0)
                        {
                            if (quantity == 0)
                            {
                                string deleteSql = @"DELETE FROM Orders WHERE UserID = @UserID AND ShiftID = @ShiftID";
                                using (SqlCommand deleteCmd = new SqlCommand(deleteSql, connection))
                                {
                                    deleteCmd.Parameters.AddWithValue("@UserID", UserID);
                                    deleteCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                    deleteCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string checkOrderSql = @"
                                                    SELECT COUNT(*) 
                                                    FROM Orders 
                                                    WHERE UserID = @UserID 
                                                    AND ShiftID = @ShiftID 
                                                    AND CONVERT(date, OrderDate) = CONVERT(date, GETDATE())";

                                using (SqlCommand checkOrderCmd = new SqlCommand(checkOrderSql, connection))
                                {
                                    checkOrderCmd.Parameters.AddWithValue("@UserID", UserID);
                                    checkOrderCmd.Parameters.AddWithValue("@ShiftID", shiftID);

                                    int count = (int)checkOrderCmd.ExecuteScalar();

                                    if (count > 0)
                                    {
                                        string updateSql = @"
                                                        UPDATE Orders
                                                        SET Quantity = @Quantity, TimeEdit = @TimeEdit
                                                        WHERE UserID = @UserID 
                                                        AND ShiftID = @ShiftID 
                                                        AND CONVERT(date, OrderDate) = CONVERT(date, GETDATE())";

                                        using (SqlCommand updateCmd = new SqlCommand(updateSql, connection))
                                        {
                                            updateCmd.Parameters.AddWithValue("@UserID", UserID);
                                            updateCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                            updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                                            updateCmd.Parameters.AddWithValue("@TimeEdit", getTime);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        DateTime orderDate = DateTime.Now;
                                        DateTime shiftDate = DateTime.Now;

                                        string insertSql = @"
                                                        INSERT INTO Orders (UserID, ShiftID, Quantity, OrderDate, ShiftDate, TimeEdit, DepartmentID)
                                                        VALUES (@UserID, @ShiftID, @Quantity, @OrderDate, @ShiftDate, @TimeEdit, @DepartmentID)";

                                        using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                                        {
                                            insertCmd.Parameters.AddWithValue("@UserID", UserID);
                                            insertCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                            insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                                            insertCmd.Parameters.AddWithValue("@OrderDate", orderDate);
                                            insertCmd.Parameters.AddWithValue("@ShiftDate", shiftDate);
                                            insertCmd.Parameters.AddWithValue("@TimeEdit", getTime);
                                            insertCmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user shifts: {ex.Message}");
                return false;
            }
        }

        private TimeSpan? GetTimeEndByShiftID(int shiftID)
        {
            TimeSpan timeEnd = new TimeSpan();

            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT TimeEnd FROM Shift WHERE ShiftID = @ShiftID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShiftID", shiftID);
                timeEnd = (TimeSpan)command.ExecuteScalar();
            }

            return timeEnd;
        }

        //Manager đăng ký cho list user department
        public bool RegisterMeals(List<int> userIDs, List<int> shiftIDs, List<int> quantities, DateTime shiftDate, int departmentID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();
                    for (int i = 0; i < userIDs.Count; i++)
                    {
                        int userID = userIDs[i];
                        int shiftID = shiftIDs[i];
                        int quantity = quantities[i];

                        DateTime orderDate = DateTime.Now;
                        //var checkTime = GetTimeEndByShiftID(shiftID);
                        //TimeSpan currentTime = DateTime.Now.TimeOfDay;

                        //if (checkTime > currentTime && quantity > 0 && shiftDate.Date >= orderDate.Date)
                        //{
                        if (quantity == 0)
                        {
                            string deleteSql = @"DELETE FROM Orders WHERE UserID = @UserID AND ShiftID = @ShiftID";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, connection))
                            {
                                deleteCmd.Parameters.AddWithValue("@UserID", userID);
                                deleteCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string checkOrderSql = @"
                                                    SELECT COUNT(*) 
                                                    FROM Orders 
                                                    WHERE UserID = @UserID 
                                                    AND ShiftID = @ShiftID 
                                                    AND CONVERT(date, OrderDate) = CONVERT(date, GETDATE())";

                            using (SqlCommand checkOrderCmd = new SqlCommand(checkOrderSql, connection))
                            {
                                checkOrderCmd.Parameters.AddWithValue("@UserID", userID);
                                checkOrderCmd.Parameters.AddWithValue("@ShiftID", shiftID);

                                int count = (int)checkOrderCmd.ExecuteScalar();

                                if (count > 0)
                                {
                                    TimeSpan getTime = DateTime.Now.TimeOfDay;
                                    string updateSql = @"
                                                        UPDATE Orders
                                                        SET Quantity = @Quantity, TimeEdit = @TimeEdit
                                                        WHERE UserID = @UserID 
                                                        AND ShiftID = @ShiftID 
                                                        AND CONVERT(date, OrderDate) = CONVERT(date, GETDATE())";

                                    using (SqlCommand updateCmd = new SqlCommand(updateSql, connection))
                                    {
                                        updateCmd.Parameters.AddWithValue("@UserID", userID);
                                        updateCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                        updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                                        updateCmd.Parameters.AddWithValue("@TimeEdit", getTime);
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    TimeSpan getTime = DateTime.Now.TimeOfDay;
                                    string insertSql = @"
                                                        INSERT INTO Orders (UserID, ShiftID, Quantity, OrderDate, ShiftDate, TimeEdit, DepartmentID)
                                                        VALUES (@UserID, @ShiftID, @Quantity, @OrderDate, @ShiftDate, @TimeEdit, @DepartmentID)";

                                    using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                                    {
                                        insertCmd.Parameters.AddWithValue("@UserID", userID);
                                        insertCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                        insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                                        insertCmd.Parameters.AddWithValue("@OrderDate", orderDate);
                                        insertCmd.Parameters.AddWithValue("@ShiftDate", shiftDate);
                                        insertCmd.Parameters.AddWithValue("@TimeEdit", getTime);
                                        insertCmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        //}
                        //else
                        //{
                        //    return false;
                        //}
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }


        public bool EditRegisterUsers(List<int> userIDs, List<int> shiftIDs, List<int> quantities, int departmentID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    connection.Open();

                    for (int i = 0; i < shiftIDs.Count; i++)
                    {
                        int UserID = userIDs[i];
                        int shiftID = shiftIDs[i];
                        int quantity = quantities[i];

                        var checkTime = GetTimeEndByShiftID(shiftID);
                        TimeSpan currentTime = DateTime.Now.TimeOfDay;
                        TimeSpan getTime = DateTime.Now.TimeOfDay;
                        if (checkTime > currentTime && quantity >= 0)
                        {
                            if (quantity == 0)
                            {
                                string deleteSql = @"DELETE FROM Orders WHERE UserID = @UserID AND ShiftID = @ShiftID";
                                using (SqlCommand deleteCmd = new SqlCommand(deleteSql, connection))
                                {
                                    deleteCmd.Parameters.AddWithValue("@UserID", UserID);
                                    deleteCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                    deleteCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string checkOrderSql = @"
                                                    SELECT COUNT(*) 
                                                    FROM Orders 
                                                    WHERE UserID = @UserID 
                                                    AND ShiftID = @ShiftID 
                                                    AND CONVERT(date, OrderDate) = CONVERT(date, GETDATE())";

                                using (SqlCommand checkOrderCmd = new SqlCommand(checkOrderSql, connection))
                                {
                                    checkOrderCmd.Parameters.AddWithValue("@UserID", UserID);
                                    checkOrderCmd.Parameters.AddWithValue("@ShiftID", shiftID);

                                    int count = (int)checkOrderCmd.ExecuteScalar();

                                    if (count > 0)
                                    {
                                        string updateSql = @"
                                                        UPDATE Orders
                                                        SET Quantity = @Quantity, TimeEdit = @TimeEdit
                                                        WHERE UserID = @UserID 
                                                        AND ShiftID = @ShiftID 
                                                        AND CONVERT(date, OrderDate) = CONVERT(date, GETDATE())";

                                        using (SqlCommand updateCmd = new SqlCommand(updateSql, connection))
                                        {
                                            updateCmd.Parameters.AddWithValue("@UserID", UserID);
                                            updateCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                            updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                                            updateCmd.Parameters.AddWithValue("@TimeEdit", getTime);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        DateTime orderDate = DateTime.Now;
                                        DateTime shiftDate = DateTime.Now;

                                        string insertSql = @"
                                                        INSERT INTO Orders (UserID, ShiftID, Quantity, OrderDate, ShiftDate, TimeEdit, DepartmentID)
                                                        VALUES (@UserID, @ShiftID, @Quantity, @OrderDate, @ShiftDate, @TimeEdit, @DepartmentID)";

                                        using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                                        {
                                            insertCmd.Parameters.AddWithValue("@UserID", UserID);
                                            insertCmd.Parameters.AddWithValue("@ShiftID", shiftID);
                                            insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                                            insertCmd.Parameters.AddWithValue("@OrderDate", orderDate);
                                            insertCmd.Parameters.AddWithValue("@ShiftDate", shiftDate);
                                            insertCmd.Parameters.AddWithValue("@TimeEdit", getTime);
                                            insertCmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user shifts: {ex.Message}");
                return false;
            }
        }
    }
}