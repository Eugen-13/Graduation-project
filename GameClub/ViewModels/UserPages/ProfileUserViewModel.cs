using GameClub.Models;
using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using GameClub.Helpers;
using GameClub.Views.Dialogs;
using System.Text.RegularExpressions;

namespace GameClub.ViewModels.UserPages
{
    public class ProfileUserViewModel : ViewModelBase
    {
        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        private int _Id;
        private string _username;
        private string _name;
        private string _lastName;
        private string _email;
        private string _password;

        private UserRepository userRepository;

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

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

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
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

        void Refresh(UserModel user)
        {
            User = user;
            Id = User.Id;
            Username = User.Username;
            Password = User.Password;
            Name = User.Name;
            LastName = User.LastName;
            Email = User.Email;
            if (user.Image != null)
                Image = user.Image;
            else
                Image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/Avatar.png", UriKind.Absolute));
    }

        public UserModel User { get; set; }
        public ProfileUserViewModel(UserModel user)
        {
            userRepository = new UserRepository();
            Refresh(user);

        }
        public ProfileUserViewModel() { }

        private ViewModelCommand _changeAvatarCommand;
        public ViewModelCommand ChangeAvatarCommand
        {
            get
            {
                return _changeAvatarCommand ??
                  (_changeAvatarCommand = new ViewModelCommand(obj =>
                  {

                  OpenFileDialog op = new OpenFileDialog();
                  op.Title = "Select a picture";
                  op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png";
                      if (op.ShowDialog() == true)
                      {
                          Image = new BitmapImage(new Uri(op.FileName));
                      }
                  }));
            }
        }

        private bool Validation()
        {
            try
            {
                string pattern = @"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$";
                if (_username.Length < 3)
                {
                    new InfoView("Логин должен быть от 3 символов").Show();
                    return false;
                }
                if (_password.Length < 3)
                {
                    new InfoView("Пароль должен быть от 3 символов").Show();
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
            }
            return false;

        }

        private ViewModelCommand _saveCommand;
        public ViewModelCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                  (_saveCommand = new ViewModelCommand(obj =>
                  {
                      if (Validation())
                      {
                          userRepository.updateUser(Id, Username, Password, Name, LastName, Email, Image);
                          Refresh(userRepository.getByUsername(Username));
                      }
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
                      Refresh(User);
                  }));
            }
        }
    }
}
