using System;

public class AccessPointInfo : IComparable
{
  public string ssid;
  public string bssid;
  public float signalStrength;

  public override string ToString() => string.Format("ssid={0} bssid={1} signalStrength={2}", (object) this.ssid, (object) this.bssid, (object) this.signalStrength);

  public int CompareTo(object obj) => !(obj is AccessPointInfo accessPointInfo) ? -1 : -this.signalStrength.CompareTo(accessPointInfo.signalStrength);
}
