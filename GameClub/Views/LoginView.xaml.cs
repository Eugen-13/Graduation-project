using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameClub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private bool mIsDragging;
        private Point mStartPoint;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mIsDragging = true;
            mStartPoint = e.GetPosition(this);
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mIsDragging = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mIsDragging)
            {
                Point offset = Point.Subtract(e.GetPosition(this), new Vector(mStartPoint.X, mStartPoint.Y));

                Left += offset.X;
                Top += offset.Y;
            }
        }
        public LoginView()
        {
            InitializeComponent();
        }
    }
}
