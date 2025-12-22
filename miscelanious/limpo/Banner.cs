using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
  public UberText m_headline;
  public UberText m_caption;
  public GameObject m_bannerDefault;
  public UberText m_headlineDefault;
  public UberText m_captionDefault;
  public GameObject m_glowObject;
  public GameObject m_bannerMurlocHolmes;
  public UberText m_headlineMurlocHolmes;
  public UberText m_captionMurlocHolmes;
  public GameObject m_bannerSuspicious;
  public UberText m_headlineSuspicious;
  public UberText m_captionSuspicious;
  private const string MURLOC_HOLMES = "REV_022";
  private const string SUSPICOUS_CARD = "REV_000e";
  private const string MURLOC_HOMES_BACON_HEROPOWER = "BG23_HERO_303p2";

  public void SetText(string headline)
  {
    this.m_headline.Text = headline;
    this.m_caption.gameObject.SetActive(false);
  }

  public void SetText(string headline, string caption)
  {
    this.m_headline.Text = headline;
    this.m_caption.gameObject.SetActive(true);
    this.m_caption.Text = caption;
  }

  public void MoveGlowForBottomPlacement() => this.m_glowObject.transform.localPosition = new Vector3(this.m_glowObject.transform.localPosition.x, this.m_glowObject.transform.localPosition.y, 0.0f);

  public void SetupBanner(Network.EntityChoices choices, List<Card> cards)
  {
    Entity entity = GameState.Get().GetEntity(choices.Source);
    string cardId = entity.GetCardId();
    if (cardId == "REV_022")
    {
      this.m_bannerDefault.SetActive(false);
      this.m_bannerMurlocHolmes.SetActive(true);
      this.m_headline = this.m_headlineMurlocHolmes;
      this.m_caption = this.m_captionMurlocHolmes;
      string headline = "";
      int tag = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
      switch (tag)
      {
        case 1:
          headline = GameStrings.Get("GAMEPLAY_MURLOC_HOLMES_QUESTION_1");
          break;
        case 2:
          headline = GameStrings.Get("GAMEPLAY_MURLOC_HOLMES_QUESTION_2");
          break;
        case 3:
          headline = GameStrings.Get("GAMEPLAY_MURLOC_HOLMES_QUESTION_3");
          break;
      }
      string caption = string.Format("{0} {1}/3", (object) GameStrings.Get("GAMEPLAY_MURLOC_HOLMES_CLUE"), (object) tag.ToString());
      this.SetText(headline, caption);
    }
    else if (cardId == "REV_000e")
    {
      this.m_bannerDefault.SetActive(false);
      this.m_bannerSuspicious.SetActive(true);
      this.m_headline = this.m_headlineSuspicious;
      this.m_caption = this.m_captionSuspicious;
      this.SetText(GameStrings.Get("GAMEPLAY_SUSPICIOUS_GUESS_HEADLINE"), "");
    }
    else if (cardId == "BG23_HERO_303p2")
    {
      this.m_bannerDefault.SetActive(false);
      this.m_bannerMurlocHolmes.SetActive(true);
      this.m_headline = this.m_headlineMurlocHolmes;
      this.m_caption = this.m_captionMurlocHolmes;
      this.SetText(GameStrings.Get("GAMEPLAY_MURLOC_HOLMES_WARBAND_QUESTION"), GameStrings.Get("GAMEPLAY_MURLOC_HOLMES_CLUE"));
    }
    else
    {
      this.m_bannerDefault.SetActive(true);
      this.m_bannerMurlocHolmes.SetActive(false);
      this.m_headline = this.m_headlineDefault;
      this.m_caption = this.m_captionDefault;
      string headline = GameState.Get().GetGameEntity().CustomChoiceBannerText();
      if (headline == null)
      {
        if (choices.IsSingleChoice())
        {
          if (entity != null)
          {
            string cardDiscoverString = GameDbf.GetIndex().GetCardDiscoverString(entity.GetCardId());
            if (cardDiscoverString != null)
              headline = GameStrings.Get(cardDiscoverString);
          }
          if (headline == null)
          {
            headline = GameStrings.Get("GAMEPLAY_CHOOSE_ONE");
            foreach (Card card in cards)
            {
              if ((Object) null != (Object) card && card.GetEntity().IsHeroPower())
              {
                headline = GameStrings.Get("GAMEPLAY_CHOOSE_ONE_HERO_POWER");
                break;
              }
            }
          }
        }
        else
          headline = string.Format("[PH] Choose {0} to {1}", (object) choices.CountMin, (object) choices.CountMax);
      }
      this.SetText(headline);
    }
  }
}
