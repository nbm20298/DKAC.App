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
    public class ReportService : IReportService
    {
        private string _connection;

        public ReportService(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DbConnection");
        }
        //Hàm để dùng trong home mangager
        public IEnumerable<Orders> GetDepartments(int pageNumber, int pageSize, out List<string> departmentNames, out List<string> fullName, out List<string> shiftNames)
        {
            List<Orders> Orders = new List<Orders>();
            departmentNames = new List<string>();
            fullName = new List<string>();
            shiftNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();


                string query = @"
                                SELECT o.OrderID, o.UserID, o.ShiftID, o.Quantity, o.DepartmentID, o.OrderDate, o.ShiftDate, o.TimeEdit, u.FullName, d.DepartmentName, s.ShiftName
                                FROM Orders o
                                JOIN Users u ON o.UserID = u.UserID
                                JOIN Departments d ON d.DepartmentID = u.DepartmentID
                                JOIN Shift s ON s.ShiftID = o.ShiftID
                                WHERE CAST(o.ShiftDate AS DATE) = CAST(GETDATE() AS DATE)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Orders orders = new Orders
                    {
                        OrderID = Convert.ToInt32(reader["OrderID"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        ShiftID = Convert.ToInt32(reader["ShiftID"]),
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                        OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : DateTime.MinValue,
                        ShiftDate = reader["ShiftDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShiftDate"]) : DateTime.MinValue,
                        TimeEdit = reader["TimeEdit"] != DBNull.Value ? (TimeSpan)reader["TimeEdit"] : TimeSpan.Zero
                    };

                    Orders.Add(orders);
                    departmentNames.Add(reader["DepartmentName"].ToString());
                    fullName.Add(reader["FullName"].ToString());
                    shiftNames.Add(reader["ShiftName"].ToString());
                }
                reader.Close();
            }
            return Orders;
        }

        public int countUserOrders()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                var getDayNow = DateTime.Today;

                string query = "SELECT COUNT(USERID) FROM Orders";
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("ShiftDate", getDayNow);
                count = (int)cmd.ExecuteScalar();

            }
            return count;
        }

        //Show thông tin nếu userID đã đặt rồi
        public IEnumerable<Orders> getOrderInDayUser(int UserID)
        {
            List<Orders> orders = new List<Orders>();
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                conn.Open();
                var getDayNow = DateTime.Today;

                string query = @"SELECT ShiftID, Quantity
                                 FROM Orders
                                 WHERE UserID = @UserID AND CAST(ShiftDate AS DATE) = @getDayNow";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@getDayNow", getDayNow);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Orders order = new Orders
                            {
                                ShiftID = Convert.ToInt32(reader["ShiftID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                            };
                            orders.Add(order);
                        }
                        reader.Close();
                    }
                }
            }
            return orders;
        }

        //show thông tin các userID trong một phòng đã đặt
        public IEnumerable<Orders> getOrderUserInDepartment(int DepartmentID)
        {
            List<Orders> orders = new List<Orders>();
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                conn.Open();
                var getDayNow = DateTime.Today;

                string query = @"SELECT *
                                 FROM Orders
                                 WHERE DepartmentID = @DepartmentID AND CAST(ShiftDate AS DATE) = @getDayNow";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                    cmd.Parameters.AddWithValue("@getDayNow", getDayNow);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Orders order = new Orders
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                ShiftID = Convert.ToInt32(reader["ShiftID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                            };
                            orders.Add(order);
                        }
                        reader.Close();
                    }
                }
            }
            return orders;
        }

        //Kiểm tra userID đã đặt chưa?
        public int checkUserOrderInDay(int UserID)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                conn.Open();
                var getDayNow = DateTime.Today;
                string query = @"SELECT * FROM Orders WHERE UserID = @UserID AND CAST(ShiftDate AS DATE) = @getDayNow";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@getDayNow", getDayNow);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Orders order = new Orders
                            {
                                OrderID = Convert.ToInt32(reader["OrderID"]),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                ShiftID = Convert.ToInt32(reader["ShiftID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                OrderDate = reader["OrderDate"] != DBNull.Value ? Convert.ToDateTime(reader["OrderDate"]) : DateTime.MinValue,
                                ShiftDate = reader["ShiftDate"] != DBNull.Value ? Convert.ToDateTime(reader["ShiftDate"]) : DateTime.MinValue
                            };
                            count++;
                        }
                    }

                }
            }
            return count;
        }

        //Kiểm tra userID trong Department đã đặt phòng chưa
        public int CheckUserInDepartmentOrderInDay(int departmentID)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                conn.Open();
                var getDayNow = DateTime.Today;
                string query = @"SELECT COUNT(*) FROM Orders WHERE DepartmentID = @DepartmentID AND CAST(ShiftDate AS DATE) = @getDayNow";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                    cmd.Parameters.AddWithValue("@getDayNow", getDayNow);

                    count = (int)cmd.ExecuteScalar();
                }
            }
            return count;
        }

        //thống kê user đặt trong tháng đến ngày hiện tại
        public IEnumerable<UserOrder> GetDayUserOrders(int UserID)
        {
            List<UserOrder> orders = new List<UserOrder>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connection))
                {
                    con.Open();
                    var getYear = DateTime.Now.Year;
                    var getMonth = DateTime.Now.Month;
                    DateTime startDate = new DateTime(getYear, getMonth, 1);
                    DateTime endDate = DateTime.Now;

                    string query = @"SELECT
                                        CAST(ShiftDate AS DATE) AS ShiftDate,
                                        UserID,
                                        ISNULL(SUM(CASE WHEN ShiftID = 1 THEN Quantity END), 0) AS Shift1_Quantity,
                                        ISNULL(SUM(CASE WHEN ShiftID = 2 THEN Quantity END), 0) AS Shift2_Quantity,
                                        ISNULL(SUM(CASE WHEN ShiftID = 3 THEN Quantity END), 0) AS Shift3_Quantity
                                    FROM Orders
                                    WHERE UserID = @UserID AND ShiftDate >= @startDate AND ShiftDate <= @endDate
                                    GROUP BY CAST(ShiftDate AS DATE), UserID
                                    ORDER BY CAST(ShiftDate AS DATE), UserID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserOrder order = new UserOrder
                                {
                                    ShiftDate = (DateTime)reader["ShiftDate"],
                                    UserID = (int)reader["UserID"],
                                    Shift1Quantity = (int)reader["Shift1_Quantity"],
                                    Shift2Quantity = (int)reader["Shift2_Quantity"],
                                    Shift3Quantity = (int)reader["Shift3_Quantity"]
                                };
                                orders.Add(order);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
            return orders;
        }

        //Thống kê ca total quantity của user nằm trong các phòng 
        public IEnumerable<Departments> GetOrderUserInShiftDay(DateTime date, int shiftID)
        {
            List<Departments> groupDepartments = new List<Departments>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connection))
                {
                    con.Open();
                    string query = @"SELECT d.DepartmentName, o.UserID, u.FullName, SUM(o.Quantity) AS TotalQuantity, o.OrderDate, o.ShiftDate
                                    FROM Orders o
                                    JOIN Users u ON o.UserID = u.UserID
                                    JOIN Departments d ON u.DepartmentID = d.DepartmentID
                                    WHERE o.ShiftID = @ShiftID AND CAST(o.ShiftDate AS DATE) = @getDay
                                    GROUP BY d.DepartmentName, u.FullName, o.UserID, o.OrderDate, o.ShiftDate
                                    ORDER BY d.DepartmentName, u.FullName";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@getDay", date);
                        cmd.Parameters.AddWithValue("@ShiftID", shiftID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var departmentName = reader["DepartmentName"].ToString();

                                var department = groupDepartments.FirstOrDefault(d => d.DepartmentName == departmentName);
                                if (department == null)
                                {
                                    department = new Departments
                                    {
                                        DepartmentName = departmentName,
                                        UserOrders = new List<UserOrder>()
                                    };
                                    groupDepartments.Add(department);
                                }

                                department.UserOrders.Add(new UserOrder
                                {
                                    UserID = (int)reader["UserID"],
                                    FullName = reader["FullName"].ToString(),
                                    TotalQuantity = (int)reader["TotalQuantity"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return groupDepartments;
        }

        //thống kê user theo tháng
        public IEnumerable<Departments> GetOrderUserInShiftMonth()
        {
            List<Departments> groupDepartments = new List<Departments>();
            try
            {
                using (var con = new SqlConnection(_connection))
                {
                    var currentMonth = DateTime.Now.Month;
                    var currentYear = DateTime.Now.Year;
                    con.Open();
                    string query = @"SELECT d.DepartmentName, o.UserID, u.FullName, 
                                ISNULL(SUM(CASE WHEN o.ShiftID = 1 THEN o.Quantity ELSE 0 END), 0) AS Shift1_Quantity,
                                ISNULL(SUM(CASE WHEN o.ShiftID = 2 THEN o.Quantity ELSE 0 END), 0) AS Shift2_Quantity,
                                ISNULL(SUM(CASE WHEN o.ShiftID = 3 THEN o.Quantity ELSE 0 END), 0) AS Shift3_Quantity
                                FROM Orders o
                                JOIN Users u ON o.UserID = u.UserID
                                JOIN Departments d ON u.DepartmentID = d.DepartmentID
                                WHERE MONTH(o.ShiftDate) = @currentMonth AND YEAR(o.ShiftDate) = @currentYear
                                GROUP BY d.DepartmentName, u.FullName, o.UserID
                                ORDER BY d.DepartmentName, u.FullName";

                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@currentMonth", currentMonth);
                        cmd.Parameters.AddWithValue("@currentYear", currentYear);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var departmentName = reader["DepartmentName"].ToString();

                                var department = groupDepartments.FirstOrDefault(d => d.DepartmentName == departmentName);
                                if (department == null)
                                {
                                    department = new Departments
                                    {
                                        DepartmentName = departmentName,
                                        UserOrders = new List<UserOrder>()
                                    };
                                    groupDepartments.Add(department);
                                }

                                department.UserOrders.Add(new UserOrder
                                {
                                    UserID = (int)reader["UserID"],
                                    FullName = reader["FullName"].ToString(),
                                    Shift1Quantity = (int)reader["Shift1_Quantity"],
                                    Shift2Quantity = (int)reader["Shift2_Quantity"],
                                    Shift3Quantity = (int)reader["Shift3_Quantity"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return groupDepartments;
        }
    }
}
