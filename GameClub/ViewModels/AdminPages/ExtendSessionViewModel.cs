using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameClub.ViewModels.AdminPages
{
    public class ExtendSessionViewModel : ViewModelBase
    {
        private UserModel _currentUser;
        public ExtendSessionViewModel(BookModel book)
        {
            _bookId = book.Id;
            _displayDate = DateTime.Now;
            _dateTime1 = book.Start_Time;
            _startValue = book.StartTime;
            CalculateHours(1);
            Id = book.PlaceId;
            DateTime date = (new DateTime() + (book.EndTime - book.StartTime));
            double CountOfHours = date.Hour;
            Minimum = (new DateTime() + (DateTime.Now - book.StartTime)).Hour + 1;
            Hours = Minimum;
            Maximum = Minimum + 6;
            int m = date.Minute;
            if (m != 0)
              CountOfHours += (m / (double)60);
            _fixPrice = ((double)(book.Price / CountOfHours));
            Price = _fixPrice * Minimum;
        }
        private DateTime _startValue;
        private int _bookId;

        double _fixPrice;
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public int Id { get; private set; }

        private double _hours;
        public double Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;
                CalculateHours(value);
                OnPropertyChanged("Hours");
            }
        }
        private double _price;
        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }
        private string _dateTime1;
        public string DateTime1
        {
            get
            {
                return _dateTime1;
            }
            set
            {
                _dateTime1 = value;
                CalculateHours(_hours);
                OnPropertyChanged("DateTime1");
            }
        }

        private string _endValue;
        public string EndValue
        {
            get
            {
                return _endValue;
            }
            set
            {
                _endValue = value;
                OnPropertyChanged(nameof(EndValue));
            }
        }
        private DateTime _displayDate;
        public DateTime DisplayDate
        {
            get
            {
                return _displayDate;
            }
            set
            {
                _displayDate = value;
                OnPropertyChanged(nameof(DisplayDate));
            }
        }

        private void CalculateHours(double value)
        {
            EndValue = (_startValue.AddHours(value)).ToString("t");
            Price = _fixPrice * Hours;
        }

        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      new BookRepository().extendSessionBook(_bookId, Price, EndValue);
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
