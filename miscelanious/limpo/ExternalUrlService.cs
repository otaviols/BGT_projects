using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections.Generic;

public class ExternalUrlService : IService
{
  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (GameDbf)
  };

  public void Shutdown()
  {
  }

  public static ExternalUrlService Get() => ServiceManager.Get<ExternalUrlService>();

  public string GetBreakingNewsLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.ALERT, Localization.GetBnetLocaleName().ToLower());

  public string GetFSGLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.FIRESIDE_GATHERINGS);

  public string GetPrivacyPolicyLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.PRIVACY_POLICY);

  public string GetDataManagementLink(string ssoToken) => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.DATA_MANAGEMENT, ssoToken);

  public string GetSystemRequirementsLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.SYSTEM_REQUIREMENTS);

  public string GetRecruitAFriendLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.RECRUIT_A_FRIEND);

  public string GetTermsOfSaleLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.TERMS_OF_SALES);

  public string GetCVVLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.CVV);

  public string GetResetPasswordLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.PASSWORD_RESET);

  public string GetDuplicatePurchaseErrorLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.DUPLICATE_PURCHASE_ERROR);

  public string GetPaymentInfoLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.PAYMENT_INFO);

  public string GetGenericPurchaseErrorLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.GENERIC_PURCHASE_ERROR);

  public string GetAddPaymentLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.ADD_PAYMENT);

  public string GetCustomerSupportLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.CUSTOMER_SUPPORT);

  public string GetMobileGameServerConnectionLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.MOBILE_GAME_SERVER_CONNECTION);

  public string GetChinaRatingsWebsiteLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.CHINA_RATINGS_WEBSITE);

  public string GetAccountDeletionLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.ACCOUNT_DELETION);

  public string GetSoftAccountDeletionLink(string token) => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.ACCOUNT_DELETION_SOFT_ACCOUNT, token);

  public string GetPersonalizedShopRulesLink() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.PERSONALIZED_SHOP_OFFER_RULES);

  public string GetRandomNamesText() => ExternalUrlService.BuildUrl(ExternalUrl.Endpoint.RANDOM_NAMES_TXT);

  private static string BuildUrl(ExternalUrl.Endpoint endpoint, params string[] args)
  {
    string regionStr = ExternalUrlService.GetRegionString();
    int num = HearthstoneApplication.GetMobileEnvironment() == MobileEnv.DEVELOPMENT ? 1 : (HearthstoneApplication.IsInternal() ? 1 : 0);
    ExternalUrlDbfRecord externalUrlDbfRecord = (ExternalUrlDbfRecord) null;
    if (num != 0)
      externalUrlDbfRecord = GameDbf.ExternalUrl.GetRecord((Predicate<ExternalUrlDbfRecord>) (dbf => dbf.AssetFlags == ExternalUrl.AssetFlags.DEV_ONLY && dbf.Endpoint == endpoint));
    if (externalUrlDbfRecord == null)
      externalUrlDbfRecord = GameDbf.ExternalUrl.GetRecord((Predicate<ExternalUrlDbfRecord>) (dbf => dbf.Endpoint == endpoint));
    if (externalUrlDbfRecord == null)
    {
      Log.BattleNet.PrintError("No external URL found for endpoint {0}", (object) endpoint.ToString());
      return regionStr == "CN" ? "https://www.blizzardgames.cn/" : "https://www.blizzard.com/";
    }
    RegionOverridesDbfRecord overridesDbfRecord = externalUrlDbfRecord.RegionOverrides.Find((Predicate<RegionOverridesDbfRecord>) (x => x.Region == regionStr));
    string format = overridesDbfRecord == null ? externalUrlDbfRecord.GlobalUrl : overridesDbfRecord.OverrideUrl;
    try
    {
      string str = string.Format(format, (object[]) args);
      Log.BattleNet.PrintDebug("Url for endpoint {0}: {1}", (object) endpoint.ToString(), (object) str);
      return str;
    }
    catch (Exception ex)
    {
      Log.BattleNet.PrintError(ex.ToString());
      Log.BattleNet.PrintError("Url for endpoint {0} could not be formatted, using unformatted URL instead: {1}", (object) endpoint.ToString(), (object) format);
      return format;
    }
  }

  public static string GetRegionString()
  {
    switch (PlatformSettings.IsMobile() ? (int) MobileDeviceLocale.GetCurrentRegionId() : (int) BattleNet.GetAccountRegion())
    {
      case 1:
        return "US";
      case 2:
        return "EU";
      case 3:
        return "KR";
      case 5:
        return "CN";
      default:
        return "US";
    }
  }
}
