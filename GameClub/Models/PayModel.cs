using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClub.Models
{
    public class PayModel
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public double Count { get; set; }
        public DateTime PayDate { get; set; }
        public string Pay_Date
        {
            get { return PayDate.ToString("g"); }
        }
        public string UserLogin
        {
            get { return new UserRepository().getById(UserId).Username; }
        }
        public int UserId { get; set; }
        public double Result
        {
            get 
            {
                if (Count == 0.5) return Price;
                else return Count * Price; 
            }
        }

    }
}
