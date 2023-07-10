using GameClub.Repositories;
using GameClub.Views;
using GameClub.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.BouncyCastle.Utilities;
using System.Text.RegularExpressions;
using System.Drawing;
using GameClub.Views.Dialogs;
using System.Net.Mail;
using System.Net;
using System.Threading;

namespace GameClub.ViewModels
{
    internal class RegistrationViewModel : ViewModelBase
    {
        private Visibility _visibility  = Visibility.Collapsed;
        public Visibility visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(visibility));
            }
        }
        private string _username;
        private string _name;
        private string _lastName;
        private string _email;
        private string _password;
        private string _rPassword;

        private string _generatedCode;
        private string _code;

        private UserRepository userRepository;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                OnPropertyChanged(nameof(Code));
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

        private ViewModelCommand _goToLoginCommand;
        public ViewModelCommand GoToLoginCommand
        {
            get
            {
                return _goToLoginCommand ??
                  (_goToLoginCommand = new ViewModelCommand(obj =>
                  {
                      Window regView = obj as Window;
                      Application.Current.MainWindow.Show();
                      regView.Close();
                  }));
            }
        }

        public RegistrationViewModel()
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
                    new InfoView("Логин должен быть от 3 символов.").Show();
                    return false;
                }
                if (_password.Length < 3)
                {
                    new InfoView("Пароль должен быть от 3 символов.").Show();
                    return false;
                }
                if (_password != _rPassword)
                {
                    new InfoView("Пароли не совпадают.").Show();
                    return false;
                }
                if (!Regex.IsMatch(_email, pattern))
                {
                    new InfoView("Неверный формат email.").Show();
                    return false;
                }
                if (!HelpMethods.Letters(_lastName))
                {
                    new InfoView("Неверный формат фамилии.").Show();
                    return false;
                }
                if (!HelpMethods.Letters(_name))
                {
                    new InfoView("Неверный формат имени.").Show();
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
        private void SendEmail()
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("zheka137.soloveyv@mail.ru", "s3L7pmJjL5HBLbENnPyt");
                MailMessage Message = new MailMessage();
                Message.From = new MailAddress("zheka137.soloveyv@mail.ru");
                Message.Subject = "Код подтверждения";
                Message.To.Add(new MailAddress(Email));
                Message.Body = $"Код для подтверждения: {_generatedCode}";
                smtp.Send(Message);
            }
            catch (Exception)
            {
                new InfoView("Ошибка отправки сообщения на почту, такой почты либо не существует либо нет подключения к интернету.").Show();
            }

        }
        private ViewModelCommand _registrationCommand;
        public ViewModelCommand RegistrationCommand
        {
            get
            {
                return _registrationCommand ??
                  (_registrationCommand = new ViewModelCommand(obj =>
                  {
                      var values = (object[])obj;
                      _password = ((PasswordBox)values[0]).Password;
                      _rPassword = ((PasswordBox)values[1]).Password;
                      if (Validation())
                      {
                          if (userRepository.checkOnExistingUser(_username))
                              new InfoView("Такой пользователь уже существует").Show();
                          else
                          {
                              visibility = Visibility.Visible;
                              _generatedCode = HelpMethods.GenerateCode();
                              new InfoView("На вашу почту отправлен код с подтверждением чтобы обновить код нажмите регистрацию ещё один раз.").Show();
                              Thread.Sleep(1000);
                              SendEmail();
                          }
                      }
                  }));
            }
        }
        private ViewModelCommand _registrationCommand2;
        public ViewModelCommand RegistrationCommand2
        {
            get
            {
                return _registrationCommand2 ??
                  (_registrationCommand2 = new ViewModelCommand(obj =>
                  {
                      if (_generatedCode == Code)
                      {
                          userRepository.addUser(_username, _password, _name, _lastName, _email, null);
                          Window loginView = obj as Window;
                          new InfoView("Вы успешно зарегестрированны").Show();
                          loginView.Close();
                          Application.Current.MainWindow.Show();
                      }
                      else
                          new InfoView("Неверный код").Show();
                  }));
            }
        }
    }
}
