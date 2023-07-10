using GameClub.Models;
using GameClub.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameClub.Repositories
{
    internal class CategoryRepository : RepositoryBase, ISelectable<CategoryModel>
    {
        public List<CategoryModel> SelectAll()
        {
            List<CategoryModel> categories = new List<CategoryModel>();
            using (var command = GetCommand($"select * from category"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                Name = reader[1].ToString(),
                                Price = double.Parse(reader[2].ToString())
                            });
                        }
                    }
                }
            }
            return categories;
        }

        public List<CategoryModel> SelectAllByOption(string findText, string findField, bool isAdminSelect)
        {
            List<CategoryModel> categories = new List<CategoryModel>();
            using (var command = GetCommand($"select * from category where {findField} Like '%{findText}%';"))
            {
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            categories.Add(new CategoryModel()
                            {

                                Id = int.Parse(reader[0].ToString()),
                                Name = reader[1].ToString(),
                                Price = double.Parse(reader[2].ToString())
                            });
                        }
                    }
                }
            }
            return categories;
        }

        public CategoryModel getCategoryById(CategoryModel categoryId)
        {
            var items = SelectAllByOption(categoryId.ToString(), "Id", false);
            if (items.Count > 0)
                return items.First();
            return null;
        }
        public void DeleteCategory(CategoryModel category)
        {
            ExecuteNonQuery($"delete FROM map WHERE place_id IN ( SELECT id FROM place WHERE category_id = '{category.Id}' );");
            ExecuteNonQuery($"delete from category where id='{category.Id}';");
        }
        public void AddCategory(CategoryModel category)
        {
            if(category.Price < 0)
            {
                new InfoView("Цена не может быть ниже 0").Show();
                return;
            }
            ExecuteNonQuery($"insert into category( id, name, price) values ('{getMaxID("category")}', '{category.Name}', '{category.Price}');");
        }
        public void EditCategory(CategoryModel category)
        {
            if (category.Price < 0)
            {
                new InfoView("Цена не может быть ниже 0").Show();
                return;
            }
            ExecuteNonQuery($"update category set name = '{category.Name}', price = '{category.Price}' where id = '{category.Id}';");
        }
    }
}

