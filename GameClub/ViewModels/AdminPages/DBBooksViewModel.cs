using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.AdminPages.Places;
using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;
using GameClub.Views.Dialogs;

namespace GameClub.ViewModels.AdminPages
{
    public class DBBooksViewModel : ViewModelBaseDataGrid<BookModel>
    {
        public ObservableCollectionPropertyNotify<BookModel> Books { get; set; }
        public DBBooksViewModel()
        {
            bookRepository = new BookRepository();
            Books = new ObservableCollectionPropertyNotify<BookModel>(bookRepository.SelectAll());
            if(Books.Count>0)
                SelectedBook = Books.First();
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
        private ViewModelCommand _printCommand;
        public ViewModelCommand PrintCommamd
        {
            get
            {
                return _printCommand ??
                  (_printCommand = new ViewModelCommand(obj =>
                  {
                      PrintDialog printDialog = new PrintDialog();
                      if (printDialog.ShowDialog() == true)
                      {
                          List<object> dataItems = new List<object>();
                          foreach (var item in dataGrid.ItemsSource)
                          {
                              dataItems.Add(item);
                          }
                          if (dataItems.Count == 0)
                          {
                              new InfoView("Функция печати недоступна так как в данной базе нет ни одной записи.").Show();
                              return;
                          }
                          FixedDocument fixedDocument = new FixedDocument();
                          double printableWidth = printDialog.PrintableAreaWidth;
                          double printableHeight = printDialog.PrintableAreaHeight;
                          int currentItemIndex = 0;
                          while (currentItemIndex < dataItems.Count)
                          {
                              FixedPage fixedPage = new FixedPage();
                              fixedPage.Width = printableWidth;
                              fixedPage.Height = printableHeight;
                              Grid printGrid = new Grid();
                              printGrid.Width = fixedPage.Width;
                              printGrid.Height = fixedPage.Height;
                              DataGrid clonedDataGrid = new DataGrid();
                              clonedDataGrid.AutoGenerateColumns = false;
                              foreach (DataGridColumn column in dataGrid.Columns)
                              {
                                  DataGridColumn clonedColumn = null;
                                  if (column is DataGridTextColumn textColumn)
                                  {
                                      clonedColumn = new DataGridTextColumn();
                                      ((DataGridTextColumn)clonedColumn).Binding = textColumn.Binding;
                                      ((DataGridTextColumn)clonedColumn).Header = textColumn.Header;
                                  }
                                  else if (column is DataGridCheckBoxColumn checkBoxColumn)
                                  {
                                      clonedColumn = new DataGridCheckBoxColumn();
                                      ((DataGridCheckBoxColumn)clonedColumn).Binding = checkBoxColumn.Binding;
                                      ((DataGridCheckBoxColumn)clonedColumn).Header = checkBoxColumn.Header;
                                  }
                                  if (clonedColumn != null)
                                  {
                                      clonedDataGrid.Columns.Add(clonedColumn);
                                  }
                              }
                              int itemsPerPage = (int)(printableHeight / HelpMethods.GetFirstVisualChild<DataGridRow>(dataGrid).ActualHeight);
                              int remainingItems = dataItems.Count - currentItemIndex;
                              int itemsToAdd = Math.Min(itemsPerPage, remainingItems);
                              for (int i = currentItemIndex; i < currentItemIndex + itemsToAdd; i++)
                              {
                                  clonedDataGrid.Items.Add(dataItems[i]);
                              }
                              printGrid.Children.Add(clonedDataGrid);
                              fixedPage.Children.Add(printGrid);
                              PageContent pageContent = new PageContent();
                              ((IAddChild)pageContent).AddChild(fixedPage);
                              fixedDocument.Pages.Add(pageContent);
                              currentItemIndex += itemsPerPage;
                          }
                          printDialog.PrintDocument(fixedDocument.DocumentPaginator, "Бронирование");
                      }
                  }));
            }
        }

        private ViewModelCommand _clearBookHistoryCommand;
        public ViewModelCommand ClearBookHistoryCommand
        {
            get
            {
                return _clearBookHistoryCommand ??
                  (_clearBookHistoryCommand = new ViewModelCommand(obj =>
                  {
                      if ((new AcceptPage($"Вы действительно хотите очистить историю бронирования? Учтите историю невозможно будет восставновить.")).ShowDialog() == true)
                      {
                          bookRepository.Clear();
                          Refresh(bookRepository.SelectAll());
                      }
                  }));
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
                      Refresh(bookRepository.SelectAll());
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
                Refresh(bookRepository.SelectAll());
            else
                Refresh(bookRepository.SelectAllByOption(search, _fields2[Fields.IndexOf(SelectedValue)], true));
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









