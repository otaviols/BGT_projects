using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using Hearthstone.Core;
using PegasusUtil;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class HttpCheater
{
  private bool m_isReady;
  private string m_address;
  private int m_port;
  private static HttpCheater s_instance;

  private string m_baseUrl => string.Format("http://{0}:{1}", (object) this.m_address, (object) this.m_port);

  public static HttpCheater Get()
  {
    if (HttpCheater.s_instance == null)
    {
      HttpCheater.s_instance = new HttpCheater();
      Network.Get().RegisterNetHandler((object) LocateCheatServerResponse.PacketID.ID, new Network.NetHandler(HttpCheater.s_instance.OnLocateCheatServerResponse));
    }
    return HttpCheater.s_instance;
  }

  public void OnLocateCheatServerResponse()
  {
    LocateCheatServerResponse cheatServerResponse = Network.Get().GetLocateCheatServerResponse();
    this.Initialize(cheatServerResponse.Address, cheatServerResponse.Port);
  }

  public void Initialize(string address, int port)
  {
    this.m_address = address;
    this.m_port = port;
    this.m_isReady = true;
  }

  private IEnumerator LocateServerCoroutine(int timeoutMilliseconds)
  {
    if (!this.m_isReady && !HearthstoneApplication.IsPublic())
    {
      Network.Get().SendLocateCheatServerRequest();
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      while (!this.m_isReady && stopwatch.ElapsedMilliseconds <= (long) timeoutMilliseconds)
        yield return (object) null;
    }
  }

  private Coroutine LocateServer(int timeoutMilliseconds = 5000) => Processor.RunCoroutine(this.LocateServerCoroutine(timeoutMilliseconds));

  public void RunSetResourceCommand(string[] args)
  {
    CheatResource resource;
    string errMsg;
    if (!CheatResourceParser.TryParse(args, out resource, out errMsg))
    {
      UIStatus.Get().AddError(errMsg);
    }
    else
    {
      switch (resource)
      {
        case TutorialCheatResource tutorialCheatResource:
          this.UpdateTutorial(tutorialCheatResource.Progress);
          break;
        case HeroCheatResource heroCheatResource:
          this.UpdateHero(heroCheatResource.ClassName, heroCheatResource.Level, heroCheatResource.Wins, heroCheatResource.Gametype);
          break;
        case ArenaCheatResource arenaCheatResource:
          this.UpdateArenaRecord(arenaCheatResource.Win, arenaCheatResource.Loss);
          break;
      }
    }
  }

  public void RunSkipResourceCommand(string[] args)
  {
    CheatResource resource;
    string errMsg;
    if (!CheatResourceParser.TryParse(args, out resource, out errMsg))
    {
      UIStatus.Get().AddError(errMsg);
    }
    else
    {
      if (!(resource is TutorialCheatResource))
        return;
      this.UpdateTutorial(new int?());
    }
  }

  public void RunUnlockResourceCommand(string[] args)
  {
    CheatResource resource;
    string errMsg;
    if (!CheatResourceParser.TryParse(args, out resource, out errMsg))
    {
      UIStatus.Get().AddError(errMsg);
    }
    else
    {
      if (!(resource is HeroCheatResource heroCheatResource))
        return;
      this.UnlockHero(heroCheatResource.ClassName, heroCheatResource.Premium);
    }
  }

  public void RunAddResourceCommand(string[] args)
  {
    CheatResource resource;
    string errMsg;
    if (!CheatResourceParser.TryParse(args, out resource, out errMsg))
    {
      UIStatus.Get().AddError(errMsg);
    }
    else
    {
      switch (resource)
      {
        case GoldCheatResource goldCheatResource:
          this.UpdateGold(goldCheatResource.Amount);
          break;
        case DustCheatResource dustCheatResource:
          this.UpdateDust(dustCheatResource.Amount);
          break;
        case FullCardCollectionCheatResource _:
          this.GrantCardCollection();
          break;
        case ArenaTicketCheatResource ticketCheatResource:
          this.GrantArenaTicket(ticketCheatResource.TicketCount);
          break;
        case PackCheatResource packCheatResource:
          this.GrantBoosterPack(packCheatResource.PackCount, packCheatResource.TypeID);
          break;
      }
    }
  }

  public void RunRemoveResourceCommand(string[] args)
  {
    CheatResource resource;
    string errMsg;
    if (!CheatResourceParser.TryParse(args, out resource, out errMsg))
    {
      UIStatus.Get().AddError(errMsg);
    }
    else
    {
      switch (resource)
      {
        case GoldCheatResource goldCheatResource:
          if (goldCheatResource.Amount.HasValue)
          {
            int? amount = goldCheatResource.Amount;
            this.UpdateGold(amount.HasValue ? new int?(-amount.GetValueOrDefault()) : new int?());
            break;
          }
          this.RemoveAllGold();
          break;
        case DustCheatResource dustCheatResource:
          if (dustCheatResource.Amount.HasValue)
          {
            int? amount = dustCheatResource.Amount;
            this.UpdateDust(amount.HasValue ? new int?(-amount.GetValueOrDefault()) : new int?());
            break;
          }
          this.RemoveAllDust();
          break;
        case HeroCheatResource heroCheatResource:
          this.RemoveHero(heroCheatResource.ClassName);
          break;
        case FullCardCollectionCheatResource _:
          this.RemoveCardCollection();
          break;
        case ArenaTicketCheatResource ticketCheatResource:
          this.RemoveArenaTicket(ticketCheatResource.TicketCount);
          break;
        case PackCheatResource packCheatResource:
          this.RemoveBoosterPack(packCheatResource.PackCount, packCheatResource.TypeID);
          break;
        case AllAdventureOwnershipCheatResource _:
          Processor.RunCoroutine(this.RemoveResourceCoroutine("adventureownership"));
          break;
      }
    }
  }

  public Coroutine GrantCardCollection() => Processor.RunCoroutine(this.GrantCardCollectionCoroutine());

  public Coroutine RemoveCardCollection() => Processor.RunCoroutine(this.RemoveCardCollectionCoroutine());

  public Coroutine UpdateGold(int? deltaAmount) => Processor.RunCoroutine(this.UpdateGoldCoroutine(deltaAmount));

  public Coroutine RemoveAllGold() => Processor.RunCoroutine(this.RemoveAllGoldCoroutine());

  public Coroutine UpdateDust(int? deltaAmount) => Processor.RunCoroutine(this.UpdateDustCoroutine(deltaAmount));

  public Coroutine RemoveAllDust() => Processor.RunCoroutine(this.RemoveAllDustCoroutine());

  public Coroutine UpdateTutorial(int? progressValue) => Processor.RunCoroutine(this.UpdateTutorialCoroutine(progressValue));

  public Coroutine UpdateHero(
    string className,
    int? heroLevel,
    int? wins,
    string gameType)
  {
    return Processor.RunCoroutine(this.UpdateHeroCoroutine(className, heroLevel, wins, gameType));
  }

  public Coroutine UnlockHero(string className, TAG_PREMIUM? premium) => Processor.RunCoroutine(this.UnlockHeroCoroutine(className, premium));

  public Coroutine RemoveHero(string className) => Processor.RunCoroutine(this.RemoveHeroCoroutine(className));

  public Coroutine GrantArenaTicket(int? ticketCount) => Processor.RunCoroutine(this.GrantArenaTicketCoroutine(ticketCount));

  public Coroutine RemoveArenaTicket(int? ticketCount) => Processor.RunCoroutine(this.RemoveArenaTicketCoroutine(ticketCount));

  public Coroutine UpdateArenaRecord(int? wins, int? losses) => Processor.RunCoroutine(this.UpdateArenaRecordCoroutine(wins, losses));

  public Coroutine GrantBoosterPack(int? packCount, int? typeID) => Processor.RunCoroutine(this.GrantBoosterPackCoroutine(packCount, typeID));

  public Coroutine RemoveBoosterPack(int? packCount, int? typeID) => Processor.RunCoroutine(this.RemoveBoosterPackCoroutine(packCount, typeID));

  private IEnumerator GrantCardCollectionCoroutine()
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    else
      yield return (object) new CheatRequest().SendGetRequest(string.Format("{0}/cheat/cards?accountId={1}", (object) this.m_baseUrl, (object) BattleNet.GetMyGameAccountId().Low));
  }

  private IEnumerator RemoveCardCollectionCoroutine()
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    else
      yield return (object) new CheatRequest().SendDeleteRequest(string.Format("{0}/cheat/cards?accountId={1}", (object) this.m_baseUrl, (object) BattleNet.GetMyGameAccountId().Low));
  }

  private IEnumerator UpdateGoldCoroutine(int? deltaAmount)
  {
    int? nullable = deltaAmount;
    int num = 0;
    if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
    {
      yield return (object) this.LocateServer();
      if (!this.m_isReady)
      {
        HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
      }
      else
      {
        ulong low = BattleNet.GetMyGameAccountId().Low;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("{0}/cheat/gold?accountId={1}", (object) this.m_baseUrl, (object) low);
        if (deltaAmount.HasValue)
          stringBuilder.AppendFormat("&amount={0}", (object) deltaAmount);
        yield return (object) new CheatRequest().SendGetRequest(stringBuilder.ToString());
      }
    }
  }

  public IEnumerator RemoveAllGoldCoroutine()
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    else
      yield return (object) new CheatRequest().SendDeleteRequest(string.Format("{0}/cheat/gold?accountId={1}", (object) this.m_baseUrl, (object) BattleNet.GetMyGameAccountId().Low));
  }

  private IEnumerator UpdateDustCoroutine(int? deltaAmount)
  {
    int? nullable = deltaAmount;
    int num = 0;
    if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
    {
      yield return (object) this.LocateServer();
      if (!this.m_isReady)
      {
        HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
      }
      else
      {
        ulong low = BattleNet.GetMyGameAccountId().Low;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("{0}/cheat/dust?accountId={1}", (object) this.m_baseUrl, (object) low);
        if (deltaAmount.HasValue)
          stringBuilder.AppendFormat("&amount={0}", (object) deltaAmount);
        yield return (object) new CheatRequest().SendGetRequest(stringBuilder.ToString());
      }
    }
  }

  public IEnumerator RemoveAllDustCoroutine()
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    else
      yield return (object) new CheatRequest().SendDeleteRequest(string.Format("{0}/cheat/dust?accountId={1}", (object) this.m_baseUrl, (object) BattleNet.GetMyGameAccountId().Low));
  }

  private IEnumerator UpdateTutorialCoroutine(int? progress)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/tutorial?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (progress.HasValue)
        stringBuilder.AppendFormat("&progress={0}", (object) progress);
      CheatRequest request = new CheatRequest();
      yield return (object) request.SendGetRequest(stringBuilder.ToString());
      if (request.IsSuccessful)
        HearthstoneApplication.Get().Reset();
    }
  }

  private IEnumerator UpdateHeroCoroutine(
    string className,
    int? heroLevel,
    int? wins,
    string gameType)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/hero?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (!string.IsNullOrEmpty(className))
        stringBuilder.AppendFormat("&class={0}", (object) className);
      if (heroLevel.HasValue)
        stringBuilder.AppendFormat("&level={0}", (object) heroLevel);
      if (wins.HasValue)
        stringBuilder.AppendFormat("&wins={0}", (object) wins);
      if (!string.IsNullOrEmpty(gameType))
        stringBuilder.AppendFormat("&gametype={0}", (object) gameType);
      yield return (object) new CheatRequest().SendGetRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator UnlockHeroCoroutine(string className, TAG_PREMIUM? premium)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/hero?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (!string.IsNullOrEmpty(className))
        stringBuilder.AppendFormat("&class={0}", (object) className);
      TAG_PREMIUM? nullable = premium;
      TAG_PREMIUM tagPremium = TAG_PREMIUM.GOLDEN;
      if (nullable.GetValueOrDefault() == tagPremium & nullable.HasValue)
        stringBuilder.AppendFormat("&wins=500");
      int maxHeroLevel = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().MaxHeroLevel;
      stringBuilder.AppendFormat("&level={0}", (object) maxHeroLevel);
      yield return (object) new CheatRequest().SendGetRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator RemoveHeroCoroutine(string className)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/hero?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (!string.IsNullOrEmpty(className))
        stringBuilder.AppendFormat("&class={0}", (object) className);
      yield return (object) new CheatRequest().SendDeleteRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator GrantArenaTicketCoroutine(int? ticketCount)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/arenaticket?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (ticketCount.HasValue)
        stringBuilder.AppendFormat("&ticketCount={0}", (object) ticketCount);
      yield return (object) new CheatRequest().SendGetRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator RemoveArenaTicketCoroutine(int? ticketCount)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/arenaticket?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (ticketCount.HasValue)
        stringBuilder.AppendFormat("&ticketCount={0}", (object) ticketCount);
      yield return (object) new CheatRequest().SendDeleteRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator UpdateArenaRecordCoroutine(int? wins, int? losses)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/arena?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (wins.HasValue)
        stringBuilder.AppendFormat("&win={0}", (object) wins);
      if (losses.HasValue)
        stringBuilder.AppendFormat("&loss={0}", (object) losses);
      CheatRequest request = new CheatRequest();
      yield return (object) request.SendGetRequest(stringBuilder.ToString());
      if (request.IsSuccessful && (bool) (Object) Object.FindObjectOfType<ArenaTrayDisplay>())
      {
        yield return (object) new WaitForSeconds(1f);
        ArenaTrayDisplay.Get().UpdateTray();
      }
    }
  }

  private IEnumerator GrantBoosterPackCoroutine(int? packCount, int? typeID)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/pack?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (packCount.HasValue)
        stringBuilder.AppendFormat("&count={0}", (object) packCount);
      if (typeID.HasValue)
        stringBuilder.AppendFormat("&typeID={0}", (object) typeID);
      yield return (object) new CheatRequest().SendGetRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator RemoveBoosterPackCoroutine(int? packCount, int? typeID)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}/cheat/pack?accountId={1}", (object) this.m_baseUrl, (object) low);
      if (packCount.HasValue)
        stringBuilder.AppendFormat("&count={0}", (object) packCount);
      if (typeID.HasValue)
        stringBuilder.AppendFormat("&typeID={0}", (object) typeID);
      yield return (object) new CheatRequest().SendDeleteRequest(stringBuilder.ToString());
    }
  }

  private IEnumerator RemoveResourceCoroutine(
    string resourceName,
    params KeyValuePair<string, string>[] paramValuePairs)
  {
    yield return (object) this.LocateServer();
    if (!this.m_isReady)
    {
      HttpCheater.LogError("Failed to locate cheat server. Please ensure that the server has Config.Util.Cheat=true enabled.");
    }
    else
    {
      ulong low = BattleNet.GetMyGameAccountId().Low;
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendFormat("{0}/cheat/{1}?accountId={2}", (object) this.m_baseUrl, (object) resourceName, (object) low);
      StringBuilder stringBuilder2 = new StringBuilder();
      foreach (KeyValuePair<string, string> paramValuePair in paramValuePairs)
        stringBuilder2.AppendFormat("&{0}={1}", (object) paramValuePair.Key, (object) paramValuePair.Value);
      stringBuilder1.Append(stringBuilder2.ToString());
      yield return (object) new CheatRequest().SendDeleteRequest(stringBuilder1.ToString());
    }
  }

  public static void LogError(string message)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    UIStatus.Get().AddError(message);
    UnityEngine.Debug.LogError((object) message);
  }
}
