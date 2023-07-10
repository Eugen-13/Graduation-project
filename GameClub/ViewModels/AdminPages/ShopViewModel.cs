using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using MaterialDesignColors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;

namespace GameClub.ViewModels.AdminPages
{
    public class ShopViewModel : ViewModelBaseDataGrid<PurchaseModel>
    {
        public ShopViewModel()
        {
            _purchaseRepository = new PurchaseRepository();
            Purchase = new ObservableCollection<PurchaseModel>(_purchaseRepository.SelectAll());
        }

        private ItemsControl _items;
        public ObservableCollection<PurchaseModel> Purchase { get; set; }
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


        private ViewModelCommand _deleteCommand;
        public ViewModelCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                  (_deleteCommand = new ViewModelCommand(obj =>
                  {
                     
                      var item = obj as PurchaseModel;
                      if ((new AcceptPage($"Вы  действительно хотите удалить товар {item.Name}?")).ShowDialog() == true)
                      {
                          _purchaseRepository.deletePurchase(item.Id);
                          Purchase.Remove(item);
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



