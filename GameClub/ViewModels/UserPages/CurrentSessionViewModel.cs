using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.AdminPages;
using GameClub.Views.AdminPages;
using GameClub.Views.Dialogs;
using GameClub.Views.UserPages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameClub.ViewModels.UserPages
{
    public class CurrentSessionViewModel : ViewModelBase
    {
        void Refresh(UserModel user)
        {
            Book = new BookRepository().getBookUserById(user.Id);
            if (Book == null)
                return;
            Place = new PlaceRepository().getPlaceById(Book.PlaceId);
            double CountOfHours = (new DateTime() + (Book.EndTime - Book.StartTime)).Hour;
            int m = (new DateTime() + (Book.EndTime - Book.StartTime)).Minute;
            if (m != 0)
                CountOfHours += (m / (double)60);
            FixPrice = ((double)(Book.Price / CountOfHours)).ToString() + $"   ({CountOfHours}) часов"; 
        }
        private UserModel _user;
        public UserModel User
        {
            get
            {
                return _user;
            }

            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }
        public PlaceModel _place;
        public PlaceModel Place
        {
            get
            {
                return _place;
            }

            set
            {
                _place = value;
                OnPropertyChanged(nameof(Place));
            }
        }
        public BookModel _book;
        public BookModel Book
        {
            get
            {
                return _book;
            }

            set
            {
                _book = value;
                OnPropertyChanged(nameof(Book));
            }
        }
        private string _fixPrice { get; set; }
        public string FixPrice
        {
            get
            {
                return _fixPrice;
            }

            set
            {
                _fixPrice = value;
                OnPropertyChanged(nameof(FixPrice));
            }
        }

        private string _time;
        public string Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        public CurrentSessionViewModel(UserModel user)
        {
            User = user;
            Refresh(user);
            System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            myDispatcherTimer.Tick += myDispatcherTimer_Tick;
            myDispatcherTimer.Start();
        }

        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Book != null)
                Time = HelpMethods.CalculateTime(Book.StartTime, Book.EndTime);
            else
            {
                Time = "Сеанс не активен";
                Place = null;
                FixPrice = null;
            }
        }

        public CurrentSessionViewModel() { }


        private ViewModelCommand _loadedCommand;
        public ViewModelCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??
                  (_loadedCommand = new ViewModelCommand(obj =>
                  {
                      Refresh(User);
                  }));
            }
        }


       

        private ViewModelCommand _extendSession;
        public ViewModelCommand ExtendSession
        {
            get
            {
                return _extendSession ??
                  (_extendSession = new ViewModelCommand(obj =>
                  {
                      if(Book == null)
                      {
                          new InfoView("У вас сейчас нет активной сессии.").Show();
                          return;
                      }
                      if (Book.StartDate.AddHours(Book.StartTime.Hour).AddMinutes(Book.StartTime.Minute) > DateTime.Now)
                      {
                          new InfoView("Сеанс ещё не начат.").Show();
                          return;
                      }
                      Refresh(User);
                      var view = new ExtendSessionView();
                      view.DataContext = new ExtendSessionViewModel(Book);
                      view.ShowDialog();
                      Refresh(User);
                  }));
            }
        }
        private ViewModelCommand _cancelSession;
        public ViewModelCommand CancelSession
        {
            get
            {
                return _cancelSession ??
                  (_cancelSession = new ViewModelCommand(obj =>
                  {
                      if (Book == null)
                      {
                          new InfoView("У вас сейчас нет активной сессии.").Show();
                          return;
                      }
                      if(Book.StartDate.AddHours(Book.StartTime.Hour).AddMinutes(Book.StartTime.Minute) > DateTime.Now)
                      {
                          new InfoView("Сеанс ещё не начат.").Show();
                          return;
                      }
                      Refresh(User);
                      var view = new CancelBookView();
                      view.DataContext = new CancelBookViewModel(Book);
                      view.ShowDialog();
                      Refresh(User);
                  }));
            }
        }
    }
}
