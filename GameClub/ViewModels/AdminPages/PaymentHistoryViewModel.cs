using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;

namespace GameClub.ViewModels.AdminPages
{
    internal class PaymentHistoryViewModel : ViewModelBaseDataGrid<PayModel>
    {
        public ObservableCollectionPropertyNotify<PayModel> Payments { get; set; }
        public PaymentHistoryViewModel()
        {
            _payRepository = new PayRepository();
            Payments = new ObservableCollectionPropertyNotify<PayModel>(_payRepository.SelectAll());
            if (Payments.Count > 0)
                SelectedPay = Payments.First();
            _fields2 = new ObservableCollection<string>(_payRepository.GetFields("pay"));
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
                        if (e.PropertyName == "PayDate")
                            e.Cancel = true;
                        if (e.PropertyName == "UserId")
                            e.Cancel = true;
                        if (e.PropertyName == "Id")
                            e.Column.Header = "Код";
                        if (e.PropertyName == "Description")
                            e.Column.Header = "Товар";
                        if (e.PropertyName == "Price")
                            e.Column.Header = "Цена (BYN)";
                        if (e.PropertyName == "Count")
                            e.Column.Header = "Кол-во";
                        if (e.PropertyName == "Pay_Date")
                            e.Column.Header = "Дата оплаты";
                        if (e.PropertyName == "Result")
                            e.Column.Header = "Итог (BYN)";
                        if (e.PropertyName == "UserLogin")
                            e.Column.Header = "Клиент";
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
                      Refresh(_payRepository.SelectAll());
                      Fields = new ObservableCollection<string>();
                      foreach (DataGridColumn column in dataGrid.Columns)
                      {
                          Fields.Add(column.Header.ToString());
                      }
                      Fields.Remove("Итог (BYN)");
                      SelectedValue = Fields.First();
                  }));
            }
        }

        protected override void Refresh(List<PayModel> models)
        {
            Payments = new ObservableCollectionPropertyNotify<PayModel>(models);
            dataGrid.ItemsSource = Payments;
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
                Refresh(_payRepository.SelectAll());
            else
            {
                if (SelectedValue == "Клиент")
                    Refresh(_payRepository.SelectPayWithUserName(search));
                else
                    Refresh(_payRepository.SelectAllByOption(search, _fields2[Fields.IndexOf(SelectedValue)], true));
            }
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
                          printDialog.PrintDocument(fixedDocument.DocumentPaginator, "Оплата");
                      }
                  }));
            }
        }

        private ViewModelCommand _clearPayHistoryCommand;
        public ViewModelCommand ClearPayHistoryCommand
        {
            get
            {
                return _clearPayHistoryCommand ??
                  (_clearPayHistoryCommand = new ViewModelCommand(obj =>
                  {
                      if ((new AcceptPage($"Вы действительно хотите очистить историю оплаты? Учтите что данная таблица связана с графиком оплаты.")).ShowDialog() == true)
                      {
                          _payRepository.Clear();
                          Refresh(_payRepository.SelectAll());
                      }
                  }));
            }
        }
        private PayRepository _payRepository;

        private PayModel _selectedPay;

        public PayModel SelectedPay
        {
            get
            {
                return _selectedPay;
            }
            set
            {
                _selectedPay = value;
                OnPropertyChanged(nameof(SelectedPay));
            }
        }

    }
}









