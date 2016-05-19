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
        private static readonly Field WordPackIDField = new Field("WordPackID", typeof(int), 100);

        public static readonly WordHandler instance = new WordHandler();

        private WordHandler()
            : base("Words", new Field[]
            {
                NameField,
                CategoryIDField,
                WordPackIDField
            }, typeof(Word))
        {
        }

        public bool SaveWord(Word word)
        {
            SettingsHandler.instance.CurrentSettings.WordLastUpdated = TimeConverter.GetDateTime();
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

        public Word GetWordByNameCategoryAndWordPackID(string Name, int CategoryID, int WordPackID)
        {
            Filter[] Filters = new Filter[]
            {
                new Filter(FilterType.And, new Field[]
                    {
                        NameField, 
                        CategoryIDField,
                        WordPackIDField
                    },
                    new string[]
                    {
                        "Name",
                        "CategoryID",
                        "WordPackID"
                    },
                    new object[]
                    {
                        Name,
                        CategoryID,
                        WordPackID
                    })
            };

            var Words = GetObjectsByFilters(Filters);
            Word word = Words.Cast<Word>().ToList().FirstOrDefault();

            if (word == null)
            {
                word = new Word(0, Name, CategoryID, WordPackID);
            }

            return word;
        }

        public Word[] GetWordsByWordPack(int WordPackID)
        {
            Filter[] Filters = new Filter[]
            {
                new Filter(FilterType.Or, new Field[]
                    {
                        WordPackIDField,
                    },
                    new string[]
                    {
                        "WordPackID",
                    },
                    new object[]
                    {
                        WordPackID
                    })
            };

            return GetObjectsByFilters(Filters, 0, OrderBy.ASC, NameField).Cast<Word>().ToArray();
        }

        public void DeleteWord(Word word)
        {
            DeleteObject(word);
        }
    }
}
