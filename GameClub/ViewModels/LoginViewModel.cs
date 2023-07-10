using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using GameClub.Views;
using System.Windows;
using GameClub.Helpers;
using GameClub.Views.Dialogs;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace GameClub.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;

        private UserRepository userRepository;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        
        public ICommand LoginCommand { get; }

        private ViewModelCommand _goToRegCommand;
        public ViewModelCommand GoToRegCommand
        {
            get
            {
                return _goToRegCommand ??
                  (_goToRegCommand = new ViewModelCommand(obj =>
                  {
                      Window loginView = obj as Window;
                      loginView.Hide();
                      RegistrationView registrationView = new RegistrationView();
                      registrationView.Show();
                  }));
            }
        }


        private ViewModelCommand _openConnectionView;
        public ViewModelCommand OpenConnectionView
        {
            get
            {
                return _openConnectionView ??
                  (_openConnectionView = new ViewModelCommand(obj =>
                  {
                     EditConnectionView editConnectionView = new EditConnectionView();
                      editConnectionView.ShowDialog();
                  }));
            }
        }

        private readonly string _connectionFilePath = "connection.txt";
        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            if (File.Exists(_connectionFilePath))
            {
                new UserRepository().ChangeConnection(File.ReadAllText(_connectionFilePath));
            }
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3)
                validData = false;
            else
                validData = true;
            return validData;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var values = (object[])obj;
            var password = (values[0] as PasswordBox).Password;
            if (password.Contains('\''))
                return;
            var window = values[1] as Window;
            var isValidUser = userRepository.authenticateUser(Username, password);
            
            if (isValidUser)
            {
                if(userRepository.isAdmin(Username, password))
                {
                    AdminView mainView = new AdminView();
                    mainView.Show();
                    mainView.DataContext = new AdminViewModel(_username);
                }
                else
                {
                    UserView mainView = new UserView();
                    mainView.Show();
                    mainView.DataContext = new UserViewModel(_username);
                }
                window.Close();
            }
            else
            {
                new InfoView("Неправильное имя пользователя или пароль").Show();
            }
        }
    }
}
