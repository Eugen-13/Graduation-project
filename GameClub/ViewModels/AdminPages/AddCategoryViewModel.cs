using GameClub.Helpers;
using GameClub.Repositories;
using GameClub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameClub.Views.Dialogs;

namespace GameClub.ViewModels.AdminPages.Places
{
    internal class AddCategoryViewModel : ViewModelBase
    {
        bool edit;
        public AddCategoryViewModel(CategoryModel category)
        {
            Category = category;
            categoryRepository = new CategoryRepository();
            edit = true;
        }
        CategoryRepository categoryRepository { get; set; }
        public CategoryModel Category { get; set; }
        public AddCategoryViewModel()
        {
            categoryRepository = new CategoryRepository();
            edit = false;
            Category = new CategoryModel();
        }
        private ViewModelCommand _okCommand;
        public ViewModelCommand OkCommand
        {
            get
            {
                return _okCommand ??
                  (_okCommand = new ViewModelCommand(obj =>
                  {
                      try
                      {
                          if (Category.Name.Length < 3)
                          {
                              new InfoView("Название категории должен быть от 3 символов.").Show();
                              return;
                          }
                          if (Category.Name.Length > 32)
                          {
                              new InfoView("Название категории должен быть меньше 32 символов.").Show();
                              return;
                          }
                          if (Category.Price <= 0)
                          {
                              new InfoView("Стоимость не может быть отрицательной или равной 0.").Show();
                              return;
                          }
                      }
                      catch (Exception)
                      {
                          new InfoView("Некорректно или непоностью заполнены данные.").Show();
                      }   
                      if (edit)
                          categoryRepository.EditCategory(Category);
                      else
                          categoryRepository.AddCategory(Category);
                      (obj as Window).Close();
                      LocalStorage.addItem("ok", "dialogResultAddorEditCategory");

                  }));
            }
        }
        private ViewModelCommand _cancelCommand;
        public ViewModelCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??
                  (_cancelCommand = new ViewModelCommand(obj =>
                  {
                      (obj as Window).Close();
                      LocalStorage.addItem("cancel", "dialogResultAddorEditCategory");
                  }));
            }
        }

        protected ObservableCollection<string> _categories;
        public ObservableCollection<string> Categoryes
        {
            get { return _categories; }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged("Categoryes");
                }
            }
        }
    }
}
