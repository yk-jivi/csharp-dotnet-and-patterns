using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace BookStore
{
    public class DbService
    {
        private readonly string connString;
        private static bool _dbInitialized = false;

        public DbService()
        {
            connString = ConfigurationManager.ConnectionStrings["BookStoreDb"].ConnectionString;
            EnsureDatabaseCreated();
        }

        // Получить строку подключения (для отладки)
        public string GetConnectionString() => connString;

        private void EnsureDatabaseCreated()
        {
            if (_dbInitialized) return;

            // Получаем имя базы данных из строки подключения
            string dbName = GetDatabaseNameFromConnectionString();

            // Создаем строку подключения к master
            string masterConnectionString = GetMasterConnectionString();

            using (SqlConnection conn = new SqlConnection(masterConnectionString))
            {
                try
                {
                    conn.Open();

                    // Проверяем и создаем базу данных если не существует
                    string checkDbSql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') " +
                                       $"CREATE DATABASE [{dbName}];";

                    int result = new SqlCommand(checkDbSql, conn).ExecuteNonQuery();

                    // Переключаемся на нашу базу данных
                    conn.ChangeDatabase(dbName);

                    // Создаем таблицы
                    CreateTablesIfNotExist(conn);

                    // Создаем хранимую процедуру
                    CreateStoredProcedure(conn);

                    _dbInitialized = true;

                    System.Diagnostics.Debug.WriteLine($"База данных '{dbName}' успешно инициализирована");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка инициализации БД: {ex.Message}");
                    throw;
                }
            }
        }

        private void CreateTablesIfNotExist(SqlConnection conn)
        {
            string sqlTables = @"
                -- Таблица Categories
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
                BEGIN
                    CREATE TABLE Categories (
                        Id INT PRIMARY KEY IDENTITY, 
                        Name NVARCHAR(100) NOT NULL UNIQUE
                    );
                    
                    -- Добавляем начальные категории
                    INSERT INTO Categories (Name) VALUES 
                        ('Романы'), ('Поэзия'), ('Драма'), ('Фэнтези'), ('Классика');
                END

                -- Таблица Books
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
                BEGIN
                    CREATE TABLE Books (
                        Id INT PRIMARY KEY IDENTITY, 
                        Title NVARCHAR(200) NOT NULL, 
                        Author NVARCHAR(100), 
                        Price DECIMAL(18,2) NOT NULL, 
                        CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
                        Year INT, 
                        Pages INT, 
                        ImageData VARBINARY(MAX)
                    );
                END

                -- Таблица Orders
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
                BEGIN
                    CREATE TABLE Orders (
                        Id INT PRIMARY KEY IDENTITY, 
                        OrderDate DATETIME DEFAULT GETDATE(), 
                        TotalAmount DECIMAL(18,2) NOT NULL
                    );
                END";

            using (SqlCommand cmd = new SqlCommand(sqlTables, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateStoredProcedure(SqlConnection conn)
        {
            string sqlProc = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_DeleteBook')
                BEGIN
                    EXEC('CREATE PROCEDURE sp_DeleteBook 
                        @Id INT 
                        AS 
                        BEGIN 
                            DELETE FROM Books WHERE Id = @Id; 
                        END');
                END";

            using (SqlCommand cmd = new SqlCommand(sqlProc, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private string GetDatabaseNameFromConnectionString()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connString);
                return builder.InitialCatalog;
            }
            catch
            {
                return "BookStoreDB";
            }
        }

        private string GetMasterConnectionString()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connString);
                builder.InitialCatalog = "master";
                return builder.ConnectionString;
            }
            catch
            {
                return "Data Source=.;Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";
            }
        }

        // --- ЧТЕНИЕ (Асинхронно) ---
        public async Task<ObservableCollection<Book>> GetAllBooksAsync()
        {
            var booksList = new ObservableCollection<Book>();
            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                string sql = @"SELECT b.*, c.Name as CategoryName 
                               FROM Books b 
                               LEFT JOIN Categories c ON b.CategoryId = c.Id";

                using (var reader = await new SqlCommand(sql, conn).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        booksList.Add(new Book
                        {
                            Id = (int)reader["Id"],
                            Title = reader["Title"].ToString(),
                            Author = reader["Author"]?.ToString() ?? "",
                            Price = (decimal)reader["Price"],
                            Year = reader["Year"] != DBNull.Value ? (int)reader["Year"] : 0,
                            Pages = reader["Pages"] != DBNull.Value ? (int)reader["Pages"] : 0,
                            Category = reader["CategoryName"]?.ToString() ?? "N/A",
                            ImageBytes = reader["ImageData"] as byte[]
                        });
                    }
                }
            }
            return booksList;
        }

        // --- Получение категорий ---
        public async Task<DataTable> GetCategoriesTableAsync()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM Categories";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                await Task.Run(() => adapter.Fill(dt));
                return dt;
            }
        }

        // --- Получение заказов ---
        public async Task<DataTable> GetOrdersTableAsync()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM Orders ORDER BY OrderDate DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                await Task.Run(() => adapter.Fill(dt));
                return dt;
            }
        }

        // --- ДОБАВЛЕНИЕ ---
        public void AddBook(Book book, int catId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"INSERT INTO Books (Title, Author, Price, CategoryId, Year, Pages, ImageData) 
                                       VALUES (@T, @A, @P, @CId, @Y, @Pg, @Img)";
                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        cmd.Parameters.AddWithValue("@T", book.Title);
                        cmd.Parameters.AddWithValue("@A", book.Author ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@P", book.Price);
                        cmd.Parameters.AddWithValue("@CId", catId);
                        cmd.Parameters.AddWithValue("@Y", book.Year > 0 ? (object)book.Year : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Pg", book.Pages > 0 ? (object)book.Pages : DBNull.Value);

                        SqlParameter imgParam = new SqlParameter("@Img", SqlDbType.VarBinary, -1);
                        imgParam.Value = book.ImageBytes ?? (object)DBNull.Value;
                        cmd.Parameters.Add(imgParam);

                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // --- ОБНОВЛЕНИЕ ---
        public void UpdateBook(Book book, int catId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"UPDATE Books SET Title=@T, Author=@A, Price=@P, CategoryId=@CId, 
                                       Year=@Y, Pages=@Pg, ImageData=@Img WHERE Id=@Id";
                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        cmd.Parameters.AddWithValue("@T", book.Title ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@A", book.Author ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@P", book.Price);
                        cmd.Parameters.AddWithValue("@CId", catId);
                        cmd.Parameters.AddWithValue("@Y", book.Year > 0 ? (object)book.Year : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Pg", book.Pages > 0 ? (object)book.Pages : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Id", book.Id);

                        SqlParameter imgParam = new SqlParameter("@Img", SqlDbType.VarBinary, -1);
                        imgParam.Value = book.ImageBytes ?? (object)DBNull.Value;
                        cmd.Parameters.Add(imgParam);

                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // --- УДАЛЕНИЕ ---
        public void DeleteBook(int id)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_DeleteBook", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- ОФОРМЛЕНИЕ ЗАКАЗА ---
        public async Task<int> CreateOrderAsync(decimal total)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "INSERT INTO Orders (TotalAmount) OUTPUT INSERTED.Id VALUES (@Total)";
                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        cmd.Parameters.AddWithValue("@Total", total);
                        int id = (int)await cmd.ExecuteScalarAsync();
                        trans.Commit();
                        return id;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // --- Проверка существования БД (для отладки) ---
        public bool CheckDatabaseExists()
        {
            string dbName = GetDatabaseNameFromConnectionString();
            string masterConn = GetMasterConnectionString();

            using (SqlConnection conn = new SqlConnection(masterConn))
            {
                conn.Open();
                string sql = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{dbName}'";
                int count = (int)new SqlCommand(sql, conn).ExecuteScalar();
                return count > 0;
            }
        }
    }
}