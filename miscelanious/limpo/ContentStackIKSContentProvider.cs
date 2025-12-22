using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Content.Delivery;
using MiniJSON;
using System;
using System.Collections;

public class ContentStackIKSContentProvider : BaseIKSContentProvider
{
  private ContentStackConnect m_connect = new ContentStackConnect();

  public override bool Ready => this.m_connect.Ready;

  public override void InitializeJsonURL(string customURL)
  {
    this.m_connect.InitializeURL("innkeeper_special", Vars.Key("ContentStack.Env").GetStr("production"), Localization.GetBnetLocaleName(), BattleNet.GetCurrentRegion() == BnetRegion.REGION_CN, "IKS_LAST_STORED_RESPONSE", 0);
    if (string.IsNullOrEmpty(customURL))
      return;
    this.m_connect.ResetServiceURL(customURL);
  }

  public override JsonList GetRootListNode(JsonNode response)
  {
    if (response.ContainsKey("entries"))
      return response["entries"] as JsonList;
    if (!response.ContainsKey("entry"))
      return (JsonList) null;
    JsonList rootListNode = new JsonList();
    rootListNode.Add(response["entry"]);
    return rootListNode;
  }

  public override IEnumerator GetQuery(
    ResponseProcessHandler responseProcessHandler,
    object param,
    bool force)
  {
    return this.m_connect.Query(responseProcessHandler, param, string.Empty, force);
  }

  public override InnKeepersSpecialAd ReadInnKeepersSpecialAd(JsonNode adNode) => new InnKeepersSpecialAd()
  {
    Importance = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_IMPORTANCE),
    PublishDate = this.GetAttribute<long>(adNode, BaseIKSContentProvider.AdAttributes.AD_PUBLISH),
    CampaignName = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_CAMPAIGN_NAME),
    Title = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_TITLE),
    SubTitle = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_SUBTITLE),
    Link = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_LINK),
    MaxViewCount = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_MAX_VIEW_COUNT),
    GameAction = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_GAME_ACTION),
    ButtonText = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_BUTTON_TEXT),
    TitleOffsetX = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_TITLE_OFFSET_X),
    TitleOffsetY = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_TITLE_OFFSET_Y),
    SubTitleOffsetX = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_SUBTITLE_OFFSET_X),
    SubTitleOffsetY = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_SUBTITLE_OFFSET_Y),
    TitleFontSize = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_TITLE_FONT_SIZE),
    SubTitleFontSize = this.GetAttribute<int>(adNode, BaseIKSContentProvider.AdAttributes.AD_SUBTITLE_FONT_SIZE),
    ImageUrl = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_URL),
    ClientVersion = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_CLIENT_VERSION),
    Platform = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_PLATFORM),
    AndroidStore = this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_ANDROID_STORE),
    Visibility = StringUtils.CompareIgnoreCase("public", this.GetAttribute<string>(adNode, BaseIKSContentProvider.AdAttributes.AD_VISIBILITY))
  };

  private T GetAttribute<T>(JsonNode adNode, BaseIKSContentProvider.AdAttributes attr)
  {
    switch (attr)
    {
      case BaseIKSContentProvider.AdAttributes.AD_IMPORTANCE:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("importance", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_PUBLISH:
        return (T) (ValueType) Convert.ToDateTime((string) (adNode["publish_details"] as JsonNode)["time"]).Ticks;
      case BaseIKSContentProvider.AdAttributes.AD_CAMPAIGN_NAME:
        return BaseIKSContentProvider.GetStringFromNode<T>("title", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_TITLE:
        return BaseIKSContentProvider.GetStringFromNode<T>("in_game_ad_title", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_SUBTITLE:
        return BaseIKSContentProvider.GetStringFromNode<T>("in_game_ad_subtitle", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_LINK:
        return BaseIKSContentProvider.GetStringFromNode<T>("url", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_MAX_VIEW_COUNT:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("maxviewcount", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_GAME_ACTION:
        return BaseIKSContentProvider.GetStringFromNode<T>("gameaction", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_BUTTON_TEXT:
        return BaseIKSContentProvider.GetStringFromNode<T>("buttontext", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_TITLE_OFFSET_X:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("titleoffsetx", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_TITLE_OFFSET_Y:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("titleoffsety", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_SUBTITLE_OFFSET_X:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("subtitleoffsetx", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_SUBTITLE_OFFSET_Y:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("subtitleoffsety", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_TITLE_FONT_SIZE:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("titlefontsize", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_SUBTITLE_FONT_SIZE:
        return BaseIKSContentProvider.GetIntegerFromNode<T>("subtitlefontsize", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_URL:
        return BaseIKSContentProvider.GetStringFromNode<T>("url", adNode["in_game_ad"] as JsonNode);
      case BaseIKSContentProvider.AdAttributes.AD_CLIENT_VERSION:
        return BaseIKSContentProvider.GetStringFromNode<T>("clientversion", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_PLATFORM:
        return BaseIKSContentProvider.GetStringFromNode<T>("platform", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_ANDROID_STORE:
        return BaseIKSContentProvider.GetStringFromNode<T>("androidstore", adNode);
      case BaseIKSContentProvider.AdAttributes.AD_VISIBILITY:
        return BaseIKSContentProvider.GetStringFromNode<T>("visibility", adNode);
      default:
        return default (T);
    }
  }
}
