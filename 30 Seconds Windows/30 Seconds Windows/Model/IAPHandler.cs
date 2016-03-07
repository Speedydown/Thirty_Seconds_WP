using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace _30_Seconds_Windows.Model
{
    public class IAPHandler
    {
        public static readonly IAPHandler instance = new IAPHandler();

        //Constants
        public const string RemoveAdsFeatureName = "RemoveAds";

        public LicenseInformation licenseInformation { get; private set; }

        private IAPHandler()
        {
#if DEBUG
            licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            licenseInformation = CurrentApp.LicenseInformation;
#endif
        }

        public bool HasFeature(string FeatureName)
        {
            return licenseInformation.ProductLicenses[FeatureName].IsActive;
        }

        public async Task<bool> BuyProduct(string FeatureName)
        {
            try
            {
#if DEBUG
                if (licenseInformation.IsTrial)
                {
                    await CurrentAppSimulator.RequestAppPurchaseAsync(false);
                }

                PurchaseResults Result = await CurrentAppSimulator.RequestProductPurchaseAsync(FeatureName);
#else
                PurchaseResults Result =  await CurrentApp.RequestProductPurchaseAsync(FeatureName);
#endif

                //Check the license state to determine if the in-app purchase was successful.
                return Result.Status == ProductPurchaseStatus.Succeeded || Result.Status == ProductPurchaseStatus.AlreadyPurchased;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
