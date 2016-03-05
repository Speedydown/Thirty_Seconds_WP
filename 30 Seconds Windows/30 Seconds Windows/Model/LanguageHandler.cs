using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.Model
{
    public class LanguageHandler
    {
        public static readonly LanguageHandler instance = new LanguageHandler();

        public Language CurrentLanguage { get; set; }
        public Language[] Languages { get; set; }

        private LanguageHandler()
        {
            ResetLanguages();
        }

        public void ResetLanguages()
        {
            int DutchPacks = WordPackHandler.instance.GetWordsPacksByLanguageID(1).Count();
            int EnglishPacks = WordPackHandler.instance.GetWordsPacksByLanguageID(2).Count();
            int GermanPacks = WordPackHandler.instance.GetWordsPacksByLanguageID(3).Count();

            Languages = new Language[]
            {
                new Language() { LanguageID = 1, Name = "Nederlands", NumberOfWordPacks= DutchPacks },
                new Language() { LanguageID = 2, Name = "English", NumberOfWordPacks= EnglishPacks },
                new Language() { LanguageID = 3, Name = "Deutch", NumberOfWordPacks= GermanPacks },
            };

            Languages = Languages.Where(l => l.NumberOfWordPacks > 0).ToArray();

            CurrentLanguage = Languages.SingleOrDefault(l => l.LanguageID == SettingsHandler.instance.CurrentSettings.CurrentLanguageID);

            if (CurrentLanguage != null)
            {
                CurrentLanguage.Enabled = true;
            }
        }

        public void ChangeLanguage(int LanguageID)
        {
            SettingsHandler.instance.CurrentSettings.CurrentLanguageID = LanguageID;
            SettingsHandler.instance.Update(true);
            SettingsHandler.instance.SaveSettings();

            CurrentLanguage.Enabled = false;
            CurrentLanguage = Languages.Single(l => l.LanguageID == SettingsHandler.instance.CurrentSettings.CurrentLanguageID);
            CurrentLanguage.Enabled = true;
        }
    }
}
