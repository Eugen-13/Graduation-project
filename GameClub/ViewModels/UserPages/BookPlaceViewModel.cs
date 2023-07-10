using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GameClub.ViewModels.UserPages
{
    public class BookPlaceViewModel : ViewModelBase
    {
        private BookRepository _bookRepository;
        private PayRepository _payRepository;
        private UserModel _currentUser;
        public BookPlaceViewModel(int id, int price, UserModel currentUser)
        {
            _displayDate = DateTime.Now;
            DisplayDateEnd = DateTime.Now.AddDays(10);
            DisplayDateStart = DateTime.Now;
            StartValue = DateTime.Now;
            _bookRepository = new BookRepository();
            _payRepository = new PayRepository();
            _currentUser = currentUser;
            CalculateHours(1);
            Id = id;
            Hours = 1;
            Price = price;
            fixPrice = price;
        }

        double fixPrice;
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
        private DateTime _dateTime1;
        public DateTime StartValue
        {
            get
            {
                return _dateTime1;
            }
            set
            {
                if (value.AddMinutes(1) < DateTime.Now) 
                    return; 
                _dateTime1 = value;
                CalculateHours(_hours);
                OnPropertyChanged("StartValue");
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
        public DateTime DisplayDateStart { get; set; }
        public DateTime DisplayDateEnd { get; set; }
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
            EndValue = (StartValue.AddHours(value)).ToString("t");
            Price = fixPrice * Hours;
        }

        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      LocalStorage.addItem("ok", "dialogResultBookPlaceView");
                      _bookRepository.bookPlace(_currentUser.Id, Id, Price, DisplayDate.ToString("d"), StartValue.ToString("t"), EndValue, 1);
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
                      LocalStorage.addItem("cancel", "dialogResultBookPlaceView");
                      (obj as Window).Close();
                  }));
            }
        }
    }
}
