using System;
using UnityEngine;

public class GpsCoordinate
{
  public double Longitude;
  public double Latitude;
  public double Accuracy = double.MaxValue;
  public double Timestamp;

  public static implicit operator GpsCoordinate(LocationInfo locationInfo) => new GpsCoordinate()
  {
    Latitude = (double) locationInfo.latitude,
    Longitude = (double) locationInfo.longitude,
    Accuracy = (double) locationInfo.horizontalAccuracy > 0.0 ? (double) locationInfo.horizontalAccuracy : double.MaxValue,
    Timestamp = locationInfo.timestamp
  };

  public GpsCoordinate()
  {
  }

  public GpsCoordinate(double latitude, double longitude, double accuracy, double timestamp)
  {
    this.Latitude = latitude;
    this.Longitude = longitude;
    this.Accuracy = accuracy;
    this.Timestamp = timestamp;
  }

  public override string ToString() => string.Format("[{0}, {1}] +/-{2}m, {3}s ago", (object) this.Latitude, (object) this.Longitude, (object) this.Accuracy, (object) (int) this.Age());

  public float Age() => (float) (TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds - this.Timestamp);

  public static double HaversineDistance(GpsCoordinate p0, GpsCoordinate p1)
  {
    if (p0 == null || p1 == null)
      return double.MaxValue;
    double num1 = Math.PI / 180.0 * (p1.Latitude - p0.Latitude);
    double num2 = Math.PI / 180.0 * (p1.Longitude - p0.Longitude);
    return 6371000.0 * (2.0 * Math.Asin(Math.Min(1.0, Math.Sqrt(Math.Sin(num1 / 2.0) * Math.Sin(num1 / 2.0) + Math.Cos(Math.PI / 180.0 * p0.Latitude) * Math.Cos(Math.PI / 180.0 * p1.Latitude) * Math.Sin(num2 / 2.0) * Math.Sin(num2 / 2.0)))));
  }

  public static double DistancePaddedWithAccuracy(GpsCoordinate p0, GpsCoordinate p1) => p0 == null || p1 == null ? double.MaxValue : GpsCoordinate.HaversineDistance(p0, p1) + p0.Accuracy + p1.Accuracy;

  public bool Equals(GpsCoordinate other) => other != null && this.Longitude == other.Longitude && this.Latitude == other.Latitude && this.Accuracy == other.Accuracy;
}
