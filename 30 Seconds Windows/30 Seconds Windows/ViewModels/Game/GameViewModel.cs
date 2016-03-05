using _30_Seconds_Windows.Model.Utils;
using _30_Seconds_Windows.Pages;
using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace _30_Seconds_Windows.ViewModels.Game
{
    public abstract class GameViewModel : ViewModel
    {
        protected DispatcherTimer Timer = new DispatcherTimer();

        protected GameViewModel()
            : base()
        {
            
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
