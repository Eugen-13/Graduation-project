using GameClub.Models;
using GameClub.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GameClub.ViewModels.UserPages
{
    internal class UserBooksViewModel : ViewModelBaseDataGrid<BookModel>
    {
        public ObservableCollectionPropertyNotify<BookModel> Books { get; set; }
        UserModel userCurrent;
        public UserBooksViewModel(UserModel user)
        {
            userCurrent = user;
            bookRepository = new BookRepository();
            Books = new ObservableCollectionPropertyNotify<BookModel>(bookRepository.SelectAllByOption(user.Id.ToString(), "User_id",true));
            _fields2 = new ObservableCollection<string>(bookRepository.GetFields("book"));
        }
        private BookModel book;
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
                        switch (e.PropertyName)
                        {
                            case "IsReally":
                                e.Cancel = true;
                                break;
                            case "DateOfBook":
                                e.Cancel = true;
                                break;
                            case "StartDate":
                                e.Cancel = true;
                                break;
                            case "StartTime":
                                e.Cancel = true;
                                break;
                            case "EndTime":
                                e.Cancel = true;
                                break;
                            case "UserId":
                                e.Cancel = true;
                                break;
                            default:
                                break;
                        }
                        if (e.PropertyName == "Id")
                            e.Column.Header = "Код";
                        if (e.PropertyName == "UserLogin")
                            e.Column.Header = "Клиент";
                        if (e.PropertyName == "PlaceId")
                            e.Column.Header = "Место";
                        if (e.PropertyName == "Date_Of_Book")
                            e.Column.Header = "Дата брони";
                        if (e.PropertyName == "Start_Date")
                            e.Column.Header = "Дата сеанса";
                        if (e.PropertyName == "Start_Time")
                            e.Column.Header = "Начало";
                        if (e.PropertyName == "End_Time")
                            e.Column.Header = "Конец";
                        if (e.PropertyName == "Price")
                            e.Column.Header = "Цена (BYN)";
                    }
                    
                },
                param => true));
            }
        }

        public class ObservableCollectionPropertyNotify<T> : ObservableCollection<T>
        {
            public ObservableCollectionPropertyNotify(List<T> value) : base(value) { }
            public ObservableCollectionPropertyNotify(IEnumerable<T> value) : base(value) { }
            public void Refresh()
            {
                for (var i = 0; i < this.Count(); i++)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
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
                      Refresh(bookRepository.SelectAllByOption(userCurrent.Id.ToString(), "User_id", true));
                      Fields = new ObservableCollection<string>();
                      foreach (DataGridColumn column in dataGrid.Columns)
                      {
                          Fields.Add(column.Header.ToString());
                      }
                      SelectedValue = Fields.First();
                  }));
            }
        }

        protected override void Refresh(List<BookModel> models)
        {
            Books = new ObservableCollectionPropertyNotify<BookModel>(models);
            dataGrid.ItemsSource = Books;
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
                Refresh(bookRepository.SelectAll().Where(x=>x.UserId == userCurrent.Id).ToList());
            else
                Refresh(bookRepository.SelectAllByOption(search, _fields2[Fields.IndexOf(SelectedValue)], true).Where(x => x.UserId == userCurrent.Id).ToList());
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

        private BookModel _currentPlace;
        private BookRepository bookRepository;

        private BookModel _selectedBook;

        public BookModel SelectedBook
        {
            get
            {
                return _selectedBook;
            }
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
            }
        }

    }
}