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
    public class PizzasTableDataService
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _providerFactory;

        public PizzasTableDataService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["appConnection"].ConnectionString;

            _providerFactory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["appConnection"].ProviderName);
        }

        public List<Pizza> GetAll()
        {
            var data = new List<Pizza>();

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = "select * from Pizzas";

                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        int id = (int)dataReader["Id"];
                        string name = dataReader["Name"].ToString();
                        string description = dataReader["Description"].ToString();
                        int price = (int)dataReader["Price"];

                        data.Add(new Pizza
                        {
                            Id = id,
                            Name = name,
                            Description = description,
                            Price = price
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

        public void Add(Pizza pizza)
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
                    command.CommandText = $"insert into Pizzas values(@name, @description, @price)";

                    DbParameter nameParametr = command.CreateParameter();
                    nameParametr.ParameterName = "@name";
                    nameParametr.DbType = System.Data.DbType.String;
                    nameParametr.Value = pizza.Name;

                    DbParameter descriptionParametr = command.CreateParameter();
                    descriptionParametr.ParameterName = "@description";
                    descriptionParametr.DbType = System.Data.DbType.String;
                    descriptionParametr.Value = pizza.Description;

                    DbParameter priceParametr = command.CreateParameter();
                    priceParametr.ParameterName = "@price";
                    priceParametr.DbType = System.Data.DbType.String;
                    priceParametr.Value = pizza.Price;

                    command.Parameters.AddRange(new DbParameter[] { nameParametr, descriptionParametr, priceParametr });

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

        public void Delete(int pizzaId)
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
                    command.CommandText = $"delete from Pizzas where id = @pizzaId";

                    DbParameter pizzaIdParametr = command.CreateParameter();
                    pizzaIdParametr.ParameterName = "@pizzaId";
                    pizzaIdParametr.DbType = System.Data.DbType.Int32;
                    pizzaIdParametr.Value = pizzaId;

                    command.Parameters.AddRange(new DbParameter[] { pizzaIdParametr });

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
