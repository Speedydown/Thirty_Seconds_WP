using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class WordPack : DataObject
    {
        public string Name { get; set; }
        public int LanguageID { get; set; }
        public bool Free { get; set; }
        public DateTime Added { get; set; }
        public bool Expired { get; set; }
        public bool IsDefault { get; set; }
        public int WordCount
        {
            get
            {
                return WordHandler.instance.GetWordsByWordPack(ID).Count();
            }
        }

        public WordPack(int ID, string Name, int LanguageID, bool Free, DateTime Added, bool Expired, bool IsDefault)
            : base(ID)
        {
            this.Name = Name;
            this.LanguageID = LanguageID;
            this.Free = Free;
            this.Added = Added;
            this.Expired = Expired;
            this.IsDefault = IsDefault;

            if (ID == 0 && Name.Length > 3)
            {
                Added = DateTime.Now;
                WordPackHandler.instance.SaveWordPack(this);
            }
        }
    }
}
