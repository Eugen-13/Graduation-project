using GameClub.Models;
using GameClub.Repositories;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts.Configurations;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Printing;

namespace GameClub.ViewModels.AdminPages
{
    public class PaymentViewModel : ViewModelBaseDataGrid<PayModel>
    {
        ListView _items;
        public ViewModelCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??
                  (_loadedCommand = new ViewModelCommand(obj =>
                  {
                    
                      var values = (object[])obj;
                      _items = (values[1] as ListView);
                      dataGrid = (values[0] as DataGrid);
                      Refresh(payRepository.SelectAll());
                  }));
            }
        }

        protected override void Refresh(List<PayModel> models)
        {
            Payments = new ObservableCollectionPropertyNotify<PayModel>(models);
            InitGraph();
            dataGrid.ItemsSource = Payments;
            _items.ItemsSource = PayItems;
            Money = payRepository.getBalance();

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
                            e.Column.Header = "Описание";
                        if (e.PropertyName == "Price")
                            e.Column.Header = "Цена за ед. (BYN)";
                        if (e.PropertyName == "Count")
                            e.Column.Header = "Кол-во";
                        if (e.PropertyName == "Pay_Date")
                            e.Column.Header = "Дата оплаты";
                        if (e.PropertyName == "Result")
                            e.Column.Header = "Цена (BYN)";
                        if (e.PropertyName == "UserLogin")
                            e.Column.Header = "Клиент";
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
                      FlowDocument document = new FlowDocument();
                      Paragraph paragraph = new Paragraph();
                      TextBlock textBlock1 = new TextBlock();
                      textBlock1.Inlines.Add(new Run("Номер \tПрибыль \tДата"));
                      paragraph.Inlines.Add(textBlock1);
                      document.Blocks.Add(paragraph);
                      int i = 0;
                      foreach (Item item in _items.Items)
                      {
                          if(i == 0)
                          {
                              i++;
                              continue;
                          }
                          paragraph = new Paragraph();
                          textBlock1 = new TextBlock();
                          textBlock1.Inlines.Add(new Run(item.Line1));
                          paragraph.Inlines.Add(textBlock1);
                          document.Blocks.Add(paragraph);
                      }
                      document.FontFamily = new FontFamily("Segoe UI");
                      document.FontSize = 18;
                      document.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF121212"));
                      PrintDialog printDialog = new PrintDialog();
                      if (printDialog.ShowDialog() == true)
                      {
                          printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "MyPrintJob");
                      }
                  }));
            }
        }
        public class Item
        {
            public string Line1 { get; set; }
            public string Line2 { get; set; } = null;
        }
        public ObservableCollectionPropertyNotify<PayModel> Payments { get; set; }
        private double _money;
        public ObservableCollection<Item> PayItems { get; set; }

        private PayRepository payRepository;

        public double Money
        {
            get
            {
                return _money;
            }
            set
            {
                _money = value;
                OnPropertyChanged(nameof(Money));
            }
        }

        public PaymentViewModel()
        {
            payRepository = new PayRepository();
            Money = payRepository.getBalance();
            InitGraph();

        }


        private SeriesCollection _senderChart;
        public SeriesCollection SenderChart
        {
            get { return _senderChart; }
            set
            {
                _senderChart = value;
                OnPropertyChanged("SenderChart");
            }
        }
        private void InitGraph()
        {
            PayItems = new ObservableCollection<Item>()
            {
                new Item() { Line1 = "Номер\tДоход за      Дата", Line2 = "дня\tдень (BYN)" }
            };
            List<double> money = new List<double>();
            money.Add(0);
            if(SenderChart != null)
                SenderChart.Clear();
            if (payRepository.SelectAll().Count > 0)
            {
                int n = 0;
                int day = payRepository.SelectAll()[0].PayDate.Day;
                for (int i = 0; i < payRepository.SelectAll().Count; i++)
                {
                    if (day == payRepository.SelectAll()[i].PayDate.Day)
                    {
                        money[n] += payRepository.SelectAll()[i].Result;
                    }
                    else
                    {
                        PayItems.Add(new Item() { Line1 = $"{n + 1}.\t{money[n]}\t{payRepository.SelectAll()[i].PayDate.ToString("d")}" });
                        day = payRepository.SelectAll()[i].PayDate.Day;
                        money.Add(0);
                        n++;
                        money[n] += payRepository.SelectAll()[i].Result;
                    }

                }
                if (n == 0)
                {
                    PayItems.Add(new Item() { Line1 = $"{n + 1}.\t{money[n]}\t{payRepository.SelectAll()[payRepository.SelectAll().Count - 1].PayDate.ToString("d")}" });
                    day = payRepository.SelectAll()[payRepository.SelectAll().Count - 1].PayDate.Day;
                    money.Add(0);
                    n++;
                }
                var mapper1 = new CartesianMapper<double>()
                    .X((value, index) => index + 1)
                    .Y((value, index) => value);
                _senderChart = new SeriesCollection(mapper1);
                var columnSeries = new ColumnSeries() { Values = new ChartValues<double>(), Title = "Доход за дни" };
                foreach (var val in money)
                {
                    columnSeries.Values.Add(val);
                }
                this._senderChart.Add(columnSeries);
                OnPropertyChanged(nameof(SenderChart));
            }
            else
                OnPropertyChanged(nameof(SenderChart));
        }
    }
}
