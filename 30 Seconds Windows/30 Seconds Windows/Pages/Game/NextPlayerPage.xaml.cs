using _30_Seconds_Windows.Common;
using _30_Seconds_Windows.ViewModels.Game;
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

namespace _30_Seconds_Windows.Pages.Game
{
    public sealed partial class NextPlayerPage : Page
    {
        public NextPlayerPageViewModel ViewModel { get; private set; }
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public NextPlayerPage()
        {
            this.InitializeComponent();

            Frame myFrame = (Frame)Window.Current.Content;
            myFrame.ContentTransitions = null;

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
            ViewModel = NextPlayerPageViewModel.instance;
            DataContext = ViewModel;
            await ViewModel.Load();
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            ViewModel.NavigatedFrom();
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

        private void NextRoundPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void NextPlayerPageContinueButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ContinueButton();
        }
    }
}
