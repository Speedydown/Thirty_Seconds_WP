using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public class EndGamePageViewModel : ViewModel
    {
        public static readonly EndGamePageViewModel instance = new EndGamePageViewModel();

        public Model.Game CurrentGame { get; set; }

        public Team[] TeamsSortedByScore
        {
            get
            {
                return CurrentGame.Teams.OrderByDescending(t => t.Points).ToArray();
            }
        }

        private EndGamePageViewModel()
        {

        }

        public async Task NavigatedFrom()
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            await ClearBackstack(0);
        }

        public async Task Load()
        {
            IsLoading = true;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            IsLoading = false;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));

        }
        
        public void EndGamePageContinueButton_Pressed()
        {
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }
    }
}
