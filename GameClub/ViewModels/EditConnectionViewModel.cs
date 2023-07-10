using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace GameClub.ViewModels
{
    internal class EditConnectionViewModel : ViewModelBase
    {
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private readonly string _connectionFilePath = "connection.txt";

        public EditConnectionViewModel()
        {
            if (File.Exists(_connectionFilePath))
            {
                Text = File.ReadAllText("connection.txt");
            }
            else
            {
                Text = "GameClub_DB.db";
            }
        }

        
        private ViewModelCommand _defaultConnection;
        public ViewModelCommand DefaultConnection
        {
            get
            {
                return _defaultConnection ??
                  (_defaultConnection = new ViewModelCommand(obj =>
                  {
                      new UserRepository().ChangeConnectionDefault();
                      if (File.Exists(_connectionFilePath))
                      {
                          File.Delete(_connectionFilePath);
                      }
                      Text = "GameClub_DB.db";
                      new UserRepository().ChangeConnectionDefault();
                  }));
            }
        }

        private ViewModelCommand _closeCommandLocal;
        public ViewModelCommand CloseCommandLocal
        {
            get
            {
                return _closeCommandLocal ??
                  (_closeCommandLocal = new ViewModelCommand(obj =>
                  {
                      (obj as Window).Close();
                  }));
            }
        }
        private ViewModelCommand _changeConnection;
        public ViewModelCommand ChangeConnection
        {
            get
            {
                return _changeConnection ??
                  (_changeConnection = new ViewModelCommand(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog();
                      openFileDialog.Filter = "DB files (*.db)|*.db";
                      openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                      if (openFileDialog.ShowDialog() == true)
                      {
                          string selectedFileName = openFileDialog.FileName;
                          using (StreamWriter sw = File.CreateText(_connectionFilePath))
                          {
                              sw.WriteLine(selectedFileName);
                          }
                          Text = selectedFileName;
                          new UserRepository().ChangeConnection(Text);
                      }
                     
                  }));
            }
        }
    }
}
