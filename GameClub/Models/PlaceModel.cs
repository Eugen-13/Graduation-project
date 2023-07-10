using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameClub.Models
{
    public class PlaceModel
    {
        public PlaceModel() { }
        public int Id { get; set; }
        public string? Performance { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public string Condition { get; set; }
        public string? timeLeft { get; set; } = "";

        private BitmapImage _image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/pc.png", UriKind.Absolute));
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

    }
}
