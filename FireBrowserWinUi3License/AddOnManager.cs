using System;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace FireBrowserWinUi3License;

public class AddonManager
{
    private StoreContext _storeContext;
    private const string SubscriptionStoreId = "9NL6FM9S0DHV";

    public AddonManager()
    {
        _storeContext = StoreContext.GetDefault();
    }

    public async Task<bool> PurchaseSubscriptionAsync()
    {
        StorePurchaseResult result = await _storeContext.RequestPurchaseAsync(SubscriptionStoreId);

        switch (result.Status)
        {
            case StorePurchaseStatus.Succeeded:
            case StorePurchaseStatus.AlreadyPurchased:
                return true;
            default:
                return false;
        }
    }

    public async Task<bool> IsSubscriptionActiveAsync()
    {
        StoreAppLicense appLicense = await _storeContext.GetAppLicenseAsync();

        foreach (var addOnLicense in appLicense.AddOnLicenses)
        {
            if (addOnLicense.Value.SkuStoreId == SubscriptionStoreId && addOnLicense.Value.IsActive)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<DateTimeOffset?> GetSubscriptionExpirationDateAsync()
    {
        StoreAppLicense appLicense = await _storeContext.GetAppLicenseAsync();

        foreach (var addOnLicense in appLicense.AddOnLicenses)
        {
            if (addOnLicense.Value.SkuStoreId == SubscriptionStoreId && addOnLicense.Value.IsActive)
            {
                return addOnLicense.Value.ExpirationDate;
            }
        }

        return null;
    }
}