using _30_Seconds_Windows.Model;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;

namespace _30_Seconds_Windows.ViewModels.Settings
{
    public class SettingsWordPacksPageViewModel : ViewModel
    {
        public static readonly SettingsWordPacksPageViewModel instance = new SettingsWordPacksPageViewModel();

        public WordPack[] WordPacks { get; private set; }
        public bool WordPacksChanged { get; set; }

        private SettingsWordPacksPageViewModel() : base()
        {

        }

        public async Task Load()
        {
            IsLoading = true;
            WordPacksChanged = false;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            WordPacks = WordPackHandler.instance.GetWordPacks()
                .OrderByDescending(wp => wp.IsDefault)
                .ThenBy(wp => wp.Free)
                .ThenBy(wp => wp.Name)
                .ToArray();

            NotifyPropertyChanged("WordPacks");
            IsLoading = false;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

            if (WordPacksChanged)
            {
                WordHandler.instance.DeleteAllWords();
                GameHandler.instance.SetAllGamesToFinished();
                WordPackHandler.instance.SaveWordPacks(WordPacks);
                SettingsHandler.instance.Update(true);
            }
        }
    }
}
