using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.AdminPages;
using GameClub.Views;
using GameClub.Views.AdminPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GameClub.Views.AdminPages;
using GameClub.Views.AdminPages.Places;
using GameClub.Views.Dialogs;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace GameClub.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        CategoryesView _categoryesView;
        UsersView _usersView;
        PlacesView _placesView;
        BooksView _booksView;
        PaymentView _paymentView;
        PaymentHistoryView _paymentHistory; 
        ShopView _shopView;
        ClubMapView _clubMapView;
        public AdminViewModel(string username)
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserModel();
            this._currentUserAccount = userRepository.getByUsername(username);
            _booksView = new BooksView();
            _paymentHistory = new PaymentHistoryView();
            _usersView = new UsersView();
            _placesView = new PlacesView();
            _categoryesView = new CategoryesView();
            _shopView = new ShopView();
            _clubMapView = new ClubMapView();
            _usersView.DataContext = new DBUsersViewModel(username);
            _placesView.DataContext = new DBPlacesViewModel (username);
            _booksView.DataContext = new DBBooksViewModel();
            _paymentView = new PaymentView();
            _paymentView.DataContext = new PaymentViewModel();
            SelectedPage = _paymentView;
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
        

        private ViewModelCommand _goToPageUsers;
        public ViewModelCommand GoToPageUsers
        {
            get
            {
                return _goToPageUsers ??
                  (_goToPageUsers = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _usersView;
                  }));
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
                      var view  = obj as Window;
                      (new LoginView()).Show();
                      view.Close();
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
                      SelectedPage = _booksView;

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
        private ViewModelCommand _goToPageCategoryes;
        public ViewModelCommand GoToPageCategoryes
        {
            get
            {
                return _goToPageCategoryes ??
                  (_goToPageCategoryes = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _categoryesView;
                  }));
            }
        }

        private ViewModelCommand _goToPagePayment;
        public ViewModelCommand GoToPagePayment
        {
            get
            {
                return _goToPagePayment ??
                  (_goToPagePayment = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _paymentView;
                  }));
            }
        }

        private ViewModelCommand _goToPageShop;
        public ViewModelCommand GoToPageShop
        {
            get
            {
                return _goToPageShop ??
                  (_goToPageShop = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _shopView;
                  }));
            }
        }
        private ViewModelCommand _goToPagePaymentHistory;
        public ViewModelCommand GoToPagePaymentHistory
        {
            get
            {
                return _goToPagePaymentHistory ??
                  (_goToPagePaymentHistory = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _paymentHistory;
                  }));
            }
        }

        private ViewModelCommand _goToPageClubMap;
        public ViewModelCommand GoToPageClubMap
        {
            get
            {
                return _goToPageClubMap ??
                  (_goToPageClubMap = new ViewModelCommand(obj =>
                  {
                      SelectedPage = _clubMapView;
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
 	                        proc.Start ();
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
