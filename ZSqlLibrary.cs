using MySql.Data.MySqlClient;
using System.Text.Json;
using System.Data;
using System;
using System.IO;

namespace ZSqlLibrary
{
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
        static string ConnectionString;
        public MySqlConnection Conn;
        public ZSql() //пустое создание класса, берем конфиги
        {
            string rawData = File.ReadAllText($"{ConfigPath}\\ZSql.data"); //если файла нет, вылет, потом чета придумать
            SqlConnData data = JsonSerializer.Deserialize<SqlConnData>(rawData);
            ConnectionString = data.GetConnStr();
            Conn = new MySqlConnection(ConnectionString);
        }

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
    }
}
