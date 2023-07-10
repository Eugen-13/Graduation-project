using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameClub.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Пароль { get; set; } = "********";
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        private BitmapImage _image = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Images/Avatar.png", UriKind.Absolute));
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
