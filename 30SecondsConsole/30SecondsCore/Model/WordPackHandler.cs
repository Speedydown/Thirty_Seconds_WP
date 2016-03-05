using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class WordPackHandler : DataHandler
    {
        private static readonly Field NameField = new Field("Name", typeof(string), 150);
        private static readonly Field LanguageIDField = new Field("LanguageID", typeof(int), 1);
        private static readonly Field FreeField = new Field("Free", typeof(bool), 1);
        private static readonly Field AddedField = new Field("Added", typeof(DateTime), 1);
        private static readonly Field ExpiredField = new Field("Expired", typeof(bool), 1);
        private static readonly Field IsDefaultField = new Field("IsDefault", typeof(bool), 1);

        public static readonly WordPackHandler instance = new WordPackHandler();

        private WordPackHandler() : base
            ("WordPacks", new Field[]
            {
                NameField,
                LanguageIDField,
                FreeField,
                AddedField,
                ExpiredField,
                IsDefaultField
            }, typeof(WordPack))
        {

        }

        public bool SaveWordPack(WordPack wordPack)
        {
            SettingsHandler.instance.CurrentSettings.WordPackLastUpdated = DateTime.Now;
            SettingsHandler.instance.SaveSettings();

            try
            {
                if (wordPack.ID == 0)
                {
                    AddObject(wordPack);
                }
                else
                {
                    UpdateObject(wordPack);
                }

                return true;
            }
            catch (Exception e)
            {
                new WebsiteException(e, ErrorOrigin.Website, this.ToString());
                return false;
            }
        }

        public WordPack[] GetWordPacks()
        {
            return base.GetObjectsByChildObjectID(ExpiredField, 0, 0).Cast<WordPack>().ToArray();
        }
    }
}
