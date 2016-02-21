using _30_Seconds_Windows.Common;
using _30_Seconds_Windows.Model;
using _30_Seconds_Windows.ViewModels.GameSetup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace _30_Seconds_Windows.Pages.GameSetup
{
    public sealed partial class PlayersPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PlayersPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            DataContext = PlayersPageViewModel.instance;
            await PlayersPageViewModel.instance.LoadData();
        }

        private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            await PlayersPageViewModel.instance.SaveTeam();
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void PlayerGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                await PlayersPageViewModel.instance.DeletePlayerButton((sender as Grid).DataContext as Player);
            }
        }

        private void PlayersPageAddPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            PlayersPageViewModel.instance.AddNewPlayerButton();
        }

        private void PreviousPlayersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PlayersPageViewModel.instance.AddExistingPlayerButton(e.ClickedItem as Player);
            PlayersListView.Focus(FocusState.Pointer);
        }

        private void PlayersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PlayersPageViewModel.instance.EditPlayerButton(e.ClickedItem as Player);
        }
    }
}
