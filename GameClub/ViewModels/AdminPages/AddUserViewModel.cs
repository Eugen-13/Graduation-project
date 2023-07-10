using GameClub.Helpers;
using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using GameClub.Views.Dialogs;
using GameClub.Models;

namespace GameClub.ViewModels.AdminPages
{
    internal class AddUserViewModel : ViewModelBase
    {
        bool edit;
        private string _username;
        private string _name;
        private string _lastName;
        private string _email;
        private string _password;
        private string _rPassword;
        private string _startUserName;

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

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Lastname
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(Lastname));
            }
        }
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string RepeatPassword
        {
            get
            {
                return _rPassword;
            }
            set
            {
                _rPassword = value;
                OnPropertyChanged(nameof(RepeatPassword));
            }
        }

        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      var values = (object[])obj;
                      _password = ((PasswordBox)values[0]).Password;
                      _rPassword = ((PasswordBox)values[1]).Password;
                      if (Validation())
                      {
                          {
                              if (userRepository.checkOnExistingUser(_username) && _username != _startUserName)
                                  new InfoView("Пользователь с таким логином уже существует.").Show();
                              else
                              {
                                  if (edit)
                                      userRepository.updateUserWoImg(_id, _username, _password, _name, _lastName, _email);
                                  else
                                  {
                                      userRepository.addUser(_username, _password, _name, _lastName, _email, null);
                                  }
                                  Window loginView = values[2] as Window;
                                  LocalStorage.addItem("ok", "dialogResultAddUser");
                                  loginView.Close();
                              }
                          }
                      }
                  }));
            }
        }
        int _id;
        public AddUserViewModel(UserModel user)
        {
            _id = user.Id;
            _startUserName = user.Username;
            _username = user.Username;
            _name = user.Name;
            _lastName = user.LastName;
            _email = user.Email;
            _password = user.Password;
            _rPassword = user.Password;
            userRepository = new UserRepository();
            edit = true;
        }
        public AddUserViewModel()
        {
            userRepository = new UserRepository();
        }

        private bool Validation()
        {
            try
            {
                string pattern = @"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$";
                if (_username.Length < 3)
                {
                    new InfoView("Логин должен быть от 3 символовd").Show();
                    return false;
                }
                if (_password.Length < 3)
                {
                    new InfoView("Пароль должен быть от 3 символов").Show();
                    return false;
                }
                if (_password != _rPassword)
                {
                    new InfoView("Пароли не совпадают").Show();
                    return false;
                }
                if (!Regex.IsMatch(_email, pattern))
                {
                    new InfoView("Неверный формат email").Show();
                    return false;
                }
                if (!HelpMethods.Letters(_lastName))
                {
                    new InfoView("Неверный формат фамилии").Show();
                    return false;
                }
                if (!HelpMethods.Letters(_name))
                {
                    new InfoView("Неверный формат имени").Show();
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                new InfoView("Корректно заполните все поля!").Show();
            }
            return false;

        }

        private ViewModelCommand _cancelCommand;
        public ViewModelCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??
                  (_cancelCommand = new ViewModelCommand(obj =>
                  {
                      Window loginView = obj as Window;
                      loginView.Close();
                  }));
            }
        }
    }
}


