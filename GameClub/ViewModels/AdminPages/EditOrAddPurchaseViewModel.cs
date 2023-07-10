using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameClub.ViewModels.AdminPages
{
    internal class EditOrAddPurchaseViewModel : ViewModelBase
    {
        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        bool _update = false;
        private string _name;
        private string _description;
        private double _price;
        private int _count;

        private PurchaseModel _purchaseModel;
        private PurchaseRepository _purchaseRepository;
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

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
        void Refresh(PurchaseModel purchase)
        {
            _purchaseRepository = new PurchaseRepository();
            _purchaseModel = purchase;
            Name = purchase.Name;
            Description = purchase.Description;
            Price = purchase.Price;
            Count = purchase.Count;
            if (purchase.Image != null)
                Image = purchase.Image;
        }

        public EditOrAddPurchaseViewModel(PurchaseModel purchase)
        {
            Refresh(purchase);
            _update = true;
        }
        public EditOrAddPurchaseViewModel() 
        {
            _purchaseRepository = new PurchaseRepository();
            Image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/purchase.png", UriKind.Absolute));
        }

        private ViewModelCommand _changeAvatarCommand;
        public ViewModelCommand ChangeAvatarCommand
        {
            get
            {
                return _changeAvatarCommand ??
                  (_changeAvatarCommand = new ViewModelCommand(obj =>
                  {

                      OpenFileDialog op = new OpenFileDialog();
                      op.Title = "Select a picture";
                      op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
                      if (op.ShowDialog() == true)
                      {
                          Image = new BitmapImage(new Uri(op.FileName));
                      }
                  }));
            }
        }



        private ViewModelCommand _saveCommand;
        public ViewModelCommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                  (_saveCommand = new ViewModelCommand(obj =>
                  {
                      try
                      {
                          if (Name.Length < 3)
                          {
                              new InfoView("Название должен быть от 3 символов.").Show();
                              return;
                          }
                          if (Name.Length > 32)
                          {
                              new InfoView("Название категории должен быть меньше 32 символов.").Show();
                              return;
                          }
                          if (Description.Length < 3)
                          {
                              new InfoView("Описание должен быть от 3 символов.").Show();
                              return;
                          }
                          if (Description.Length > 32)
                          {
                              new InfoView("Описание категории должен быть меньше 32 символов.").Show();
                              return;
                          }
                          if (Price <= 0)
                          {
                              new InfoView("Стоимость не может быть отрицательной или равной 0.").Show();
                              return;
                          }
                          if (Count <= 0)
                          {
                              new InfoView("Количество не может быть отрицательным или равным 0.").Show();
                              return;
                          }
                      }
                      catch (Exception)
                      {
                          new InfoView("Некорректно или непоностью заполнены данные.").Show();
                          return;
                      }
                      if (_update)
                          _purchaseRepository.updatePurchase(_purchaseModel.Id ,Name, Description, Price, Count, Image);
                      else
                          _purchaseRepository.addPurchase(Name, Description, Price, Count, Image);
                      (obj as Window).Close();
                  }));
            }
        }

        private ViewModelCommand _cancelCommand;
        public ViewModelCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??
                  (_cancelCommand = new ViewModelCommand(obj =>
                  {
                      (obj as Window).Close();
                  }));
            }
        }
    }
}
