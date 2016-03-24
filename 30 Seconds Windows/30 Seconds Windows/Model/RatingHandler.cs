using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace _30_Seconds_Windows.Model
{
    public static class RatingHandler
    {
        private static bool hasBeenStarted = false;

        public static async Task AddArticleCount()
        {
            if (hasBeenStarted)
            {
                return;
            }

            hasBeenStarted = true;

            int Counter = GetCurrentCount() + 1;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["NumberOfTimesStarted"] = Counter;
            }
            catch
            {

            }

            if (Counter == 10)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await ShowRateDialog();
                });
               
            }
        }

        private static int GetCurrentCount()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                return (int)localSettings.Values["NumberOfTimesStarted"];
            }
            catch
            {
                return 0;
            }
        }

        private static async Task ShowRateDialog()
        {
            var messageDialog = new MessageDialog(Utils.Utils.ResourceLoader.GetString("ReviewPopupBody"), Utils.Utils.ResourceLoader.GetString("ReviewPopupTitle"));
            messageDialog.Commands.Add(
            new UICommand(Utils.Utils.ResourceLoader.GetString("Text_Review"), CommandInvokedHandler));
            messageDialog.Commands.Add(
            new UICommand(Utils.Utils.ResourceLoader.GetString("text_Cancel"), CommandInvokedHandler));
            await messageDialog.ShowAsync();
        }


        private static async void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == Utils.Utils.ResourceLoader.GetString("Text_Review"))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
            }
        }
    }
}
