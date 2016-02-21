﻿using _30_Seconds_Windows.Model.Utils;
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
    public class WordHandler : DataHandler
    {
        public static readonly WordHandler instance = new WordHandler();

        //Properties
        public Random Randomizer { get; set; }

        private WordHandler() : base()
        {
            CreateItemTable<Word>();
        }

        public async Task<Word[]> Get5Words()
        {
            List<Word> words = GetItems<Word>()
                .Where(w => !w.Guessed && 
                (w.LanguageID == SettingsHandler.instance.CurrentSettings.CurrentLanguageID || w.LanguageID == 0))
                .OrderBy(w => w.NumberOfTimesTimesPlayed).ThenBy(w => w.LastPlayed).ThenBy(w => w.Name).Take(100).ToList();

            if (words.Count == 0)
            {
                if (GetItems<Word>().Where(w => (w.LanguageID == SettingsHandler.instance.CurrentSettings.CurrentLanguageID || w.LanguageID == 0)).Take(5).Count() == 5)
                {
                    ResetAllWords();
                    return await Get5Words();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Database empty, could not retrieve words");
                    //TODO dialoog;
                    return null;
                }
            }
            else if (words.Count < 10)
            {
                Task t = Task.Run(() => ResetAllWords());
                return await Get5Words();
            }

            List<Word> CurrentWords = new List<Word>();

            for (int i = 0; i < 5; i++)
            {
                int Index = Randomizer.Next(0, words.Count() - 1);
                Word CurrentWord = words[Index];

                CurrentWord.NumberOfTimesTimesPlayed++;
                CurrentWord.Guessed = true;
                CurrentWord.LastPlayed = DateTime.Now;

                SaveItem(CurrentWord);

                CurrentWords.Add(CurrentWord);
                words.Remove(CurrentWord);
            }

            return CurrentWords.ToArray();
        }

        private void ResetAllWords()
        {
            var words = GetItems<Word>()
                .Where(w => w.LanguageID == SettingsHandler.instance.CurrentSettings.CurrentLanguageID || w.LanguageID == 0);

            foreach (Word w in words)
            {
                w.Guessed = false;
            }

            SaveItems(words);
        }

        public async Task Update()
        {
            try
            {
                Word[] Words = JsonConvert.DeserializeObject<Word[]>(await HTTPGetUtil.GetDataAsStringFromURL(Constants.ServerAddress + "ThirtySeconds/GetWordsByLanguage/" + SettingsHandler.instance.CurrentSettings.CurrentLanguageID));

                if (Words.Count() > 0)
                {
                    ClearTable<Word>();
                    SaveItems(Words);
                }
            }
            catch (Exception e)
            {
                new AppException(e);
                return;
            }
        }
    }
}
