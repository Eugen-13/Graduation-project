using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameClub.Models
{
    public class PurchaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public string IsAble
        {
            get 
            {
                if (Count > 0)
                    return "В наличии";
                else
                    return "Нет на складе";
             }
        }
        private BitmapImage _image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/purchase.png", UriKind.Absolute));
        public BitmapImage? Image
        {
            get
            {
                return _image;
            }
            set
            {
                this._image = value;
            }
        }
        private int _count;
        public int CurrentCount
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("CurrentCount");

            }
        }
    }
}
