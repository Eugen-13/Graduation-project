using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.AdminPages.Places;
using GameClub.Views.Dialogs;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Azure.Core.HttpHeader;

namespace GameClub.ViewModels.AdminPages
{
    public class DBPlacesViewModel : ViewModelBaseDataGrid<PlaceModel>
    {
        private ItemsControl _items;
        public ObservableCollectionPropertyNotify<PlaceModel> Places { get; set; }
        public DBPlacesViewModel(string username)
        {
            placeRepository = new PlaceRepository();
            Places = new ObservableCollectionPropertyNotify<PlaceModel>(placeRepository.SelectAll());
            System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            myDispatcherTimer.Tick += myDispatcherTimer_Tick;
            myDispatcherTimer.Start();
        }
        public DBPlacesViewModel() { }
        private BookModel book;
        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            foreach (var place in Places.Where(x => x.Condition == "Занято"))
            {
                book = new BookRepository().getBookPlacesById(place.Id);
                if (book != null)
                {
                    place.timeLeft = HelpMethods.CalculateTime(book.StartTime, book.EndTime);
                    if (place.timeLeft == "Время вышло")
                    {
                        double CountOfHours = (new DateTime() + (book.EndTime - book.StartTime)).Hour;
                        int m = (new DateTime() + (book.EndTime - book.StartTime)).Minute;
                        if (m != 0)
                            CountOfHours += (m / (double)60);
                        double fixPrice = ((double)(book.Price / CountOfHours)); ;
                        new BookRepository().endBook(book.Id, place.Description, fixPrice, CountOfHours,book.UserId);
                        new InfoView($"Сеанс пользователя {new UserRepository().getById(book.UserId).Username} окончен.").Show();
                    }
                }
            }
            Places.Refresh();

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
        public ViewModelCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??
                  (_loadedCommand = new ViewModelCommand(obj =>
                  {
                      _items = (obj as ItemsControl);
                      Refresh(placeRepository.SelectAll());
                  }));
            }
        }

        protected override void Refresh(List<PlaceModel> models)
        {
            Places = new ObservableCollectionPropertyNotify<PlaceModel>(models);
            _items.ItemsSource = Places;
        }
        private PlaceRepository placeRepository;

        private ViewModelCommand _editCommand;
        public ViewModelCommand EditCommand
        {
            get
            {
                return _editCommand ??
                  (_editCommand = new ViewModelCommand(obj =>
                  {
                      var item = obj as PlaceModel;
                      var view = new Views.AdminPages.Places.EditPlacesVIew();
                      view.DataContext = new EditDBPlacesViewModel(item);
                      view.ShowDialog();
                      if ((LocalStorage.getObjectByName("dialogResultAddorEditPlace") as string) == "ok")
                      {
                          Refresh(placeRepository.SelectAll());
                          LocalStorage.addItem("cancel", "dialogResultAddorEditPlace");
                      }
                  }));
            }
        }
        private ViewModelCommand _addCommand;
        public ViewModelCommand AddCommamd
        {
            get
            {
                return _addCommand ??
                  (_addCommand = new ViewModelCommand(obj =>
                  {
                      new Views.AdminPages.Places.EditPlacesVIew().ShowDialog();
                      if ((LocalStorage.getObjectByName("dialogResultAddorEditPlace") as string) == "ok")
                      {
                          Refresh(placeRepository.SelectAll());
                          LocalStorage.addItem("cancel", "dialogResultAddorEditPlace");
                      }
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
                      var item = obj as PlaceModel;
                      if ((new AcceptPage($"Вы  действительно хотите удалить место {item.Description}?")).ShowDialog() == true)
                      {
                          placeRepository.DeletePlace(item);
                          Places.Remove(item);
                      }
                  }));
            }
        }

        private List<PayModel> _payList;
        private ViewModelCommand _startNewSessionCommand;
        public ViewModelCommand StartNewSessionCommand
        {
            get
            {
                return _startNewSessionCommand ??
                  (_startNewSessionCommand = new ViewModelCommand(obj =>
                  {
                      var item = obj as PlaceModel;
                      if (new BookRepository().getBookPlacesById(item.Id) == null)
                      {
                          var temp = new BookRepository().SelectAll();
                          BookModel _book;
                          if (temp.FindIndex(x => x.PlaceId == item.Id) != -1)
                          {
                              _book = new BookRepository().SelectAll().Where(x => x.PlaceId == item.Id).Last();
                              _payList = new PayRepository().SelectAll().Where(x => x.UserId == _book.UserId && x.PayDate >= _book.StartDate.AddHours(_book.StartTime.Hour).AddMinutes(_book.StartTime.Minute) && x.PayDate <= _book.StartDate.AddHours(_book.EndTime.Hour).AddMinutes(_book.EndTime.Minute)).ToList();
                            
                          }
                          else
                          {
                              new InfoView("В базе данных нету информации о данном сеансе, возможно кто-то очистил историю бронирования.").Show();
                              placeRepository.StartNewSession(item.Id);
                              Refresh(placeRepository.SelectAll());
                              return;
                          }
                          new InfoView("Сейчас будет распечатан чек.").Show();
                          PrintDocument printDoc = new PrintDocument();
                          printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                          Thread.Sleep(1000);
                          PrintDialog printDialog = new PrintDialog();
                          if (printDialog.ShowDialog() == true)
                          {
                              printDoc.Print();
                              placeRepository.StartNewSession(item.Id);
                              Refresh(placeRepository.SelectAll());
                          }
                      }
                      else if(new BookRepository().getBookPlacesById(item.Id).IsReally == 1)
                      {
                          new InfoView("Сессия ещё не закончена.").Show();
                          return;
                      }
                      
                  }));
            }
        }
        void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Arial", 20);
            graphics.DrawString("ООО\"Game Club\"", font, Brushes.Black, new PointF(50, 10));
            font = new Font("Arial", 20);
            graphics.DrawString("220108 г. Минск, ул. Казинца 91", font, Brushes.Black, new PointF(50, 40));
            font = new Font("Arial", 20);
            graphics.DrawString($"от: {DateTime.Now.ToString("d")}", font, Brushes.Black, new PointF(50, 70));; ;
            font = new Font("Arial", 20);
            graphics.DrawString("Товарный чек:", font, Brushes.Black, new PointF(50, 100));
            font = new Font("Arial", 13);
            int y = 130;
            int i = 1;
            foreach (var item in _payList)
            {
                string text = $"{i}. {item.Description} \t {item.Pay_Date} \t {item.Price} * {item.Count} (BYN) = {item.Result} (BYN)";
                graphics.DrawString(text, font, Brushes.Black, new PointF(10, y));
                i++;
                y += 20;
            }
            font = new Font("Arial", 18);
            graphics.DrawString($"Итого: {_payList.Sum(x=> x.Result)} (BYN)", font, Brushes.Black, new PointF(50, y));
        }
    }

}






