using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//                                
namespace GameClub.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ViewModelCommand _closeCommand;
        public ViewModelCommand CloseCommand
        {
            get
            {
                return _closeCommand ??
                  (_closeCommand = new ViewModelCommand(obj =>
                  {
                      foreach (Window window in Application.Current.Windows)
                      {
                          window.Close();
                      }
                  }));
            }
        }
    }
}
