using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static GameClub.ViewModels.UserPages.DBPlacesViewModel;

namespace GameClub.ViewModels.AdminPages
{
    public class DBUsersViewModel : ViewModelBaseDataGrid<UserModel>
    {
        public DBUsersViewModel(string username)
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserModel();
            _currentUserAccount = userRepository.getByUsername(username);
            Users = new ObservableCollection<UserModel>(userRepository.SelectAll());
            if (Users.Count > 0) 
                SelectedUser = Users.First();
            _fields2 = new ObservableCollection<string>(userRepository.GetFields("user"));
        }

        ViewModelCommand _autoGeneratingColumnCommand;
        public ViewModelCommand AutoGeneratingColumnCommand
        {
            get
            {
                return _autoGeneratingColumnCommand ?? (_autoGeneratingColumnCommand = new ViewModelCommand(param =>
                {
                    var e = param as DataGridAutoGeneratingColumnEventArgs;
                    if (e != null)
                    {
                        if (e.PropertyName == "Image")
                            e.Cancel = true;
                        if (e.PropertyName == "Password")
                            e.Cancel = true;
                        if (e.PropertyName == "Id")
                            e.Column.Header = "Код";
                        if (e.PropertyName == "Username")
                            e.Column.Header = "Логин";
                        if (e.PropertyName == "Name")
                            e.Column.Header = "Имя";
                        if (e.PropertyName == "LastName")
                            e.Column.Header = "Фамилия";
                        if (e.PropertyName == "Email")
                            e.Column.Header = "Адрес почты";
                    }
                },
                param => true));
            }
        }

        protected string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    OnPropertyChanged("SelectedValue");
                }
            }
        
        }
        public ViewModelCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??
                  (_loadedCommand = new ViewModelCommand(obj =>
                  {
                      dataGrid = (obj as DataGrid);
                      Refresh(userRepository.SelectAll());
                     
                      Fields = new ObservableCollection<string>();
                      foreach (DataGridColumn column in dataGrid.Columns)
                      {
                          Fields.Add(column.Header.ToString());
                      }
                      SelectedValue = Fields.First();
                  }));
            }
        }

        protected override void Refresh(List<UserModel> models)
        {
            Users = new ObservableCollection<UserModel>(models);
            dataGrid.ItemsSource = Users;
            if (Users.Count > 0)
                SelectedUser = Users.First();
        }

        protected string _search;
        public string Search
        {
            get { return _search; }
            set
            {
                if (_search != value)
                {
                    _search = value;
                    Find(_search);
                    OnPropertyChanged("Search");
                }
            }
        }

        private void Find(string search)
        {
            if (search == null || search == "")
                Refresh(userRepository.SelectAll());
            else
                Refresh(userRepository.SelectAllByOption(search, _fields2[Fields.IndexOf(SelectedValue)], false));
        }

        private ObservableCollection<string> _fields2;

        protected ObservableCollection<string> _fields;
        public ObservableCollection<string> Fields
        {
            get { return _fields; }
            set
            {
                if (_fields != value)
                {
                    _fields = value;
                    OnPropertyChanged("Fields");
                }
            }
        }

        private UserModel _currentUserAccount;
        private UserRepository userRepository;

        private UserModel _selectedUser;
        
        public UserModel SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public ObservableCollection<UserModel> Users { get; set; }

        private ViewModelCommand _addCommand;
        public ViewModelCommand AddCommamd
        {
            get
            {
                return _addCommand ??
                  (_addCommand = new ViewModelCommand(obj =>
                  {
                      new Views.AdminPages.AddUserView().ShowDialog();
                      if ((LocalStorage.getObjectByName("dialogResultAddUser") as string) == "ok")
                      {
                          Refresh(userRepository.SelectAll());
                          LocalStorage.addItem("cancel", "dialogResultAddUser");
                      }
                  }));
            }
        }
        

        private ViewModelCommand _editCommand;
        public ViewModelCommand EditCommand
        {
            get
            {
                return _editCommand ??
                  (_editCommand = new ViewModelCommand(obj =>
                  {
                      var view =  new Views.AdminPages.AddUserView();
                      view.DataContext = new AddUserViewModel(SelectedUser);
                      view.ShowDialog();
                      if ((LocalStorage.getObjectByName("dialogResultAddUser") as string) == "ok")
                      {
                          Refresh(userRepository.SelectAll());
                          LocalStorage.addItem("cancel", "dialogResultAddUser");
                      }
                  }));
            }
        }

        private ViewModelCommand _deleteCommand;
        public ViewModelCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                  (_deleteCommand = new ViewModelCommand(obj =>
                  {
                      if ((new AcceptPage($"Вы  действительно хотите удалить пользователя {SelectedUser.Username}?")).ShowDialog() == true)
                      {
                          UserModel tempUser = SelectedUser;
                          if (Users.Remove(SelectedUser))
                              userRepository.deleteUser(tempUser);
                      }
                  }));
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

    }
}


