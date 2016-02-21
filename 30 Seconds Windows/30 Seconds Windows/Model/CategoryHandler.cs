using _30_Seconds_Windows.Model.Utils;
using BaseLogic;
using BaseLogic.DataHandler;
using BaseLogic.ExceptionHandler;
using BaseLogic.HtmlUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class CategoryHandler : DataHandler
    {
        public static readonly CategoryHandler instance = new CategoryHandler();

        private List<Category> Categories = null;

        private CategoryHandler()
            : base()
        {
            CreateItemTable<Category>();
            Categories = GetItems<Category>().ToList();
        }

        public Category GetCategoryByID(int ID)
        {
            return Categories.SingleOrDefault(c => c.InternalID == ID);
        }

        public async Task Update()
        {
            try
            {
                Category[] Categories = JsonConvert.DeserializeObject<Category[]>(await HTTPGetUtil.GetDataAsStringFromURL(Constants.ServerAddress + "ThirtySeconds/GetCategories"));

                if (Categories.Count() > 0)
                {
                    ClearTable<Category>();
                    SaveItems(Categories);
                    this.Categories = Categories.ToList();
                }
            }
            catch(Exception e)
            {
                new AppException(e);
                return;
            }
        }
    }
}
