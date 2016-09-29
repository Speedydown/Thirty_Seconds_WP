using _30_Seconds_Windows.Model.Utils;
using BaseLogic.ClientIDHandler;
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
    public class WordPackHandler : DataHandler
    {
        public static readonly WordPackHandler instance = new WordPackHandler();
     
        private List<WordPack> WordPacks = null;

        private WordPackHandler()
            : base()
        {
            CreateItemTable<WordPack>();
            WordPacks = GetItems<WordPack>().ToList();
        }

        public WordPack[] GetWordsPacksByLanguageID(int LanguageID)
        {
            return WordPacks.Where(wp => wp.LanguageID == 0 || wp.LanguageID == LanguageID).OrderBy(wp => wp.LanguageID).ThenBy(wp => wp.Name).ToArray();
        }

        public WordPack GetWordPackByID(int ID)
        {
            return WordPacks.SingleOrDefault(wp => wp.InternalID == ID);
        }

        public WordPack[] GetEnabledWordPacks()
        {
            return GetWordsPacksByLanguageID(SettingsHandler.instance.CurrentSettings.CurrentLanguageID).Where(wp => wp.Enabled).ToArray();
        }

        public WordPack[] GetWordPacks()
        {
            return GetWordsPacksByLanguageID(SettingsHandler.instance.CurrentSettings.CurrentLanguageID).ToArray();
        }

        public void SaveWordPacks(WordPack[] WordPacks)
        {
            if (WordPacks != null)
            {
                SaveItems(WordPacks);
            }
        }

        public async Task Update()
        {
            try
            {
                bool IsUpdate = this.WordPacks.Count(wp => wp.LanguageID == SettingsHandler.instance.CurrentSettings.CurrentLanguageID) > 0;
                WordPack[] EnabledWordPacks = null;

                if (IsUpdate)
                {
                    EnabledWordPacks = this.WordPacks.Where(wp => wp.Enabled).ToArray();
                }

                WordPack[] WordPacks = JsonConvert.DeserializeObject<WordPack[]>(await HTTPGetUtil.GetDataAsStringFromURL(Constants.ServerAddress + "ThirtySeconds/GetWordPacks"));

                
                    foreach (WordPack w in WordPacks)
                    {
                        if (IsUpdate)
                        {
                            if (EnabledWordPacks.Count(wp => wp.Enabled && wp.ID == w.ID) > 0)
                            {
                                w.Enabled = true;
                            }
                        }
                        else if (w.IsDefault)
                        {
                            w.Enabled = true;
                        }
                    }
                
                if (WordPacks.Count() > 0)
                {
                    ClearTable<WordPack>();
                    SaveItems(WordPacks);
                    this.WordPacks = WordPacks.ToList();

                    //Update languagesettings
                    LanguageHandler.instance.ResetLanguages();
                }
            }
            catch(Exception e)
            {
                await ExceptionHandler.instance.PostException(new AppException(e, (int)ClientIDHandler.AppName._30Seconds));
                return;
            }
        }

    }
}
