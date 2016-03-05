using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.Pages.Game;
using _30_Seconds_Windows.Pages.GameSetup;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.GameSetup
{
    public class SplashPageViewModel : ViewModel
    {
        public static readonly SplashPageViewModel instance = new SplashPageViewModel();

        private SplashPageViewModel()
            : base()
        {
         
        }

        public async Task LoadData()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = true;
                    StatusBar.GetForCurrentView().ForegroundColor = Color.FromArgb(255, 0, 173, 163);
                });

            Task NewGameTask = Task.Run(() =>
                {
                    GameHandler.instance.StartNewGame();
                });

            Task[] Tasks = { NewGameTask, SettingsHandler.instance.WaitForUpdate(), Task.Delay(0) };

            Task.WaitAll(Tasks);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                (Window.Current.Content as Frame).Navigate(typeof(TeamsPage));
                (Window.Current.Content as Frame).BackStack.Remove((Window.Current.Content as Frame).BackStack.Last());
                IsLoading = false;
            });
        }
    }
}
