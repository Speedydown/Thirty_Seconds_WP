using _30_Seconds_Windows.Model.Utils;
using _30_Seconds_Windows.Pages;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public abstract class GameViewModel : ViewModel
    {
        protected MediaElement MediaPlayer = new MediaElement();

        //SoundFiles
        protected static Task LoadFileTask = null;
        protected static StorageFile StopwatchFile = null;
        protected static StorageFile AlarmFile = null;
        protected static StorageFile CheerFile = null;
        protected static StorageFile AwwFile = null;
        protected static StorageFile VictoryFile = null;

        protected static MemoryStream StopwatchStream = new MemoryStream();
        protected static MemoryStream AlarmStream = new MemoryStream();
        protected static MemoryStream CheerStream = new MemoryStream();
        protected static MemoryStream AwwStream = new MemoryStream();
        protected static MemoryStream VictoryStream = new MemoryStream();

        private object locker = new object();

        protected GameViewModel()
            : base()
        {
            lock (locker)
            {
                if (StopwatchFile == null || AlarmFile == null)
                {
                    LoadFileTask = Task.Run(async () =>
                {
                    StorageFolder folder = await (await Package.Current.InstalledLocation.GetFolderAsync("Assets")).GetFolderAsync("Sounds");
                    StopwatchFile = await folder.GetFileAsync("Stopwatch.wav");
                    AlarmFile = await folder.GetFileAsync("Alarm.wav");
                    CheerFile = await folder.GetFileAsync("Cheer.wav");
                    AwwFile = await folder.GetFileAsync("Aww.wav");
                    VictoryFile = await folder.GetFileAsync("Victory.wav");

                    (await StopwatchFile.OpenAsync(FileAccessMode.Read)).AsStream().CopyTo(StopwatchStream);
                    (await AlarmFile.OpenAsync(FileAccessMode.Read)).AsStream().CopyTo(AlarmStream);
                    (await CheerFile.OpenAsync(FileAccessMode.Read)).AsStream().CopyTo(CheerStream);
                    (await AwwFile.OpenAsync(FileAccessMode.Read)).AsStream().CopyTo(AwwStream);
                    (await VictoryFile.OpenAsync(FileAccessMode.Read)).AsStream().CopyTo(VictoryStream);

                });
                }
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                Task t = BackButtonPressed();
            }
        }

        public void NavigatedTo()
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        public virtual void NavigatedFrom()
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private async Task BackButtonPressed()
        {
            ////Create warning dialog:
            var messageDialog = new MessageDialog(Utils.ResourceLoader.GetString("CancelGame_Text"), Utils.ResourceLoader.GetString("CancelGame_Header"));

            messageDialog.Commands.Add(
                new UICommand(
                    Utils.ResourceLoader.GetString("text_MainMenu"),
                    null,
                    0));
            messageDialog.Commands.Add(
                 new UICommand(
                    Utils.ResourceLoader.GetString("text_Cancel"),
                    null,
                    1));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            IUICommand Command = await messageDialog.ShowAsync();

            if ((int)Command.Id == 0)
            {
                (Window.Current.Content as Frame).Navigate(typeof(MainPage));
                await ClearBackstack(0);
            }
        }
    }


}
