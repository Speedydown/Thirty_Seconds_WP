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
            AppConfig.Instance.EnableLogging = false;

            var Words = WordHandler.instance.GetWordsByLanguage(1);

            foreach (Word w in Words)
            {
                Console.WriteLine(w);
            }

            Console.WriteLine(Words.Count());
            Console.ReadLine();
        }

        public static void ReadCSV()
        {
            int LanguageID = 0;

            for (int i = 0; i < 20; i++)
            {
                List<string> Words = CSVReader.ReadCSV("Algemeen.csv", i);

                string CategoryName = Words.FirstOrDefault();
                Words.RemoveAt(0);

                //Add Category
                Category category = CategoryHandler.instance.GetCategoryByName(CategoryName);

                foreach (string s in Words)
                {
                    Word word = WordHandler.instance.GetWordByNameCategoryAndLanguage(s, category.ID, LanguageID);
                }
            }
        }
    }
}
