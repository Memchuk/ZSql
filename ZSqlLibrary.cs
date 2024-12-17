using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Data;
using System.IO;
using System;

namespace ZSqlLibrary
{
    /// <summary>
    /// Используется для чтения конфига.
    /// </summary>
    public class SqlConnData
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string DataBase { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public SqlConnData(string server, string port, string dataBase, string login, string password)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            Port = port ?? throw new ArgumentNullException(nameof(port));
            DataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
            Login = login ?? throw new ArgumentNullException(nameof(login));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public string GetConnStr()
        {
            return $"Server={Server};Port={Port};Database={DataBase};uid={Login};pwd={Password};";
        }
    }

    /// <summary>
    /// Представляет собой оболочку для управления MySQL запросов.
    /// </summary>
    public class ZSql
    {
        static string ConfigPath = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public event EventHandler Error;
        public MySqlConnection Conn;

        /// <summary>
        /// Создает экземпляр класса, создавая строку подключения через обращение в конфиг файл.
        /// </summary>
        public ZSql()
        {
            string rawData = File.ReadAllText($"{ConfigPath}\\ZSql.data"); //если файла нет, вылет, потом чета придумать
            SqlConnData data = JsonSerializer.Deserialize<SqlConnData>(rawData);
            string ConnectionString = data.GetConnStr();
            Conn = new MySqlConnection(ConnectionString);
        }
        /// <summary>
        /// Создает экземпляр класса, создавая строку подключения через передаваемые аргументы.
        /// </summary>
        public ZSql(string _Server, string _Port, string _DataBase, string _Login, string _Password)
        {
            string ConnectionString = $"Server={_Server};Port={_Port};Database={_DataBase};uid={_Login};pwd={_Password};";
            Conn = new MySqlConnection(ConnectionString);
        }
        /// <summary>
        /// Создает экземпляр класса, передает строку подключения из аргумента.
        /// </summary>
        public ZSql(string ConnectionString) => Conn = new MySqlConnection(ConnectionString);
        /// <summary>
        /// Возвращает используемую строку подключения.
        /// </summary>
        public string GetConnectionString => Conn.ConnectionString;
        /// <summary>
        /// Возвращает используемый экземпляр подключения.
        /// </summary>
        public MySqlConnection GetConn => Conn;

        /// <summary>
        /// Пингует сервер.
        ///     
        /// Возврат:
        ///     Возвращает True, если подключение установлено, иначе False.
        /// </summary>
        public bool CheckConnection()
        {
            try
            {
                Conn.Open();
                return Conn.Ping();
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                Conn.Close();
            }
        }
        /// <summary>
        /// Пингует сервер.
        ///     
        /// Возврат:
        ///     Возвращает True, если подключение установлено, иначе False.
        ///     Error - Пойманная ошибка.
        /// </summary>
        public bool CheckConnection(out Exception Error)
        {
            Error = null;
            try
            {
                Conn.Open();
                return Conn.Ping();
            }
            catch (Exception e)
            {
                Error = e;
                return false;
            }
            finally
            {
                Conn.Close();
            }
        }

        /// <summary>
        /// Делает запрос на взятие таблицы.
        /// 
        /// Аргументы:
        ///     TableName - Название таблицы;
        ///     ColumnNames - Название столбцов через запятую, можно использовать "*";
        ///     RowLimit - Лимит на количество строк, 0 = без лимита;
        ///     
        /// Возврат:
        ///     Возвращает True, если запрос выполнен, иначе False.
        ///     Table - Полученная таблица.
        /// </summary>
        public bool GetTable(string TableName, string ColumnNames, uint RowLimit, out DataTable Table)
        {
            Table = new DataTable();
            try
            {
                string Limit = RowLimit != 0 ? $" LIMIT {RowLimit}" : "";
                string Query = $"SELECT {ColumnNames} FROM {TableName}{Limit};";
                new MySqlDataAdapter(Query, Conn).Fill(Table);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                Conn.Close();
            }
        }
        /// <summary>
        /// Делает запрос на взятие таблицы.
        /// 
        /// Аргументы:
        ///     TableName - Название таблицы;
        ///     ColumnNames - Название столбцов через запятую, можно использовать "*";
        ///     RowLimit - Лимит на количество строк, 0 = без лимита;
        ///     
        /// Возврат:
        ///     Возвращает True, если запрос выполнен, иначе False.
        ///     Table - Полученная таблица.
        ///     Error - Пойманная ошибка.
        /// </summary>
        public bool GetTable(string TableName, string ColumnNames, uint RowLimit, out DataTable Table, out Exception Error)
        {
            Table = new DataTable();
            Error = null;
            try
            {
                string Limit = RowLimit != 0 ? $" LIMIT {RowLimit}" : "";
                string Query = $"SELECT {ColumnNames} FROM {TableName}{Limit};";
                new MySqlDataAdapter(Query, Conn).Fill(Table);
                return true;
            }
            catch (Exception e)
            {
                Error = e;
                return false;
            }
            finally
            {
                Conn.Close();
            }
        }
    }
}
