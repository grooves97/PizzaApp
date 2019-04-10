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
    public class OrdersTableDataService
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _providerFactory;

        public OrdersTableDataService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;

            _providerFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["appConnection"].ProviderName);
        }

        public List<Order> GetAll()
        {
            var data = new List<Order>();

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = "select * from [Order]";

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        int userId = (int)dataReader["UserId"];
                        DateTime date = (DateTime)dataReader["DateTime"];
                        int sum = (int)dataReader["TotalSum"];
                        bool state = (bool)dataReader["Status"];

                        data.Add(new Order
                        {
                            Id = id,
                            UserId = userId,
                            DateTime = date,
                            Sum = sum,
                            State = state
                        });
                    }

                    dataReader.Close();
                }
                catch (DbException exception)
                {
                    Console.WriteLine($"Ошибка: {exception.Message}.");
                    throw;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Ошибка: {exception.Message}.");
                    throw;
                }
            }

            return data;
        }

        public void ChangeStatus(int orderId)
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
                    command.CommandText = $"update [Order] set Status = @status where Id = @orderId";

                    DbParameter orderIdParametr = command.CreateParameter();
                    orderIdParametr.ParameterName = "@orderId";
                    orderIdParametr.DbType = System.Data.DbType.Int32;
                    orderIdParametr.Value = orderId;

                    DbParameter statusParametr = command.CreateParameter();
                    statusParametr.ParameterName = "@status";
                    statusParametr.DbType = System.Data.DbType.Boolean;
                    statusParametr.Value = true;

                    command.Parameters.AddRange(new DbParameter[] { orderIdParametr, statusParametr });

                    var affectedRows = command.ExecuteNonQuery();

                    if (affectedRows < 1) throw new Exception("Изменение не выполнено.");

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

        public void Add(Order order)
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
                    command.CommandText = $"insert into [Order] values(@userId, @date, @totalSum, @status)";

                    DbParameter userIdParametr = command.CreateParameter();
                    userIdParametr.ParameterName = "@userId";
                    userIdParametr.DbType = System.Data.DbType.Int32;
                    userIdParametr.Value = order.UserId;

                    DbParameter dateParametr = command.CreateParameter();
                    dateParametr.ParameterName = "@date";
                    dateParametr.DbType = System.Data.DbType.DateTime;
                    dateParametr.Value = order.DateTime;

                    DbParameter totalSumParametr = command.CreateParameter();
                    totalSumParametr.ParameterName = "@totalSum";
                    totalSumParametr.DbType = System.Data.DbType.Int32;
                    totalSumParametr.Value = order.Sum;

                    DbParameter statusParametr = command.CreateParameter();
                    statusParametr.ParameterName = "@status";
                    statusParametr.DbType = System.Data.DbType.Boolean;
                    statusParametr.Value = order.State;

                    command.Parameters.AddRange(new DbParameter[] { userIdParametr, totalSumParametr, statusParametr, dateParametr });

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

        public void Delete(int orderId)
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
                    command.CommandText = $"delete from Order where id = @orderId";

                    DbParameter orderIdParametr = command.CreateParameter();
                    orderIdParametr.ParameterName = "@orderId";
                    orderIdParametr.DbType = System.Data.DbType.Int32;
                    orderIdParametr.Value = orderId;

                    command.Parameters.AddRange(new DbParameter[] { orderIdParametr });

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
