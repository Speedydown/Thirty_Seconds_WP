using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class CategoryHandler : DataHandler
    {
        private static readonly Field NameField = new Field("Name", typeof(string), 50);
        public static readonly CategoryHandler instance = new CategoryHandler();

        private CategoryHandler()
            : base("Categories", new Field[] { NameField}, typeof(Category))
        {

        }

        public bool SaveCategory(Category category)
        {
            SettingsHandler.instance.CurrentSettings.CategoryLastUpdated = DateTime.Now;
            SettingsHandler.instance.SaveSettings();

            try
            {
                if (category.ID == 0)
                {
                    AddObject(category);
                }
                else
                {
                    UpdateObject(category);
                }

                return true;
            }
            catch (Exception e)
            {
                new WebsiteException(e, ErrorOrigin.Website, this.ToString());
                return false;
            }
        }

        public Category[] GetCategoryList(int Limit = 0)
        {
            return GetObjectList(Limit, OrderBy.ASC, NameField).Cast<Category>().ToArray();
        }

        public Category GetCategoryByName(string Name)
        {
            Category category = GetObjectByFieldsAndSearchQuery(new Field[] { NameField }, Name, true, 0).Cast<Category>().FirstOrDefault();

            if (category == null)
            {
                category = new Category(0, Name);
                SaveCategory(category);
            }

            return category;
        }

        public Category GetCategoryByID(int ID)
        {
            return GetObjectByID(ID) as Category;
        }
    }
}
