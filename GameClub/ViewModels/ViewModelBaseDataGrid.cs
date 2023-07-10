using GameClub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameClub.ViewModels
{
    public abstract class ViewModelBaseDataGrid<T> : ViewModelBase
    {
        protected DataGrid dataGrid;

        protected ViewModelCommand _loadedCommand;
       protected abstract void Refresh(List<T> models);
    }
}
