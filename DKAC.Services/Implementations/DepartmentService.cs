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
    public class DepartmentService : IDepartmentService
    {
        private string _connection;

        public DepartmentService(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DbConnection");
        }
        public IEnumerable<Departments> GetDepartments()
        {
            List<Departments> departments = new List<Departments>();
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();

                string query = "SELECT * FROM Departments";
                SqlCommand cmd = new SqlCommand(query, connection);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Departments department = new Departments
                    {
                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                        DepartmentName = reader["DepartmentName"].ToString(),
                        ManagerID = (reader["ManagerID"] != DBNull.Value ? Convert.ToInt32(reader["ManagerID"]) : (int?)null),
                    };

                    departments.Add(department);
                }
                reader.Close();
            }
            return departments;
        }
        public IEnumerable<Departments> GetDepartments(int pageNumber, int pageSize, out List<string> managerName)
        {
            List<Departments> departments = new List<Departments>();
            managerName = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();

                //string query = "SELECT * FROM Departments";
                string query = @"SELECT d.DepartmentID, d.DepartmentName, u.FullName
                                 FROM Departments d
                                 LEFT JOIN Users u ON d.ManagerID = u.UserID ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Departments department = new Departments
                    {
                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                        DepartmentName = reader["DepartmentName"].ToString(),
                        //ManagerID = (reader["ManagerID"] != DBNull.Value ? Convert.ToInt32(reader["ManagerID"]) : (int?)null),
                    };

                    departments.Add(department);
                    managerName.Add(reader["FullName"].ToString());
                }
                reader.Close();
            }
            return departments;
        }

        public string getDepartmentName(int DepartmentID)
        {
            string departmentName = null;
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT DepartmentName FROM Departments WHERE DepartmentID = @DepartmentID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                departmentName = (string)command.ExecuteScalar();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        departmentName = reader["DepartmentName"].ToString();
                    }
                }
            }
            return departmentName;
        }

        //Delete department
        public void deleteDepartment(int DepartmentID)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "DELETE FROM Departments WHERE DepartmentID = @DepartmentID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("DepartmentID", DepartmentID);
                cmd.ExecuteNonQuery();
            }
        }
        //Delete managerID
        public void deleteManagerID(int DepartmentID)
        {
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "UPDATE Departments SET ManagerID = null WHERE DepartmentID = @DepartmentID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("DepartmentID", DepartmentID);
                cmd.ExecuteNonQuery();
            }
        }

        public int countDepartments()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(_connection))
            {
                connection.Open();
                string query = "SELECT COUNT(DepartmentID) FROM Departments";
                SqlCommand cmd = new SqlCommand(query, connection);
                count = (int)cmd.ExecuteScalar();
            }
            return count;
        }

        //create department
        public bool createDepartment(string DepartmentName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connection))
                {
                    if (CheckIfDepartmentExists(DepartmentName))
                    {
                        return false;
                    }
                    connection.Open();
                    string query = "INSERT INTO Departments (DepartmentName) VALUES (@DepartmentName)";
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("DepartmentName", DepartmentName);
                        int DepartmentID = Convert.ToInt32(cmd.ExecuteScalar());
                    };
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //check department name đã có chưa
        private bool CheckIfDepartmentExists(string DepartmentName)
        {
            bool exists = false;

            try
            {
                using (SqlConnection con = new SqlConnection(_connection))
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM Departments WHERE DepartmentName = @DepartmentName";
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@DepartmentName", DepartmentName);
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

        //get manager Departemnt
        public int getManagerIDDepartment(int departmentID)
        {
            int managerID = 0;
            using (SqlConnection conn = new SqlConnection(_connection))
            {
                conn.Open();
                string query = @"SELECT ManagerID FROM Departments WHERE DepartmentID = @DepartmentID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                    using (SqlDataReader reader =  cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            managerID = reader.GetInt32(0);
                        }
                    }
                }
            }
            return managerID;
        }
    }
}
