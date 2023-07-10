using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameClub.ViewModels.AdminPages
{
    internal class EditDBPlacesViewModel : ViewModelBase
    {
        bool edit;
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
        public PlaceModel Place { get; set; }
        PlaceRepository placeRepository { get; set; }
        public EditDBPlacesViewModel()
        {
            Place = new PlaceModel();
            placeRepository = new PlaceRepository();
            Categoryes = new ObservableCollection<string>(placeRepository.getCategores());
            Coinditions = new ObservableCollection<string>(placeRepository.getConditions().Where(x => x != "Занято"));
            if (Categoryes.Count > 0)
                SelectedCategory = Categoryes.First();
            if (Coinditions.Count > 0)
                SelectedCoinditions = Coinditions.First();
            edit = false;
            Image = new PlaceModel().Image;
          
        }
        public EditDBPlacesViewModel(PlaceModel place)
        {
            placeRepository = new PlaceRepository();
            Place = place;
            Categoryes = new ObservableCollection<string>(placeRepository.getCategores());
            if (Place.Condition == "Занято")
                Coinditions = new ObservableCollection<string>(placeRepository.getConditions());
            else
                Coinditions = new ObservableCollection<string>(placeRepository.getConditions().Where(x => x != "Занято"));
            SelectedCategory = Categoryes.First(x => x == Place.Category);
            SelectedCoinditions = Coinditions.First(x => x == Place.Condition);
            edit = true;
            if (place.Image != null)
                Image = place.Image;
            else
                Image = new PlaceModel().Image;
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
        private ViewModelCommand _addCategoryCommand;
        public ViewModelCommand AddCategoryCommand
        {
            get
            {
                return _addCategoryCommand ??
                  (_addCategoryCommand = new ViewModelCommand(obj =>
                  {
                      new Views.AdminPages.AddCategory().ShowDialog();
                      if (LocalStorage.getObjectByName("dialogResultAddorEditCategory") as string == "ok")
                      {
                          Categoryes = new ObservableCollection<string>(placeRepository.getCategores());
                          if (SelectedCategory != null)
                              SelectedCategory = Categoryes.First(x => x == Place.Category);
                          LocalStorage.addItem("cancel", "dialogResultAddorEditCategory");
                      }
                  }));
            }
        }

        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      try
                      {
                          if (Place.Description.Length < 3)
                          {
                              new InfoView("Описание должен быть от 3 символов.").Show();
                              return;
                          }
                          if (Place.Description.Length > 32)
                          {
                              new InfoView("Описание категории должен быть меньше 32 символов.").Show();
                              return;
                          }
                          if (Place.Performance.Length < 3)
                          {
                              new InfoView("Характеристики должны быть от 3 символов.").Show();
                              return;
                          }
                          if (Place.Performance.Length > 200)
                          {
                              new InfoView("Характеристики должны быть меньше 200 символов.").Show();
                              return;
                          }
                      }
                      catch (Exception)
                      {
                          new InfoView("Некорректно или непоностью заполнены данные.").Show();
                          return;
                      }
                      Place.Image = Image;
                      if (edit)
                          placeRepository.EditPlace(Place);
                      else
                          placeRepository.AddPlace(Place);
                      (obj as Window).Close();
                      LocalStorage.addItem("ok", "dialogResultAddorEditPlace");

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
                      LocalStorage.addItem("cancel", "dialogResultAddorEditCategory");
                  }));
            }
        }

        protected ObservableCollection<string> _categories;
        public ObservableCollection<string> Categoryes
        {
            get { return _categories; }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged("Categoryes");
                }
            }
        }

        protected string _selectedCategory;
        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    Place.Category = value;
                    OnPropertyChanged("SelectedCategory");
                }
            }

        }
        protected ObservableCollection<string> _conditions;
        public ObservableCollection<string> Coinditions
        {
            get { return _conditions; }
            set
            {
                if (_conditions != value)
                {
                    _conditions = value;
                    OnPropertyChanged("Coinditions");
                }
            }
        }

        protected string _selectedCoinditions;
        public string SelectedCoinditions
        {
            get { return _selectedCoinditions; }
            set
            {
                if (_selectedCoinditions != value)
                {
                    _selectedCoinditions = value;
                    Place.Condition = value;
                    OnPropertyChanged("SelectedCoinditions");
                }
            }

        }
    }
}
