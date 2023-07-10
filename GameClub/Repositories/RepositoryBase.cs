using GameClub.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.Sqlite;
using GameClub.Views.Dialogs;
using System.Windows.Automation;
using Org.BouncyCastle.Asn1.X509;

namespace GameClub.Repositories
{
    public abstract class RepositoryBase 
    {
        static private string _connectionString = "Data Source=GameClub_DB.db";
        public void ChangeConnection(string path)
        {
            _connectionString = $"Data Source={path.Trim()}";
        }
        public void ChangeConnectionDefault()
        {
            _connectionString = "Data Source=GameClub_DB.db";
        }
        protected SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
        protected SqliteCommand GetCommand(string queryString)
         {
            return new SqliteCommand(queryString,GetConnection());
        }
        protected void ExecuteNonQuery(string queryString)
        {
            if (queryString.Count(c => c == '\'') % 2 != 0)
            {
                new InfoView("Нельзя вводить одинарную кавычку").Show();
                return;
            }
            using (var connection = this.GetConnection())
            {
                using (var command = new SqliteCommand(queryString, connection))
                {
                    command.Connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                   
                }
            }
        }
        public bool ExecuteScalar(string queryString)
        {
            if (queryString.Count(c => c == '\'') % 2 != 0)
            {
                new InfoView("Нельзя вводить одинарную кавычку").Show();
                return false;
            }
            using (var connection = GetConnection())
            using (var command = new SqliteCommand(queryString, connection))
            {
                command.Connection.Open();
                return command.ExecuteScalar() == null ? false : true;
            }
        }
        public List<string> GetFields(string tableName)
        {
            List<string> fields = new List<string>(); 
            using (var command = GetCommand($"PRAGMA table_info({tableName}); "))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            fields.Add((reader[1].ToString()));
                        }
                    }
                }
            }
            return fields;
        }

        public int getMaxID(string tableName)
        {
            using (var command = GetCommand($"SELECT MAX(id) + 1 FROM {tableName};"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    string read = reader[0].ToString();
                    if(read == "")
                        return 0;
                    return int.Parse(reader[0].ToString());
                }
            }
        }
    }
}
