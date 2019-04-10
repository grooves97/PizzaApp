using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaApp.Models;
using System.Data.Common;
using System.Configuration;

namespace PizzaApp.DataAcces
{
    class UsersTableDataService
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _providerFactory;

        public UsersTableDataService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;

            _providerFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["appConnection"].ProviderName);
        }

        public List<User> GetAll()
        {
            var data = new List<User>();

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = "select * from Users";

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        string name = dataReader["Name"].ToString();
                        string phone = dataReader["Phone"].ToString();
                        string password = dataReader["Password"].ToString();

                        data.Add(new User
                        {
                            Id = id,
                            Name = name,
                            Phone = phone,
                            Password = password
                        });
                    }
                    dataReader.Close();
                }
                catch(DbException exception)
                {
                    Console.WriteLine($"Error:{exception.Message}");
                    throw;
                }
                catch(Exception exception)
                {
                    Console.WriteLine($"Error:{exception.Message}"):
                    throw;
                }
            }
            return data;
        }

        public void Add(User user)
        {
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                DbTransaction transaction = null;

                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    command.Transaction = transaction;
                    command.CommandText = $"insert into Users values(@name, @phone, @password)";

                    DbParameter nameParametr = command.CreateParameter();
                    nameParametr.ParameterName = "@name";
                    nameParametr.DbType = System.Data.DbType.String;
                    nameParametr.Value = user.Name;

                    DbParameter phoneNumberParametr = command.CreateParameter();
                    phoneNumberParametr.ParameterName = "@phonenumber";
                    phoneNumberParametr.DbType = System.Data.DbType.String;
                    phoneNumberParametr.Value = user.Phone;

                    DbParameter passwordParametr = command.CreateParameter();
                    passwordParametr.ParameterName = "@password";
                    passwordParametr.DbType = System.Data.DbType.String;
                    passwordParametr.Value = user.Password;

                    command.Parameters.AddRange(new DbParameter[] { nameParametr, phoneNumberParametr, passwordParametr });

                    var affectedRows = command.ExecuteNonQuery();

                    if (affectedRows < 1) throw new Exception("Вставка не удалась.");

                    transaction.Commit();
                }
                catch (DbException exception)
                {
                    Console.WriteLine($"Ошибка: {exception.Message}.");
                    transaction?.Rollback();
                    throw;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Ошибка: {exception.Message}.");
                    throw;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public void Delete(int userId)
        {
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                DbTransaction transaction = null;

                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    transaction = connection.BeginTransaction();

                    command.Transaction = transaction;
                    command.CommandText = $"delete from Users where id = @userId";

                    DbParameter userIdParametr = command.CreateParameter();
                    userIdParametr.ParameterName = "@userId";
                    userIdParametr.DbType = System.Data.DbType.Int32;
                    userIdParametr.Value = userId;

                    command.Parameters.AddRange(new DbParameter[] { userIdParametr });

                    var affectedRows = command.ExecuteNonQuery();

                    if (affectedRows != 1) throw new Exception("Ошибка при удалении!");

                    transaction.Commit();
                }
                catch (DbException exception)
                {
                    Console.WriteLine($"Ошибка: {exception.Message}.");
                    transaction?.Rollback();
                    throw;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Ошибка: {exception.Message}.");
                    throw;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }
    }
}
