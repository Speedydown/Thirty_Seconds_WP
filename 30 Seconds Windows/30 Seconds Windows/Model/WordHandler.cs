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
    public class WordHandler : DataHandler
    {
        public static readonly WordHandler instance = new WordHandler();

        //Properties
        public Random Randomizer { get; set; }
        public Word[] CurrentWords { get; private set; }

        private WordHandler() : base()
        {
            CreateItemTable<Word>();
            Randomizer = new Random();
        }

        public bool DatabaseHasWords()
        {
            return GetItems<Word>().Take(5).Count() == 5;
        }

        public async Task<Word[]> Get5Words(int CurrentPlayerID)
        {
            List<Word> words = GetItems<Word>()
                .Where(w => !w.Guessed)
                .OrderBy(w => w.NumberOfTimesTimesPlayed).ThenBy(w => w.LastPlayed).ThenBy(w => w.Name).ToList();

            if (words.Count == 0)
            {
                //Check if table has words
                if (DatabaseHasWords())
                {
                    ResetAllWords();
                    return await Get5Words(CurrentPlayerID);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Database empty, could not retrieve words");
                    return null;
                }
            }
            else if (words.Count < 10)
            {
                Task t = Task.Run(() => ResetAllWords());

                if (words.Count < 5)
                {
                    await t;
                    return await Get5Words(CurrentPlayerID);
                }
            }

            List<Word> CurrentWords = new List<Word>();

            for (int i = 0; i < 5; i++)
            {
                int Index = Randomizer.Next(0, words.Count() - 1);
                Word CurrentWord = words[Index];

                CurrentWord.NumberOfTimesTimesPlayed++;
                CurrentWord.Guessed = true;
                CurrentWord.LastPlayed = DateTime.Now;
                CurrentWord.LastPlayedByPlayerID = CurrentPlayerID;
                CurrentWord.CurrentGameCorrect = false;

                CurrentWords.Add(CurrentWord);
                words.Remove(CurrentWord);
            }

            SaveItems(CurrentWords);

            this.CurrentWords = CurrentWords.ToArray();

            return this.CurrentWords;
        }

        public void SaveWords(Word[] Words)
        {
            if (Words != null)
            {
                SaveItems(Words);
            }
        }

        private void ResetAllWords()
        {
            var words = GetItems<Word>().ToArray();

            foreach (Word w in words)
            {
                w.Guessed = false;
            }

            SaveItems(words);
        }

        public void DeleteAllWords()
        {
            ClearTable<Word>();
        }

        public async Task Update()
        {
            try
            {
                DeleteAllWords();

                foreach (WordPack wp in WordPackHandler.instance.GetEnabledWordPacks())
                {
                    Word[] Words = JsonConvert.DeserializeObject<Word[]>(await HTTPGetUtil.GetDataAsStringFromURL(Constants.ServerAddress + "ThirtySeconds/GetWordsByWordPackID/" + wp.ID));

                    foreach (Word w in Words)
                    {
                        w.Retrieved = DateTime.Now;
                    }

                    if (Words.Count() > 0)
                    {
                        SaveItems(Words);
                    }
                }
            }
            catch (Exception e)
            {
                await ExceptionHandler.instance.PostException(new AppException(e, (int)ClientIDHandler.AppName._30Seconds));
                return;
            }
        }
    }
}
