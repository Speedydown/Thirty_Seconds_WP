using _30SecondsCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
           // var WordPacks = WordPackHandler.instance.GetWordPacks().OrderBy(wp => wp.LanguageID).ThenBy(wp => wp.ID);

         //   var WordPack = new WordPack(0, "Zomer", 1, true, DateTime.Now, false, false);

            int WordPackID = 7;
       //     WordPackID = WordPack.ID;
            AppConfig.Instance.EnableLogging = false;

            WordPack wordPack = WordPackHandler.instance.GetObjectByID(WordPackID) as WordPack;
            
            ReadCSV(WordPackID);

            var Words = WordHandler.instance.GetWordsByWordPack(WordPackID);

            foreach (Word w in Words)
            {
                Console.WriteLine(w);
            }

            Console.WriteLine(Words.Count());
            Console.ReadLine();
        }

        public static void ReadCSV(int WordPackID)
        {
            for (int i = 0; i < 30; i++)
            {

                List<string> Words = CSVReader.ReadCSV("Zomer.csv", i);

                string CategoryName = Words.FirstOrDefault();

                try
                {
                    Words.RemoveAt(0);
                }
                catch
                {
                    return;
                }

                //Add Category
                Category category = CategoryHandler.instance.GetCategoryByName(CategoryName);

                foreach (string s in Words)
                {
                    Word word = WordHandler.instance.GetWordByNameCategoryAndWordPackID(s, category.ID, WordPackID);
                }
            }
        }
    }
}
