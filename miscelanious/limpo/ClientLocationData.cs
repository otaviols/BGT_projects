using System.Collections.Generic;
using System.Text;

public class ClientLocationData
{
  public GpsCoordinate location;
  public List<AccessPointInfo> accessPointSamples = new List<AccessPointInfo>();
  public bool complete;

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(string.Format("Best Location:\n{0}\n", (object) this.location));
    stringBuilder.Append("Wifi Samples:\n");
    for (int index = 0; index < this.accessPointSamples.Count; ++index)
      stringBuilder.Append(this.accessPointSamples[index].ToString() + "\n");
    return stringBuilder.ToString();
  }

  public override bool Equals(object obj) => obj is ClientLocationData clientLocationData && this.complete == clientLocationData.complete && this.location.Equals(clientLocationData.location) && this.accessPointSamples.Equals((object) clientLocationData.accessPointSamples);

  public override int GetHashCode() => base.GetHashCode();
}
