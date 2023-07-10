using GameClub.Models;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Repositories
{
    internal class MapRepository : RepositoryBase
    {
        public void saveInDB(List<MapModel> map)
        {
            foreach (var point in map)
            {
                if (ExecuteScalar($"select * from map where id='{point.Id}';"))
                    ExecuteNonQuery($"update map set PosX = '{point.PosX}', PosY = '{point.PosY}', IsPlace = '{point.IsPlace}', Place_Id = '{point.PlaceId}', Text = '{point.Text}' where id = '{point.Id}';");
                else
                    ExecuteNonQuery($"INSERT INTO map (Id, PosX, PosY, IsPlace, Place_Id, Text) VALUES ('{point.Id}', '{point.PosX}', '{point.PosY}', '{point.IsPlace}', '{point.PlaceId}', '{point.Text}');");
            }
        }
        public List<MapModel> SelectAll()
        {
            List<MapModel> points = new List<MapModel>();
            using (var command = GetCommand($"select * from map"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            points.Add(new MapModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                PosX = int.Parse(reader[1].ToString()),
                                PosY = int.Parse(reader[2].ToString()),
                                IsPlace = int.Parse(reader[3].ToString()),
                                PlaceId = int.Parse(reader[4].ToString()),
                                Text = reader[5].ToString()
                            });
                        }
                    }
                }
            }
            return points;
        }

        public List<MapModel> SelectAllForUser()
        {
            List<MapModel> points = new List<MapModel>();
            using (var command = GetCommand($"select * from map where map.Place_Id not in (select DISTINCT id from place where place.condition_id = 3);"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            points.Add(new MapModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                PosX = int.Parse(reader[1].ToString()),
                                PosY = int.Parse(reader[2].ToString()),
                                IsPlace = int.Parse(reader[3].ToString()),
                                PlaceId = int.Parse(reader[4].ToString()),
                                Text = reader[5].ToString()
                            });
                        }
                    }
                }
            }
            return points;
        }

        public void DeletePoint(MapModel point)
        {
            ExecuteNonQuery($"delete from map where id='{point.Id}';");
        }
        public void Update(MapModel point)
        {
            ExecuteNonQuery($"update map set PosX = '{point.PosX}', PosY = '{point.PosY}', IsPlace = '{point.IsPlace}', Place_Id = '{point.PlaceId}', Text = '{point.Text}' where id = '{point.Id}';");
        }
        public void AddPoint(MapModel point)
        {
            ExecuteNonQuery($"INSERT INTO map (Id, PosX, PosY, IsPlace, Place_Id, Text) VALUES ('{point.Id}', '{point.PosX}', '{point.PosY}', '{point.IsPlace}', '{point.PlaceId}', '{point.Text}');");
        }
        public void AddMapText(MapModel map)
        {
            ExecuteNonQuery($"insert into map( id, posX, posY, isPlace, Place_Id, Text) values ('{getMaxID("map")}', '{map.PosX}', '{map.PosY}', '{map.IsPlace}', '{map.PlaceId}', '{map.Text}');");
        }
    }
}
