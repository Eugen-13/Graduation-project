using GameClub.Models;
using GameClub.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameClub.Repositories
{
    public class BookRepository : RepositoryBase, ISelectable<BookModel>
    {

        public List<BookModel> SelectAll()
        {
            List<BookModel> books = new List<BookModel>();
            using (var command = GetCommand($"select * from book"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var date_of_book = DateTime.Parse(reader[5].ToString());
                            var startValue = DateTime.Parse(reader[6].ToString());
                            var endValue = DateTime.Parse(reader[7].ToString());
                            DateTime endTime;
                            if (startValue < endValue)
                                endTime = date_of_book.AddHours(endValue.Hour).AddMinutes(endValue.Minute);
                            else
                                endTime = date_of_book.AddDays(1).AddHours(endValue.Hour).AddMinutes(endValue.Minute);
                            books.Add(new BookModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                UserId = int.Parse(reader[1].ToString()),
                                PlaceId = int.Parse(reader[2].ToString()),
                                Price = double.Parse(reader[3].ToString()),
                                DateOfBook = DateTime.Parse(reader[4].ToString()),
                                StartDate = date_of_book,
                                StartTime = date_of_book.AddHours(startValue.Hour).AddMinutes(startValue.Minute),
                                EndTime = endTime,
                                IsReally = int.Parse(reader[8].ToString()),
                            });
                        }
                    }
                }
            }
            return books;
        }

        public bool checkActiceBooks(int userId)
        {
            return ExecuteScalar($"select * from book where IsReally=1 and User_id = '{userId}';");
        }
        public List<BookModel> SelectAllByOption(string findText, string findField, bool isAdminSelect)
        {
            string query = !isAdminSelect ? $"select * from book where {findField} Like '%{findText}%' and IsReally = 1;" : $"select * from book where {findField} Like '%{findText}%';";
            List <BookModel> books = new List<BookModel>();
            using (var command = GetCommand(query))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var date_of_book = DateTime.Parse(reader[5].ToString());
                            var startValue = DateTime.Parse(reader[6].ToString());
                            var endValue = DateTime.Parse(reader[7].ToString());
                            DateTime endTime;
                            if (startValue < endValue)
                                endTime = date_of_book.AddHours(endValue.Hour).AddMinutes(endValue.Minute);
                            else
                                endTime = date_of_book.AddDays(1).AddHours(endValue.Hour).AddMinutes(endValue.Minute);
                            books.Add(new BookModel()
                            {
                                Id = int.Parse(reader[0].ToString()),
                                UserId = int.Parse(reader[1].ToString()),
                                PlaceId = int.Parse(reader[2].ToString()),
                                Price = double.Parse(reader[3].ToString()),
                                DateOfBook = DateTime.Parse(reader[4].ToString()),
                                StartDate = date_of_book,
                                StartTime = date_of_book.AddHours(startValue.Hour).AddMinutes(startValue.Minute),
                                EndTime = endTime,
                                IsReally = int.Parse(reader[8].ToString()),
                            });
                        }
                    }
                }
            }
            return books;
        }
        public BookModel getBookUserById(int userId)
        {
            var items = SelectAllByOption(userId.ToString(), "user_Id", false);
            if (items.Count > 0)
                return items.First();
            return null;
        }

        public BookModel getBookPlacesById(int placeId)
        {
            var items = SelectAllByOption(placeId.ToString(), "place_Id", false);
            if (items.Count > 0)
                return items.First();
            return null;
        }

        public BookModel getBookPlacesById1(int placeId)
        {
            var items = SelectAllByOption(placeId.ToString(), "place_Id", true);
            if (items.Count > 0)
                return items.First();
            return null;
        }
        public void endBook(int id, string description, double price, double hours, int userId)
        {
            ExecuteNonQuery($"update book set IsReally = '0' where id = '{id}';");
            new PayRepository().AddPay(description, price, hours, userId);
        }

        public void extendSessionBook(int id, double price, string endTime)
        {
            ExecuteNonQuery($"update book set price = '{price}', EndTime = '{endTime}' where id = '{id}';");
        }
        public void changeEndTime(int id, string endTime)
        {
            ExecuteNonQuery($"update book set EndTime = '{endTime}' where id = '{id}';");
        }

        public void bookPlace(int userId, int placeId, double price, string date,string startTime, string endTime,int IsReally)
        {
            ExecuteNonQuery($"insert into book( id, User_id, Place_id, Price, Date_of_book, StartDate, StartTime, EndTime, IsReally) values ({getMaxID("book")}, '{userId}', '{placeId}', '{price}', '{DateTime.Now}', '{date}', '{startTime}', '{endTime}', '{IsReally}');");
        }
        public void Clear()
        {
            ExecuteNonQuery($"delete from book;");
        }

    }
}
