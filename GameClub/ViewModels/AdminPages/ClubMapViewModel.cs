using GameClub.Models;
using GameClub.Repositories;
using GameClub.Views.AdminPages;
using GameClub.Views.Dialogs;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static GameClub.ViewModels.AdminPages.PaymentViewModel;

namespace GameClub.ViewModels.AdminPages
{
    internal class ClubMapViewModel : ViewModelBase
    {
        private Grid _myGrid;
        public Grid MyGrid
        {
            get { return _myGrid; }
            set { _myGrid = value; OnPropertyChanged(nameof(MyGrid)); }
        }

        MapRepository _mapRepository;
        public ClubMapViewModel()
        {
            _mapRepository = new MapRepository();
            map = _mapRepository.SelectAll();
            InitializeGrid();

        }

        List<MapModel> map;
        int cofX = 160;
        int cofY = 60;

        public void InitializeGrid()
        {
            _myGrid = new Grid(); //700 - 350
            _myGrid.Width = cofX * 10;
            _myGrid.Height = cofY * 10; ;
            _myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            _myGrid.VerticalAlignment = VerticalAlignment.Top;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int x = j;
                    int y = i ;

                    TextBlock textBlock = new TextBlock();
                    if (map.Find(m => m.PosX == x && m.PosY == y) != null)
                    {
                        if (map.Find(m => m.PosX == x && m.PosY == y).IsPlace == 0)
                        {
                            textBlock.Text = map.Find(m => m.PosX == x && m.PosY == y).Text;
                        }
                        else
                        {
                            PlaceModel place = new PlaceRepository().getPlaceById(map.Find(m => m.PosX == x && m.PosY == y).PlaceId);
                            Run run = new Run("●  ");
                            if(place.Condition == "Свободно")
                                run.Foreground = Brushes.Green;
                            else if(place.Condition == "Занято")
                                run.Foreground = Brushes.Orange;
                            else
                                run.Foreground = Brushes.Red;
                            textBlock.Inlines.Add(run);
                            Run greenRun = new Run(place.Description);
                            greenRun.Foreground = Brushes.White;
                            textBlock.Inlines.Add(greenRun);
                        }
                        textBlock.Name = "t" + map.Find(m => m.PosX == x && m.PosY == y).Id.ToString();
                    }
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.Foreground = Brushes.White;
                    textBlock.FontSize = 18;
                    textBlock.TextWrapping = TextWrapping.Wrap;


                    Border border = new Border();
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.Width = cofX;
                    border.Height = cofY;
                    border.Margin = new Thickness(x * cofX - 2, y * cofY - 2, 0, 0);
                    border.MouseEnter += Border_MouseEnter;
                    border.MouseLeave += Border_MouseLeave;
                    border.MouseDown += Border_MouseDown;
                    border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF121212"));
                    Thickness thickness = new Thickness(1, 1, 1, 1);
                    if (y == 0)
                        thickness.Top = 0;
                    if (y == 9)
                        thickness.Bottom = 0;
                    if (x == 0)
                        thickness.Left = 0;
                    if (x == 9)
                        thickness.Right = 0;
                    border.BorderThickness = thickness;
                    border.BorderBrush = Brushes.Black;
                    border.Child = textBlock;
                    _myGrid.Children.Add(border);
                }
            }
        }

        private ContentControl contentControl;
        private ViewModelCommand _loadedCommand;
        public ViewModelCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??
                  (_loadedCommand = new ViewModelCommand(obj =>
                  {
                      contentControl = (obj as ContentControl);
                      Refresh();

                  }));
            }
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool isNew = false;
            var view = new EditMapView();
            var textBlock = ((sender as Border).Child as TextBlock);
            if (_current != null)
                _currentViewModel = new EditMapViewModel(_current, false, map.Select(item => item.PlaceId).ToList());
            else
            {
                _currentViewModel = new EditMapViewModel(new MapModel()
                {
                    IsPlace = 0,
                    PosX = (((int)(sender as Border).Margin.Left) + 2) / cofX,
                    PosY = (((int)(sender as Border).Margin.Top) + 2) / cofY
                }, true, map.Select(item => item.PlaceId).ToList());
                isNew = true;
            }
            view.DataContext = _currentViewModel; 
            view.ShowDialog();
            _current = ((EditMapViewModel)_currentViewModel).Model;
            if (_current.IsPlace == -13)
            {
                map.RemoveAt(map.FindIndex(x=> x.Id == _current.Id));
                textBlock.Name = "";
                textBlock.Text = "";
                _mapRepository.DeletePoint(_current);
                _current = new MapModel();
                return;
            }
            if (isNew)
            {
                _current.Id = map.Max(x => x.Id) + 1;  
                map.Add(_current);
                _mapRepository.AddPoint(_current);
                textBlock.Name = "t" + _current.Id.ToString();

            }
            else
                _mapRepository.Update(_current);
            textBlock.Text = "";
            if (_current.IsPlace == 0)
            {
                textBlock.Text = _current.Text;
            }
            else
            {
                PlaceModel place = new PlaceRepository().getPlaceById(_current.PlaceId);
                Run run = new Run("●  ");
                if (place.Condition == "Свободно")
                    run.Foreground = Brushes.Green;
                else if (place.Condition == "Занято")
                    run.Foreground = Brushes.Orange;
                else
                    run.Foreground = Brushes.Red;
                textBlock.Inlines.Add(run);
                Run greenRun = new Run(place.Description);
                greenRun.Foreground = Brushes.White;
                textBlock.Inlines.Add(greenRun);
            }
            _current = null;

        }
        string _currentSelectText;
        MapModel _current;
        object _currentViewModel;
        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF121212"));
            var textBlock = (sender as Border).Child as TextBlock;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.FontSize = 18;
            textBlock.Text = "";
            if(_current == null)
            {
                textBlock.Text = "";
                return;
            }
            if (_current.IsPlace == 0)
            {
                textBlock.Text = _current.Text;
            }
            else
            {
                PlaceModel place = new PlaceRepository().getPlaceById(_current.PlaceId);
                Run run = new Run("●  ");
                if (place.Condition == "Свободно")
                    run.Foreground = Brushes.Green;
                else if (place.Condition == "Занято")
                    run.Foreground = Brushes.Orange;
                else
                    run.Foreground = Brushes.Red;
                textBlock.Inlines.Add(run);
                Run greenRun = new Run(place.Description);
                greenRun.Foreground = Brushes.White;
                textBlock.Inlines.Add(greenRun);
            }
            _current = null;
        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            (sender as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4C900000"));
            var textBlock = (sender as Border).Child as TextBlock;
            textBlock.Foreground = new SolidColorBrush(Colors.Red);
            if (textBlock.Name != "" && textBlock.Name != null) 
                _current = map.Find(x => x.Id == int.Parse(textBlock.Name.Remove(0, 1)));
            else
                _current = null;
            textBlock.Text = "+";
            textBlock.FontSize = 35;
        }
        void Refresh()
        {
            map = _mapRepository.SelectAll();
            MyGrid.Children.Clear();
            InitializeGrid();
            contentControl.Content = MyGrid;
            OnPropertyChanged(nameof(MyGrid));
        } 
    }
}
