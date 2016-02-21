using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class Word : DataObject
    {
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public int LanguageID { get; set; }

        public Word(int ID, string Name, Category category, Languages Language)
            : this(ID, Name, category.ID, (int)Language)
        {

        }

        public Word(int ID, string Name, int CategoryID, int LanguageID) : base(ID)
        {
            this.Name = Name;
            this.CategoryID = CategoryID;
            this.LanguageID = LanguageID;

            if (ID == 0 && Name.Length > 3)
            {
                WordHandler.instance.SaveWord(this);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
