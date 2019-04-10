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
    public class PizzasOrderTableDataService
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _providerFactory;

        public PizzasOrderTableDataService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;

            _providerFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["appConnection"].ProviderName);
        }
        public List<PizzaOrder> GetAll()
        {
            var data = new List<PizzaOrder>();

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = "select * from PizzasOrder";

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int orderId = (int)dataReader["OrderId"];
                        int pizzaId = (int)dataReader["PizzaId"];
                        int count = (int)dataReader["Count"];

                        data.Add(new PizzaOrder
                        {
                            OrderId = orderId,
                            PizzaId = pizzaId,
                            CountPizza = count
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

        public void Add(PizzaOrder pizzasOrder)
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
                    command.CommandText = $"insert into PizzasOrder values(@orderId, @pizzaId, @countPizza)";

                    DbParameter orderIdParametr = command.CreateParameter();
                    orderIdParametr.ParameterName = "@orderId";
                    orderIdParametr.DbType = System.Data.DbType.Int32;
                    orderIdParametr.Value = pizzasOrder.OrderId;

                    DbParameter pizzaIdParametr = command.CreateParameter();
                    pizzaIdParametr.ParameterName = "@pizzaId";
                    pizzaIdParametr.DbType = System.Data.DbType.Int32;
                    pizzaIdParametr.Value = pizzasOrder.PizzaId;

                    DbParameter countPizzaParametr = command.CreateParameter();
                    countPizzaParametr.ParameterName = "@countPizza";
                    countPizzaParametr.DbType = System.Data.DbType.Int32;
                    countPizzaParametr.Value = pizzasOrder.CountPizza;

                    command.Parameters.AddRange(new DbParameter[] { orderIdParametr, pizzaIdParametr, countPizzaParametr });

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

        public void Delete(int orderPizzaId)
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
                    command.CommandText = $"delete from PizzasOrder where id = @pizzasOrderId";

                    DbParameter pizzasOrderIdParametr = command.CreateParameter();
                    pizzasOrderIdParametr.ParameterName = "@pizzasOrderId";
                    pizzasOrderIdParametr.DbType = System.Data.DbType.Int32;
                    pizzasOrderIdParametr.Value = orderPizzaId;

                    command.Parameters.AddRange(new DbParameter[] { pizzasOrderIdParametr });

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
