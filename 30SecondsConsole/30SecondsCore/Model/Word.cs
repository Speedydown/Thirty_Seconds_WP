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
        public int WordPackID { get; set; }
        public int CategoryID { get; set; }

        public Word(int ID, string Name, Category category, WordPack wordPack)
            : this(ID, Name, category.ID, wordPack.ID)
        {

        }

        public Word(int ID, string Name, int CategoryID, int WordPackID)
            : base(ID)
        {
            this.Name = Name;
            this.CategoryID = CategoryID;
            this.WordPackID = WordPackID;

            if (ID == 0 && Name.Length > 0)
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
