using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;

namespace GameClub.ViewModels.AdminPages
{
    internal class EditMapViewModel : ViewModelBase
    {
        private Visibility _visibility = Visibility.Collapsed;
        public Visibility visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(visibility));
            }
        }
        private bool isText;
        public bool IsText
        {
            get
            {
                return isText;
            }
            set
            {
                isText = value;
                OnPropertyChanged(nameof(IsText));
            }
        }
        private bool isPC;
        public bool IsPC
        {
            get
            {
                return isPC;
            }
            set
            {
                isPC = value;
                OnPropertyChanged(nameof(IsPC));
            }
        }
        private MapModel _model;
        public MapModel Model
        {
            get
            {
                return _model;
            }

            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        public ObservableCollection<string> Fields { get; set; }
        public string _selectedValue;
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
        Dictionary<string , int>  dict = new Dictionary<string,int>();
        public EditMapViewModel(MapModel map,bool isNew,List<int> ids)
        {
            Model = map;
            Text = map.Text;
            if (isNew)
                visibility = Visibility.Collapsed;
            else
                visibility = Visibility.Visible;
            IsText = Model.IsPlace == 0 ? true : false;
            IsPC = !IsText;
            Fields = new ObservableCollection<string>();
            var temp = new PlaceRepository().SelectAll();
            temp.RemoveAll(x => ids.Contains(x.Id));
            foreach (var item in temp)
            {
                dict.Add(item.Description, item.Id);
                Fields.Add(item.Description);
            }
            if (!isNew && !isText)
            {
                dict.Add(new PlaceRepository().getPlaceById(Model.PlaceId).Description, new PlaceRepository().getPlaceById(Model.PlaceId).Id);
                Fields.Add(new PlaceRepository().getPlaceById(Model.PlaceId).Description);
            }
            if(Fields .Count > 0)   
                SelectedValue = Fields.Last();
        }

        public EditMapViewModel(){}

        private ViewModelCommand _saveChangesCommand;
        public ViewModelCommand SaveChangesCommand
        {
            get
            {
                return _saveChangesCommand ??
                  (_saveChangesCommand = new ViewModelCommand(obj =>
                  {
                      
                      if (isPC == true)
                      {
                          if (dict.Count == 0)
                          {
                              new InfoView("Вы не можите сохранить точку как место, т. к. в списке нет доступных мест.").Show();
                              return;
                          }
                          Model.PlaceId = dict[SelectedValue];
                          Model.Text = "";
                          Model.IsPlace = 1;
                      }
                      else
                      {
                          Model.Text = Text;
                          Model.IsPlace = 0;
                          Model.PlaceId = -1;
                      }
                  
                          
                      (obj as Window).Close();
                      
                  }));
            }
        }

        private ViewModelCommand _cancelChangesCommand;
        public ViewModelCommand CancelChangesCommand
        {
            get
            {
                return _cancelChangesCommand ??
                  (_cancelChangesCommand = new ViewModelCommand(obj =>
                  {
                      (obj as Window).Close();
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
                      Model.IsPlace = -13;
                      (obj as Window).Close();
                  }));
            }
        }
    }
}

