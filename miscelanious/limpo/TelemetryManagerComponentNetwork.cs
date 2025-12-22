using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Hearthstone.Telemetry;
using HearthstoneTelemetry;
using System.Collections.Generic;
using System.Text;

public class TelemetryManagerComponentNetwork : ITelemetryManagerComponent, ISocketEventListener
{
  private Map<string, TcpQualitySampler> m_samplers;

  public bool IsInitialized => this.m_samplers != null;

  public void Initialize() => this.m_samplers = new Map<string, TcpQualitySampler>();

  public void Shutdown()
  {
    foreach (KeyValuePair<string, TcpQualitySampler> sampler in this.m_samplers)
      sampler.Value.EndConnection();
  }

  public void ConnectEvent(string address, uint port)
  {
    if (!this.IsInitialized)
      return;
    string key = this.GetKey(address, port);
    if (this.m_samplers.ContainsKey(key))
      return;
    TcpQualitySampler tcpQualitySampler = new TcpQualitySampler(60000f);
    this.m_samplers.Add(key, tcpQualitySampler);
    tcpQualitySampler.StartConnection(address, port);
  }

  public void DisconnectEvent(string address, uint port)
  {
    if (!this.IsInitialized)
      return;
    string key = this.GetKey(address, port);
    TcpQualitySampler tcpQualitySampler;
    if (!this.m_samplers.TryGetValue(key, out tcpQualitySampler))
      return;
    tcpQualitySampler.EndConnection();
    this.m_samplers.Remove(key);
  }

  public void FlushSamplers()
  {
    if (!this.IsInitialized)
      return;
    foreach (KeyValuePair<string, TcpQualitySampler> sampler in this.m_samplers)
      sampler.Value.FlushSampler();
  }

  public void SendPacketEvent(string address, uint port, uint bytes)
  {
    TcpQualitySampler tcpQualitySampler;
    if (!this.IsInitialized || !this.m_samplers.TryGetValue(this.GetKey(address, port), out tcpQualitySampler))
      return;
    tcpQualitySampler.OnMessageSent(bytes);
  }

  public void ReceivePacketEvent(string address, uint port, uint bytes)
  {
    TcpQualitySampler tcpQualitySampler;
    if (!this.IsInitialized || !this.m_samplers.TryGetValue(this.GetKey(address, port), out tcpQualitySampler))
      return;
    tcpQualitySampler.OnMessageReceived(bytes);
  }

  public void ReceivePingEvent(string address, uint port, float travelTime)
  {
    TcpQualitySampler tcpQualitySampler;
    if (!this.IsInitialized || !this.m_samplers.TryGetValue(this.GetKey(address, port), out tcpQualitySampler))
      return;
    tcpQualitySampler.OnPing(travelTime);
  }

  private string GetKey(string address, uint host)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(address);
    stringBuilder.Append(':');
    stringBuilder.Append(host);
    return stringBuilder.ToString();
  }

  public void Update()
  {
  }
}
