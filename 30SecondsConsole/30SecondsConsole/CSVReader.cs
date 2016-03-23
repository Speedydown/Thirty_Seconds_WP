using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30SecondsConsole
{
    class CSVReader
    {
        public static List<string> ReadCSV(string CSVPath, int Col)
        {
            List<string> Words = new List<string>();

            using (StreamReader sr = new StreamReader(CSVPath, true))
            {
                String line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');

                    try
                    {
                        Words.Add(parts[Col]);
                    }
                    catch
                    {
                        break;
                    }

                }
            }

            return Words;
        }

    }
}
