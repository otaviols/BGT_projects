using System.Diagnostics;

public class CPUTimeSoftYield
{
  private float maxInterval;
  private Stopwatch stopwatch;

  public CPUTimeSoftYield(float maxInterval)
  {
    this.maxInterval = maxInterval;
    this.stopwatch = new Stopwatch();
    this.stopwatch.Start();
  }

  public void NewFrame()
  {
    this.stopwatch.Stop();
    this.stopwatch.Reset();
    this.stopwatch.Start();
  }

  public bool ShouldSoftYield() => (double) this.stopwatch.ElapsedMilliseconds / 1000.0 > (double) this.maxInterval;
}
