using GameClub.Models;
using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameClub.ViewModels.UserPages
{
    public class CancelBookViewModel : ViewModelBase
    {
        private UserModel _currentUser;
        public CancelBookViewModel(BookModel book)
        {
            _book = book;
            DateTime date = (new DateTime() + (book.EndTime - book.StartTime));
            double CountOfHours = date.Hour;
            if (date.Minute != 0)
                CountOfHours += 0.5;
            double fixPrice = ((double)(book.Price / CountOfHours));

            date = (new DateTime() + (DateTime.Now - book.StartTime));
            CountOfHours = date.Hour;
            int m = date.Minute;
            if (m == 0) m++;
            while (m % 30 != 0) m++;
            CountOfHours += (m / (double)60); 
             
            Price = fixPrice * CountOfHours;
            Hours = CountOfHours;
        }
        private BookModel _book;
        public double Hours { get; set; }
        public double Price { get; set; }

        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      
                      new BookRepository().endBook(_book.Id, new PlaceRepository().SelectAll().Where(x => x.Id == _book.PlaceId).Last().Description, Price, Hours,_book.UserId);
                      new BookRepository().changeEndTime(_book.Id, DateTime.Now.ToString("t"));
                      (obj as Window).Close();
                  }));
            }
        }

        private ViewModelCommand _cancelCommand;
        public ViewModelCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??
                  (_cancelCommand = new ViewModelCommand(obj =>
                  {
                      (obj as Window).Close();
                  }));
            }
        }
    }
}
