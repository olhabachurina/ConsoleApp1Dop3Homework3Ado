using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ConsoleApp1Dop3Homework3Ado
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            //1) Добавлять нового клиента в таблицу "Customers". 
            int customerId = AddNewCustomer(connectionString, "Igor Lunin", "lunin@ukr.net");

            //2) Создавать новый заказ для существующего клиента в таблице "Orders". 
            int orderId = CreateNewOrder(connectionString, customerId, DateTime.Now);

            //3) Добавление продуктов в заказ
           AddProductsToOrder(connectionString, orderId, "Smartphone", 2);

            //4) Проверка целостности данных с использованием транзакции
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    
                    transaction.Commit();
                    Console.WriteLine("Транзакция успешно завершена.");
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    transaction.Rollback();
                }
            }
        }

        static int AddNewCustomer(string connectionString, string name, string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertCustomerQuery = "INSERT INTO Customers (Name, Email) VALUES (@Name, @Email); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertCustomerQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Email", email);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        static int CreateNewOrder(string connectionString, int customerId, DateTime orderDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertOrderQuery = "INSERT INTO Orders (CustomerID, OrderDate) VALUES (@CustomerID, @OrderDate); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertOrderQuery, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerId);
                    command.Parameters.AddWithValue("@OrderDate", orderDate);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        static void AddProductsToOrder(string connectionString, int orderId, string productName, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertOrderItemsQuery = "INSERT INTO OrderItems (OrderID, ProductName, Quantity) VALUES (@OrderID, @ProductName, @Quantity);";

                using (SqlCommand command = new SqlCommand(insertOrderItemsQuery, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}




