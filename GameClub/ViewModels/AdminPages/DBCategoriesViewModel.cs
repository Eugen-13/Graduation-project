using GameClub.Helpers;
using GameClub.Models;
using GameClub.Repositories;
using GameClub.ViewModels.AdminPages.Places;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameClub.ViewModels.AdminPages
{
    internal class DBCategoriesViewModel : ViewModelBaseDataGrid<CategoryModel>
    {
        public DBCategoriesViewModel()
        {
            categoryRepository = new CategoryRepository();
            Categories = new ObservableCollection<CategoryModel>(categoryRepository.SelectAll());
            if (Categories.Count > 0) 
                SelectedCategory = Categories.First();
            Fields = new ObservableCollection<string>(categoryRepository.GetFields("category"));
            SelectedValue = Fields.First();
        }

        protected string _selectedValue;
        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    OnPropertyChanged("SelectedValue");
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
                      dataGrid = obj as DataGrid;
                      Refresh(categoryRepository.SelectAll());
                     
                  }));
            }
        }

        protected override void Refresh(List<CategoryModel> models)
        {
            Categories = new ObservableCollection<CategoryModel>(models);
            dataGrid.ItemsSource = Categories;
            if (Categories.Count > 0)
                SelectedCategory = Categories.First();
        }

        protected string _search;
        public string Search
        {
            get { return _search; }
            set
            {
                if (_search != value)
                {
                    _search = value;
                    Find(_search);
                    OnPropertyChanged("Search");
                }
            }
        }

        private void Find(string search)
        {
            if (search == null || search == "")
                Refresh(categoryRepository.SelectAll());
            else
                Refresh(categoryRepository.SelectAllByOption(search, SelectedValue, false));
        }

        protected ObservableCollection<string> _fields;
        public ObservableCollection<string> Fields
        {
            get { return _fields; }
            set
            {
                if (_fields != value)
                {
                    _fields = value;
                    OnPropertyChanged("Fields");
                }
            }
        }

        private CategoryRepository categoryRepository;
        public CategoryModel categoryModel { get; set; }
        public ObservableCollection<CategoryModel> Categories { get; set; }
        private CategoryModel _selectedCategory;

        public CategoryModel SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
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
                      new Views.AdminPages.AddCategory().ShowDialog();
                      if (LocalStorage.getObjectByName("dialogResultAddorEditCategory") as string == "ok")
                      {
                          Refresh(categoryRepository.SelectAll());
                          LocalStorage.addItem("cancel", "dialogResultAddorEditCategory");
                      }
                  }));
            }
        }


        private ViewModelCommand _cancelCommand;

        private ViewModelCommand _deleteCommand;
        public ViewModelCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??
                  (_deleteCommand = new ViewModelCommand(obj =>
                  {
                      if ((new AcceptPage($"Вы  действительно хотите удалить категорию {SelectedCategory.Name}?")).ShowDialog() == true)
                      {
                          CategoryModel tempCategory = SelectedCategory;
                          if (Categories.Remove(SelectedCategory))
                              categoryRepository.DeleteCategory(tempCategory);
                      }
          
                  }));
            }
        }

        private ViewModelCommand _editCommand;
        public ViewModelCommand EditCommand
        {
            get
            {
                return _editCommand ??
                  (_editCommand = new ViewModelCommand(obj =>
                  {
                      var categoryView =  new Views.AdminPages.AddCategory();
                      categoryView.DataContext = new AddCategoryViewModel(SelectedCategory);
                      categoryView.ShowDialog();
                      if (LocalStorage.getObjectByName("dialogResultAddorEditCategory") as string == "ok")
                      {
                          Refresh(categoryRepository.SelectAll());
                          LocalStorage.addItem("cancel", "dialogResultAddorEditCategory");
                      }
                  }));
            }
        }
        ViewModelCommand _autoGeneratingColumnCommand;
        public ViewModelCommand AutoGeneratingColumnCommand
        {
            get
            {
                return _autoGeneratingColumnCommand ?? (_autoGeneratingColumnCommand = new ViewModelCommand(param =>
                {
                    var e = param as DataGridAutoGeneratingColumnEventArgs;
                    if (e != null)
                    {
                        if (e.PropertyName == "Id")
                            e.Column.Header = "Код";
                        if (e.PropertyName == "Name")
                            e.Column.Header = "Имя";
                        if (e.PropertyName == "Price")
                            e.Column.Header = "Цена (BYN)";
                    }
                },
                param => true));
            }
        }
    }
}




                      