using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Data;
using System.IO;
using System;

namespace ZSqlLibrary
{
    public delegate void EventHandler(object sender, ZSQLException e);
    public class ZSQLException : EventArgs
    {
        public Exception Exception { get; private set; }

        public ZSQLException(Exception error)
        {
            Exception = error;
        }
    }
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


    public class ZSql
    {
        static string ConfigPath = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        public event EventHandler Error;
        static string ConnectionString;
        public MySqlConnection Conn;

        public ZSql() //пустое создание класса, берем конфиги
        {
            string rawData = File.ReadAllText($"{ConfigPath}\\ZSql.data"); //если файла нет, вылет, потом чета придумать
            SqlConnData data = JsonSerializer.Deserialize<SqlConnData>(rawData);
            ConnectionString = data.GetConnStr();
            Conn = new MySqlConnection(ConnectionString);
        }
        public ZSql(string _Server, string _Port, string _DataBase, string _Login, string _Password)
        {
            ConnectionString = $"Server={_Server};Port={_Port};Database={_DataBase};uid={_Login};pwd={_Password};";
            Conn = new MySqlConnection(ConnectionString);
        }
        public ZSql(string _ConnectionString)
        {
            ConnectionString = _ConnectionString;
            Conn = new MySqlConnection(ConnectionString);
        }

        public string GetConnectionString => ConnectionString;

        public bool CheckConnection()
        {
            try
            {
                Conn.Open();
                return Conn.Ping();
            }
            catch (Exception e)
            {
                onError(new ZSQLException(e));
                return false;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void onError(ZSQLException e)
        {
            EventHandler handler = Error;
            handler?.Invoke(this, e);
        }
    }
}
