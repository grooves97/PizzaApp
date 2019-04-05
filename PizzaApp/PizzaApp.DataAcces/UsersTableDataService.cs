using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaApp.Models;
using System.Data.SqlClient;

namespace PizzaApp.DataAcces
{
    public class UsersTableDataService
    {
        readonly string _connectionString;

        public UsersTableDataService()
        {
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\НурсеитовА.CORP.000\source\repos\PizzaApp\PizzaApp\PizzaApp.DataAcces\Database.mdf;Integrated Security=True";
        }

        public List<User> GetAll()
        {
            var data = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = "select * from Users";

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        string name = dataReader["Name"].ToString();
                        int phone = (int)dataReader["Phone"];

                        data.Add(new User
                        {
                            Id = id,
                            Name = name,
                            Phone = phone
                        });
                    }
                    dataReader.Close();
                }
                catch (SqlException exception)
                {//TODO
                    throw;
                }
                catch (Exception exception)
                {//TODO
                    throw;
                }

            }
            return data;
        }
    }
}
