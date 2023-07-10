using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace GameClub.Helpers
{
    public static class BindCollumsInDataGrid
    {
        public static ICommand GetAutoGenerateColumnEvent(DataGrid grid) { return (ICommand)grid.GetValue(AutoGenerateColumnEventProperty); }
        public static void SetAutoGenerateColumnEvent(DataGrid grid, ICommand value) { grid.SetValue(AutoGenerateColumnEventProperty, value); }
        public static readonly DependencyProperty AutoGenerateColumnEventProperty =
            DependencyProperty.RegisterAttached("AutoGenerateColumnEvent", typeof(ICommand), typeof(BindCollumsInDataGrid), new UIPropertyMetadata(null, OnAutoGenerateColumnEventChanged));
        static void OnAutoGenerateColumnEventChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid grid = depObj as DataGrid;
            if (grid == null || e.NewValue is ICommand == false)
                return;
            ICommand command = (ICommand)e.NewValue;
            grid.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>((s, args) => OnAutoGeneratingColumn(command, s, args));
            // handle unsubscribe if needed
        }
        static void OnAutoGeneratingColumn(ICommand command, object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (command.CanExecute(e)) command.Execute(e);
        }
    }
}
