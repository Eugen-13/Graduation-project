using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using GameClub.Views.UserPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;

namespace GameClub.ViewModels.UserPages
{
    internal class ViewSelectedPlaceViewModel
    {
        public PlaceModel Place { get; set; }
        PlaceRepository placeRepository { get; set; }
        UserModel _currentUser;
        public ViewSelectedPlaceViewModel(PlaceModel place, UserModel currentUser)
        {
            placeRepository = new PlaceRepository();
            Place = place;
            _currentUser = currentUser;
        }

        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      var item = Place;
                      if (item.Condition == "Занято")
                      {
                          new InfoView("Место уже занято").Show();
                          return;
                      }
                      if ((new BookRepository()).checkActiceBooks(_currentUser.Id))
                      {
                          new InfoView("У вас уже есть забронированное место").Show();
                          return;
                      }
                      BookPlaceView bookPlaceView = new BookPlaceView();
                      bookPlaceView.DataContext = new BookPlaceViewModel(item.Id, item.Price, _currentUser);
                      bookPlaceView.ShowDialog();
                      Place = placeRepository.getPlaceById(Place.Id);
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
                      Place = null;
                      (obj as Window).Close();
                  }));
            }
        }

      
    }
}
