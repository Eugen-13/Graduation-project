using GameClub.Helpers;
using GameClub.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameClub.Repositories
{
    public class PlaceRepository : RepositoryBase,ISelectable<PlaceModel>
    {
        public List<PlaceModel> SelectAll()
        {
            List<PlaceModel> places = new List<PlaceModel>();
            using (var command = GetCommand($"select * from placeview"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            places.Add(new PlaceModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                Performance = reader[1].ToString(),
                                Description = reader[2].ToString(),
                                Image = HelpMethods.Base64ToImage(reader[3].ToString()),
                                Category = reader[4].ToString(),
                                Price = int.Parse(reader[5].ToString()),
                                Condition = reader[6].ToString(),
                            });
                        }
                    }
                }
            }
            return places;
        }
        public List<PlaceModel> SelectAllUseLessPlaces()
        {
            List<PlaceModel> places = new List<PlaceModel>();
            using (var command = GetCommand($"SELECT * FROM placeview WHERE NOT EXISTS (SELECT * FROM map WHERE placeview.id = map.Place_Id);"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            places.Add(new PlaceModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                Performance = reader[1].ToString(),
                                Description = reader[2].ToString(),
                                Image = HelpMethods.Base64ToImage(reader[3].ToString()),
                                Category = reader[4].ToString(),
                                Price = int.Parse(reader[5].ToString()),
                                Condition = reader[6].ToString(),
                            });
                        }
                    }
                }
            }
            return places;
        }

        public List<string> getCategores()
        {
            List<string> strs = new List<string>();
            using (var command = GetCommand($"SELECT Name FROM category;"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            strs.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return strs;
        }
        public List<string> getConditions()
        {
            List<string> strs = new List<string>();
            using (var command = GetCommand($"SELECT Name FROM conditions;"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            strs.Add(reader[0].ToString());
                        }
                    }
                }
            }
            return strs;
        }
        public List<PlaceModel> SelectAllByOption(string findText, string findField, bool isAdminSelect)
        {
            List<PlaceModel> places = new List<PlaceModel>();
            using (var command = GetCommand($"select * from placeview where {findField} Like '%{findText}%';"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            places.Add(new PlaceModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                Performance = reader[1].ToString(),
                                Description = reader[2].ToString(),
                                Image = HelpMethods.Base64ToImage(reader[3].ToString()),
                                Category = reader[4].ToString(),
                                Price = int.Parse(reader[5].ToString()),
                                Condition = reader[6].ToString(),
                            });
                        }
                    }
                }
            }
            return places;
        }

        public PlaceModel getPlaceById(int placeId)
        {
            var items = SelectAllByOption(placeId.ToString(), "Id", false);
            if (items.Count > 0)
                return items.First();
            return null;
        }

        public void DeletePlace(PlaceModel place)
        {
            ExecuteNonQuery($"delete from place where Id='{place.Id}';");
            ExecuteNonQuery($"delete from map where Place_Id='{place.Id}';");
        }
        public int GetCategoryIdByName(string category)
        {
            using (var command = GetCommand($"select id from category where name = '{category}'"))
            {
                List<int> ids = new List<int>();
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            return int.Parse(reader[0].ToString());
                    }
                }
            }
            return 0;
        }
        public int GetConditionByName(string condition)
        {
            using (var command = GetCommand($"select id from conditions where name = '{condition}'"))
            {
                List<int> ids = new List<int>();
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            return int.Parse(reader[0].ToString());
                    }
                }
            }
            return 0;
        }

        public void AddPlace(PlaceModel place)
        {
            string img = place.Image == null ? "" : HelpMethods.ImageToBase64(place.Image);
            ExecuteNonQuery($"insert into place( Id, Performance, Description, category_id, condition_id, Image) values ('{getMaxID("place")}', '{place.Performance}', '{place.Description}', '{GetCategoryIdByName(place.Category)}', '{GetConditionByName(place.Condition)}', '{img}');");
        }
        public void EditPlace(PlaceModel place)
        {
            string img = place.Image == null ? "" : HelpMethods.ImageToBase64(place.Image);
            ExecuteNonQuery($"update place set Performance = '{place.Performance}', Description = '{place.Description}',  category_id = '{GetCategoryIdByName(place.Category)}', condition_id = '{GetConditionByName(place.Condition)}', Image = '{img}' where id = '{place.Id}';");
        }
        public void StartNewSession(int id)
        {
            ExecuteNonQuery($"update place set condition_id = '1' where id = '{id}';");
        }

    }
}

