using GameClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameClub.Repositories
{
    public class PayRepository : RepositoryBase, ISelectable<PayModel>
    {
        public List<PayModel> SelectAll()
        {
            List<PayModel> pay = new List<PayModel>();
            using (var command = GetCommand($"select * from pay"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            pay.Add(new PayModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Description = reader[1].ToString(),
                                Price = double.Parse(reader[2].ToString()),
                                Count = double.Parse(reader[3].ToString()),
                                PayDate = DateTime.Parse(reader[4].ToString()),
                                UserId = int.Parse(reader[5].ToString())
                            });
                        }
                    }
                }
            }
            return pay;
        }
        public List<PayModel> SelectPayWithUserName(string query)
        {
            List<PayModel> pay = new List<PayModel>();
            using (var command = GetCommand($"select pay.ID, pay.Description, pay.Price, pay.Count, pay.Date, pay.User_Id from pay join user on user.ID = pay.User_Id where user.username like '%{query}%'; "))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            pay.Add(new PayModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Description = reader[1].ToString(),
                                Price = double.Parse(reader[2].ToString()),
                                Count = double.Parse(reader[3].ToString()),
                                PayDate = DateTime.Parse(reader[4].ToString()),
                                UserId = int.Parse(reader[5].ToString())
                            });
                        }
                    }
                }
            }
            return pay;
        }
        public double getBalance()
        {
           return SelectAll().Sum(x => x.Count * x.Price);
        }
       
        public List<PayModel> SelectAllByOption(string findText, string findField, bool isAdminSelect)
        {
            List<PayModel> pay = new List<PayModel>();
            using (var command = GetCommand($"select * from pay where {findField} Like '%{findText}%';"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            pay.Add(new PayModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Description = reader[1].ToString(),
                                Price = double.Parse(reader[2].ToString()),
                                Count = double.Parse(reader[3].ToString()),
                                PayDate = DateTime.Parse(reader[4].ToString()),
                                UserId = int.Parse(reader[5].ToString())
                            });
                        }
                    }
                }
            }
            return pay;
        }

        public void AddPay(string description, double price, double count, int userId)
        {
            ExecuteNonQuery($"insert into pay( id, Description, price, count, date, User_Id) values ('{getMaxID("pay")}', '{description}', '{price}', '{count}', '{DateTime.Now.ToString("g")}', '{userId}');");
        }
        public void Clear()
        {
            ExecuteNonQuery($"delete from pay;");
        }
    }
}

