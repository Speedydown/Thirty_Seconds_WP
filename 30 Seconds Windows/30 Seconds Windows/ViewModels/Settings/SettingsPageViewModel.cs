using BaseLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30_Seconds_Windows.ViewModels.Settings
{
    public class SettingsPageViewModel : ViewModel
    {
        public static readonly SettingsPageViewModel instance = new SettingsPageViewModel();

        private SettingsPageViewModel() : base()
        {

        }

        public async Task load()
        {
            IsLoading = true;

            IsLoading = false;
        }
    }
}
