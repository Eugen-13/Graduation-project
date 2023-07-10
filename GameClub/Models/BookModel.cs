using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string UserLogin
        {
            get { return new UserRepository().getById(UserId).Username; }
        }
        public int UserId { get; set; }
        public int PlaceId { get; set; }
        public double Price { get; set; }

        public DateTime DateOfBook { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int IsReally { get; set; }

        public string Date_Of_Book
        {
            get
            {
                return DateOfBook.ToString("d");
            }
        }
        public string Start_Date
        {
            get
            {
                return StartDate.ToString("d");
            }
        }
        public string Start_Time
        {
            get
            {
                return StartTime.ToString("t");
            }
        }
        public string End_Time
        {
            get
            {
                return EndTime.ToString("t");
            }
        }
    }
}
