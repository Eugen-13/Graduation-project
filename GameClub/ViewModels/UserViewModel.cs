using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.UserPages;
using GameClub.Views;
using GameClub.Views.Dialogs;
using GameClub.Views.UserPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GameClub.ViewModels
{ 
    public class UserViewModel : ViewModelBase
    {
        PlacesView _placesView;
        ProfileUserView _profileView;
        UserBooksView _userBooksView;
        CurrentSessionView _currentSessionView;
        ShopView _shopView;
        UserPaymentHistoryView _userPaymentHistoryView;
        ViewMapView _userMapView;
        public UserViewModel(string username)
        {
            userRepository = new UserRepository();
            CurrentUserAccount = userRepository.getByUsername(username);
            _profileView = new ProfileUserView();
            _profileView.DataContext = new ProfileUserViewModel(_currentUserAccount);
            _userMapView = new ViewMapView();
            _userMapView.DataContext = new ViewMapViewModel(_currentUserAccount);
            _placesView = new PlacesView();
            _userBooksView = new UserBooksView();
            _currentSessionView = new CurrentSessionView();
            _shopView = new ShopView();
            _userPaymentHistoryView = new UserPaymentHistoryView();
            _userPaymentHistoryView.DataContext = new UserPaymentHistoryViewModel(_currentUserAccount);
            _shopView.DataContext = new ShopViewModel(_currentUserAccount);
            _currentSessionView.DataContext = new CurrentSessionViewModel(_currentUserAccount);
            _userBooksView.DataContext = new UserBooksViewModel(CurrentUserAccount);
            _placesView.DataContext = new DBPlacesViewModel(username,CurrentUserAccount);
            _presT = "Добро пожаловать, " + CurrentUserAccount.Name;
            SelectedPage = _profileView;

            System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            myDispatcherTimer.Tick += myDispatcherTimer_Tick;
            myDispatcherTimer.Start();
        }

        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            var book = new BookRepository().getBookUserById(CurrentUserAccount.Id);
            if (book != null)
                Time = HelpMethods.CalculateTime(book.StartTime, book.EndTime);
            else
                Time = "Сеанс не активен";

        }

        private UserModel _currentUserAccount;
        private UserRepository userRepository;

        private Page _selectedPage;
        public Page SelectedPage
        {
            get
            {
                return _selectedPage;
            }

            set
            {
                _selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
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

        private string _presT;
        public string PresText
        {
            get
            {
                return _presT;
            }

            set
            {
                _presT = value;
                OnPropertyChanged(nameof(PresText));
            }
        }
        public UserModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }
        
        private ViewModelCommand _changeAccountCommand;
        public ViewModelCommand ChangeAccountCommand
        {
            get
            {
                return _changeAccountCommand ??
                  (_changeAccountCommand = new ViewModelCommand(obj =>
                  {
                      var view = obj as Window;
                      (new LoginView()).Show();
                      view.Close();
                  }));
            }
        }
        
        private ViewModelCommand _goToPageUsers;
        public ViewModelCommand GoToPageUsers
        {
            get
            {
                return _goToPageUsers ??
                  (_goToPageUsers = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _profileView;
                  }));
            }
        }
        private ViewModelCommand _goToPagePlaces;
        public ViewModelCommand GoToPagePlaces
        {
            get
            {
                return _goToPagePlaces ??
                  (_goToPagePlaces = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _placesView;
                  }));
            }
        }
        private ViewModelCommand _goToPageBooks;
        public ViewModelCommand GoToPageBooks
        {
            get
            {
                return _goToPageBooks ??
                  (_goToPageBooks = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _userBooksView;
                  }));
            }
        }

        private ViewModelCommand _goToPageCurentSession;
        public ViewModelCommand GoToPageCurentSession
        {
            get
            {
                return _goToPageCurentSession ??
                  (_goToPageCurentSession = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _currentSessionView;
                  }));
            }
        }

        private ViewModelCommand _goToPageShop;
        public ViewModelCommand GoToPageShop
        {
            get
            {
                return _goToPageShop ??
                  (_goToPageCurentSession = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _shopView;
                  }));
            }
        }
        private ViewModelCommand _goToPageUserPaymentHistory;
        public ViewModelCommand GoToPageUserPaymentHistory
        {
            get
            {
                return _goToPageUserPaymentHistory ??
                  (_goToPageUserPaymentHistory = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _userPaymentHistoryView;
                  }));
            }
        }

        private ViewModelCommand _goToPageMap;
        public ViewModelCommand GoToPageMap
        {
            get
            {
                return _goToPageMap ??
                  (_goToPageMap = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _userMapView;
                  }));
            }
        }

        private ViewModelCommand _openAbout;
        public ViewModelCommand OpenAbout
        {
            get
            {
                return _openAbout ??
                  (_openAbout = new ViewModelCommand(obj =>
                  {
                      new About().ShowDialog();
                  }));
            }
        }
        private ViewModelCommand _openHelp;
        public ViewModelCommand OpenHelp
        {
            get
            {
                return _openHelp ??
                  (_openHelp = new ViewModelCommand(obj =>
                  {
                      try
                      {
                          string commandText = @"123.chm";
                          var proc = new System.Diagnostics.Process();
                          proc.StartInfo.FileName = commandText;
                          proc.StartInfo.UseShellExecute = true;
                          proc.Start();
                      }
                      catch (Exception)
                      {
                          new InfoView("Ошбика вызова файла справки, возможно файл повреждён либо несовместим в вашей ОС.").Show();
                      }
                  }));
            }
        }
    }
}
