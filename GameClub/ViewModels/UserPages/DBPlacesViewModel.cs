using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.Dialogs;
using GameClub.Views.UserPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GameClub.ViewModels.UserPages
{
    public class DBPlacesViewModel : ViewModelBaseDataGrid<PlaceModel>
    {
        private ItemsControl _items;
        private PlaceModel _selectedPlace;
        public ObservableCollectionPropertyNotify<PlaceModel> Places { get; set; }
        public DBPlacesViewModel(string username, UserModel currentUser)
        {

            _placeRepository = new PlaceRepository();
            Places = new ObservableCollectionPropertyNotify<PlaceModel>(_placeRepository.SelectAll().Where(x=> x.Condition != "Недоступно"));
            _currentUser = currentUser;

            System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            myDispatcherTimer.Tick += myDispatcherTimer_Tick;
            myDispatcherTimer.Start();
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
        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            foreach (var place in Places.Where(x => x.Condition == "Занято"))
            {
                BookModel book = new BookRepository().getBookPlacesById(place.Id);
                if (book != null)
                {
                    place.timeLeft = HelpMethods.CalculateTime(book.StartTime, book.EndTime);
                    if (book.UserId == _currentUser.Id)
                    {
                        if (place.timeLeft == "Время вышло")
                        {
                            double CountOfHours = (new DateTime() + (book.EndTime - book.StartTime)).Hour;
                            int m = (new DateTime() + (book.EndTime - book.StartTime)).Minute;
                            if (m != 0)
                                CountOfHours += (m / (double)60);
                            double fixPrice = ((double)(book.Price / CountOfHours)); ;
                            new BookRepository().endBook(book.Id, place.Description, fixPrice, CountOfHours, book.UserId);
                            new InfoView("Ваш сеанс окончен").Show();
                        }
                    }
                }
            }
            Places.Refresh();
        }
        public ViewModelCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??
                  (_loadedCommand = new ViewModelCommand(obj =>
                  {
                      _items = (obj as ItemsControl);
                      Refresh(_placeRepository.SelectAll().Where(x => x.Condition != "Недоступно").ToList());
                  }));
            }
        }

        protected override void Refresh(List<PlaceModel> models)
        {
            Places = new ObservableCollectionPropertyNotify<PlaceModel>(models);
            _items.ItemsSource = Places;
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

        private UserModel _currentUser;
        private PlaceRepository _placeRepository;

        private ViewModelCommand _bookCommand;
        public ViewModelCommand BookCommand
        {
            get
            {
                return _bookCommand ??
                  (_bookCommand = new ViewModelCommand(obj =>
                  {
                      var item = obj as PlaceModel;
                      if (item.Condition == "Занято")
                      {
                          new InfoView("Место уже занято").Show();
                          return;
                      }
                      if((new BookRepository()).checkActiceBooks(_currentUser.Id))
                      {
                          new InfoView("У вас уже есть забронированное место").Show();
                          return;
                      }
                      BookPlaceView bookPlaceView = new BookPlaceView();
                      bookPlaceView.DataContext = new BookPlaceViewModel(item.Id, item.Price,_currentUser);
                      bookPlaceView.ShowDialog();
                      if((LocalStorage.getObjectByName("dialogResultBookPlaceView") as string) == "ok")
                      {
                          Places = new ObservableCollectionPropertyNotify<PlaceModel>(_placeRepository.SelectAll().Where(x => x.Condition != "Недоступно"));
                          _items.ItemsSource = Places;
                      }

                  }));
            }
        }

    }
}


