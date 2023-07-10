using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.AdminPages;
using GameClub.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GameClub.Views.Dialogs;
using System.Net;

namespace GameClub.ViewModels.UserPages
{
    internal class ShopViewModel : ViewModelBaseDataGrid<PurchaseModel>
    {
        public ShopViewModel()
        {
            _purchaseRepository = new PurchaseRepository();
            Purchase = new ObservableCollection<PurchaseModel>(_purchaseRepository.SelectAll());
        }
        public ShopViewModel(UserModel user)
        {
            _purchaseRepository = new PurchaseRepository();
            Purchase = new ObservableCollection<PurchaseModel>(_purchaseRepository.SelectAll());
            _currentUser = user;
        }

        private ItemsControl _items;
        private UserModel _currentUser;
        public ObservableCollection<PurchaseModel> Purchase { get; set; }
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
                      _items = (obj as ItemsControl);
                      Refresh(_purchaseRepository.SelectAll());
                  }));
            }
        }

        protected override void Refresh(List<PurchaseModel> models)
        {
            Purchase = new ObservableCollection<PurchaseModel>(models);
            _items.ItemsSource = Purchase;
        }

      

        private ViewModelCommand _plusOne;
        public ViewModelCommand PlusOne
        {
            get
            {
                return _plusOne ??
                  (_plusOne = new ViewModelCommand(obj =>
                  {
                      var item = obj as PurchaseModel;
                      if(item.CurrentCount < item.Count)
                        item.CurrentCount += 1;
                  }));
            }
        }

        private ViewModelCommand _minusOne;
        public ViewModelCommand MinusOne
        {
            get
            {
                return _minusOne ??
                  (_minusOne = new ViewModelCommand(obj =>
                  {
                      var item = obj as PurchaseModel;
                      if (item.CurrentCount > 0)
                          item.CurrentCount -= 1;
                  }));
            }
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
                Refresh(_purchaseRepository.SelectAll());
            else
                Refresh(_purchaseRepository.SelectAllByOption(search, "Name", false));
        }

     
        private PurchaseRepository _purchaseRepository;
        public ObservableCollection<UserModel> Users { get; set; }

        private ViewModelCommand _addCommand;
        public ViewModelCommand AddCommamd
        {
            get
            {
                return _addCommand ??
                  (_addCommand = new ViewModelCommand(obj =>
                  {
                      new Views.AdminPages.EditOrAddPurchaseView().ShowDialog();
                      Refresh(_purchaseRepository.SelectAll());
                  }));
            }
        }


        private ViewModelCommand _buyCommand;
        public ViewModelCommand BuyCommand
        {
            get
            {
                return _buyCommand ??
                  (_buyCommand = new ViewModelCommand(obj =>
                  {
                      var item = obj as PurchaseModel;
                      if(item.CurrentCount  == 0)
                      {
                          new InfoView("Вы не выбрали кол-во товара!").Show();
                          return;
                      }
                      if(new BookRepository().getBookUserById(_currentUser.Id) == null)
                      {
                          new InfoView("Для того чтобы покупать товар в нашем магазине вам необходимо забарнировать или начать сессию.").Show();
                          return;
                      }
                      if ((new AcceptPage($"Вы  действительно  хотите  приобрести {item.Name} в кол-ве {item.CurrentCount} по цене {item.Price * item.CurrentCount} ")).ShowDialog() == true)
                      {
                          new PayRepository().AddPay(item.Name, item.Price * item.CurrentCount, item.CurrentCount, _currentUser.Id);
                          new PurchaseRepository().IncreaseCount(item.CurrentCount, item.Id, item.Count);
                          Refresh(_purchaseRepository.SelectAll());
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
                      var item = obj as PurchaseModel;
                      var view = new Views.AdminPages.EditOrAddPurchaseView();
                      view.DataContext = new EditOrAddPurchaseViewModel(item);
                      view.ShowDialog();
                      Refresh(_purchaseRepository.SelectAll());
                  }));
            }
        }
    }
}



