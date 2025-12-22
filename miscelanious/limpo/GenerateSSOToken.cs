using Blizzard.Commerce;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using UnityEngine;

public class GenerateSSOToken : 
  IUnreliableJobDependency,
  IJobDependency,
  IAsyncJobResult,
  ITokenManager
{
  private bool m_hasResponse;
  private float m_startTime;
  private const float TIMEOUT_THRESHOLD_SECONDS = 12f;

  public bool HasToken { get; private set; }

  public string Token { get; private set; }

  public GenerateSSOToken()
  {
    BattleNet.GenerateAppWebCredentials(new System.Action<bool, string>(this.OnTokenReceieved));
    this.m_startTime = Time.realtimeSinceStartup;
  }

  public bool IsReady() => this.m_hasResponse;

  public bool HasFailed()
  {
    float num = Time.realtimeSinceStartup - this.m_startTime;
    return !this.m_hasResponse && (double) num > 12.0;
  }

  private void OnTokenReceieved(bool hasToken, string token)
  {
    this.m_hasResponse = true;
    this.HasToken = hasToken;
    this.Token = token;
  }
}
