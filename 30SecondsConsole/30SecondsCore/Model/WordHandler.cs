using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class WordHandler : DataHandler
    {
        private static readonly Field NameField = new Field("Name", typeof(string), 100);
        private static readonly Field CategoryIDField = new Field("CategoryID", typeof(int), 1);
        private static readonly Field LanguageIDField = new Field("LanguageID", typeof(int), 100);

        public static readonly WordHandler instance = new WordHandler();

        private WordHandler()
            : base("Words", new Field[]
            {
                NameField,
                CategoryIDField,
                LanguageIDField
            }, typeof(Word))
        {
        }

        public bool SaveWord(Word word)
        {
            SettingsHandler.instance.CurrentSettings.WordLastUpdated = DateTime.Now;
            SettingsHandler.instance.SaveSettings();

            try
            {
                if (word.ID == 0)
                {
                    AddObject(word);
                }
                else
                {
                    UpdateObject(word);
                }

                return true;
            }
            catch (Exception e)
            {
                new WebsiteException(e, ErrorOrigin.Website, this.ToString());
                return false;
            }
        }

        public Word GetWordByNameCategoryAndLanguage(string Name, int CategoryID, int LanguageID)
        {
            Filter[] Filters = new Filter[]
            {
                new Filter(FilterType.And, new Field[]
                    {
                        NameField, 
                        CategoryIDField,
                        LanguageIDField
                    },
                    new string[]
                    {
                        "Name",
                        "CategoryID",
                        "LanguageID"
                    },
                    new object[]
                    {
                        Name,
                        CategoryID,
                        LanguageID
                    })
            };

            var Words = GetObjectsByFilters(Filters);
            Word word = Words.Cast<Word>().ToList().FirstOrDefault();

            if (word == null)
            {
                word = new Word(0, Name, CategoryID, LanguageID);
            }

            return word;
        }

        public Word[] GetWordsByLanguage(int LanguageID)
        {
            Filter[] Filters = new Filter[]
            {
                new Filter(FilterType.Or, new Field[]
                    {
                        LanguageIDField,
                        LanguageIDField
                    },
                    new string[]
                    {
                        "Language1",
                        "language2"
                    },
                    new object[]
                    {
                        0,
                        LanguageID
                    })
            };

            return GetObjectsByFilters(Filters, 0, OrderBy.ASC, NameField).Cast<Word>().ToArray();
        }
    }
}
