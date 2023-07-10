using GameClub.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using GameClub.Helpers;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace GameClub.Repositories
{
    public class UserRepository : RepositoryBase, ISelectable<UserModel>
    {
        public void saveInDB(List<UserModel> users)
        {
            foreach (var user in users)
            {
                if (ExecuteScalar($"select * from User where id='{user.Id}';"))
                    ExecuteNonQuery($"update user set username = '{user.Username}', password = '{user.Password}', name = '{user.Name}', lastname = '{user.LastName}', email = '{user.Email}' where id = '{user.Id}';");
                else
                    ExecuteNonQuery($"insert into user( id, username, password, name, lastname, email, Image) values ({getMaxID("user")}, '{user.Username}', '{user.Password}', '{user.Name}', '{user.LastName}', '{user.Email}', '');");
            }
        }
        public void updateUser(int Id,string username, string password, string name, string lastname, string email, BitmapImage image)
        {
            string img = image == null ? "" : HelpMethods.ImageToBase64(image);
            ExecuteNonQuery($"update user set username = '{username}', password = '{password}', name = '{name}', lastname = '{lastname}', email = '{email}', image = '{img}' where id = '{Id}';");
        }
        public void updateUserWoImg(int Id, string username, string password, string name, string lastname, string email)
        {
            ExecuteNonQuery($"update user set username = '{username}', password = '{password}', name = '{name}', lastname = '{lastname}', email = '{email}' where id = '{Id}';");
        }
        public void deleteUser(UserModel user)
        {
            ExecuteNonQuery($"delete from User where username='{user.Username}';");
        }
        public bool checkOnExistingUser(string username)
        {
            return ExecuteScalar($"select * from User where username='{username}';");
        }
        public void addUser(string username, string password, string name, string lastname, string email, BitmapImage image)
        {
            ExecuteNonQuery($"insert into user( id, username, password, name, lastname, email, Image) values ({getMaxID("user")}, '{username}', '{password}', '{name}', '{lastname}', '{email}', '');");
        }
        public bool isAdmin(string username, string password)
        {
            return ExecuteScalar($"select * from User where id=0 and username='{username}' and password='{password}';");
        }
        public bool authenticateUser(string username, string password)
        {
            return ExecuteScalar($"select * from User where username='{username}' and password='{password}';");
        }
        public UserModel getByUsername(string username)
        {
            return SelectAllByOption(username, "username", true).First();
        }
        public UserModel getById(int id)
        {
            return SelectAllByOption(id.ToString(), "id", true).First();
        }
        public List<UserModel> SelectAll()
        {
            List<UserModel> users = new List<UserModel>();
            using (var command = GetCommand($"select * from User where username != 'admin'"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Username = reader[1].ToString(),
                                Password = reader[2].ToString(),
                                Name = reader[3].ToString(),
                                LastName = reader[4].ToString(),
                                Email = reader[5].ToString(),
                                Image = HelpMethods.Base64ToImage(reader[6].ToString())
                            });
                        }
                    }
                }
            }
            return users;
        }

        public List<UserModel> SelectAllByOption(string findText, string findField, bool isAdminSelect)
        {
            List<UserModel> users = new List<UserModel>();
            string query = isAdminSelect ? $"Like '{findText}'" : $"  Like '%{findText}%' and username != 'admin'";
            using (var command = GetCommand($"select * from User where {findField} {query};"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Username = reader[1].ToString(),
                                Password = reader[2].ToString(),
                                Name = reader[3].ToString(),
                                LastName = reader[4].ToString(),
                                Email = reader[5].ToString(),
                                Image = HelpMethods.Base64ToImage(reader[6].ToString())
                            });
                        }
                    }
                }
            }
            return users;
        }
    }
}
