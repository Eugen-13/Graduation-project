using GameClub.Helpers;
using GameClub.Models;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameClub.Repositories
{
    public class PurchaseRepository : RepositoryBase, ISelectable<PurchaseModel>
    {
        public void updatePurchase(int id, string name, string description, double price, int count, BitmapImage image)
        {
            string img = image == null ? "" : HelpMethods.ImageToBase64(image);
            ExecuteNonQuery($"update purchase set Name = '{name}', Description = '{description}', Price = '{price}', Count = '{count}', Image = '{img}' where id = '{id}';");
        }
        public void deletePurchase(int id)
        {
            ExecuteNonQuery($"delete from purchase where id='{id}';");
        }
        public void IncreaseCount(int count,int id, int currentCount)
        {
            ExecuteNonQuery($"update purchase set Count = '{currentCount - count}' where id = '{id}';");
        }
        public void addPurchase(string name, string description, double price, int count, BitmapImage image)
        {
            string img = image == null ? "" : HelpMethods.ImageToBase64(image);
            ExecuteNonQuery($"insert into purchase( id, Name, Description, Price, Count, Image) values ({getMaxID("purchase")}, '{name}', '{description}', '{price}', '{count}', '{img}');");
        }
        public PurchaseModel getById(int id)
        {
            return SelectAllByOption(id.ToString(), "id", true).First();
        }
        public List<PurchaseModel> SelectAll()
        {
            List<PurchaseModel> purchases = new List<PurchaseModel>();
            using (var command = GetCommand($"select * from purchase;"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            purchases.Add(new PurchaseModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Name = reader[1].ToString(),
                                Description = reader[2].ToString(),
                                Price = double.Parse(reader[3].ToString()),
                                Count = int.Parse(reader[4].ToString()),
                                Image = HelpMethods.Base64ToImage(reader[5].ToString())
                            });
                        }
                    }
                }
            }
            return purchases;
        }

        public List<PurchaseModel> SelectAllByOption(string findText, string findField, bool isAdminSelect)
        {
            List<PurchaseModel> purchases = new List<PurchaseModel>();
            using (var command = GetCommand($"select * from purchase where {findField}  Like '%{findText}%';"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            purchases.Add(new PurchaseModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                Name = reader[1].ToString(),
                                Description = reader[2].ToString(),
                                Price = double.Parse(reader[3].ToString()),
                                Count = int.Parse(reader[4].ToString()),
                                Image = HelpMethods.Base64ToImage(reader[5].ToString())
                            });
                        }
                    }
                }
            }
            return purchases;
        }
    }
}
