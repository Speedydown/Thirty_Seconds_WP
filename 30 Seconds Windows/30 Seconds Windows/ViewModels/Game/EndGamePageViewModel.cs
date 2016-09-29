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
            await ClearBackStack(0);
        }

        public async Task LoadData()
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Navigate(typeof(MainPage));

        }
        
        public async void EndGamePageContinueButton_Pressed()
        {
           await Navigate(typeof(MainPage));
        }

        public override void Unload()
        {
            
        }

        public async override Task Load()
        {
            
        }
    }
}
