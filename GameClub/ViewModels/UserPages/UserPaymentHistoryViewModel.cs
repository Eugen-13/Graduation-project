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

namespace GameClub.ViewModels.UserPages
{
    internal class UserPaymentHistoryViewModel : ViewModelBaseDataGrid<PayModel>
    {
        public ObservableCollectionPropertyNotify<PayModel> Payments { get; set; }
        UserModel userCurrent;
        public UserPaymentHistoryViewModel(UserModel user)
        {
            userCurrent = user;
            _payRepository = new PayRepository();
            Payments = new ObservableCollectionPropertyNotify<PayModel>(_payRepository.SelectAllByOption(user.Id.ToString(), "User_id", true));
            _fields2 = new ObservableCollection<string>(_payRepository.GetFields("pay"));
        }
        private PayModel book;
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
                      Refresh(_payRepository.SelectAllByOption(userCurrent.Id.ToString(), "User_id", true));
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
                    Refresh(_payRepository.SelectAll().Where(x => x.UserLogin.Contains(search) && x.UserId == userCurrent.Id).ToList());
                else
                    Refresh(_payRepository.SelectAllByOption(search, _fields2[Fields.IndexOf(SelectedValue)], true).Where(x => x.UserId == userCurrent.Id).ToList());
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