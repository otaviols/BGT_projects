using System;

public class ProductAvailabilityRange
{
  private bool m_isNever;

  public ProductAvailabilityRange()
  {
  }

  public ProductAvailabilityRange(string eventTimingName, DateTime? startUtc, DateTime? endUtc)
  {
    this.Start = new ProductAvailabilityRange.Moment()
    {
      DateTimeUtc = startUtc,
      SourceEventTimingName = eventTimingName
    };
    this.End = new ProductAvailabilityRange.Moment()
    {
      DateTimeUtc = endUtc,
      SourceEventTimingName = eventTimingName
    };
    ProductAvailabilityRange.Moment moment = new ProductAvailabilityRange.Moment();
    ref ProductAvailabilityRange.Moment local = ref moment;
    DateTime? nullable1 = endUtc;
    TimeSpan timeSpan = TimeSpan.FromMinutes(10.0);
    DateTime? nullable2 = nullable1.HasValue ? new DateTime?(nullable1.GetValueOrDefault() - timeSpan) : new DateTime?();
    local.DateTimeUtc = nullable2;
    moment.SourceEventTimingName = eventTimingName;
    this.SoftEnd = moment;
  }

  public ProductAvailabilityRange(Network.ShopSale shopSale)
  {
    DateTime? startUtc = shopSale.StartUtc;
    DateTime? hardEndUtc = shopSale.HardEndUtc;
    DateTime? softEndUtc = shopSale.SoftEndUtc;
    DateTime dateTime1;
    if (startUtc.HasValue)
    {
      ref DateTime? local = ref startUtc;
      dateTime1 = startUtc.Value;
      DateTime dateTime2 = dateTime1.AddSeconds((double) (SpecialEventManager.Get().DevTimeOffsetSeconds * -1L));
      local = new DateTime?(dateTime2);
    }
    if (hardEndUtc.HasValue)
    {
      ref DateTime? local = ref hardEndUtc;
      dateTime1 = hardEndUtc.Value;
      DateTime dateTime3 = dateTime1.AddSeconds((double) (SpecialEventManager.Get().DevTimeOffsetSeconds * -1L));
      local = new DateTime?(dateTime3);
    }
    if (softEndUtc.HasValue)
    {
      ref DateTime? local = ref softEndUtc;
      dateTime1 = softEndUtc.Value;
      DateTime dateTime4 = dateTime1.AddSeconds((double) (SpecialEventManager.Get().DevTimeOffsetSeconds * -1L));
      local = new DateTime?(dateTime4);
    }
    this.Start = new ProductAvailabilityRange.Moment()
    {
      DateTimeUtc = startUtc,
      SourceSaleId = (long) shopSale.SaleId
    };
    this.End = new ProductAvailabilityRange.Moment()
    {
      DateTimeUtc = hardEndUtc,
      SourceSaleId = (long) shopSale.SaleId
    };
    this.SoftEnd = new ProductAvailabilityRange.Moment()
    {
      DateTimeUtc = softEndUtc,
      SourceSaleId = (long) shopSale.SaleId
    };
  }

  public ProductAvailabilityRange.Moment Start { get; set; }

  public ProductAvailabilityRange.Moment End { get; set; }

  public ProductAvailabilityRange.Moment SoftEnd { get; set; }

  public DateTime? StartDateTime => this.Start.DateTimeUtc;

  public DateTime? EndDateTime => this.End.DateTimeUtc;

  public DateTime? SoftEndDateTime => this.SoftEnd.DateTimeUtc;

  public bool IsNever
  {
    get => this.GetDuration().Ticks <= 0L;
    set => this.m_isNever = value;
  }

  public bool IsAlways => !this.m_isNever && !this.StartDateTime.HasValue && !this.EndDateTime.HasValue;

  public bool IsBuyableAtTime(DateTime time)
  {
    TimeSpan displacement;
    return this.TryGetTimeDisplacementRequiredToBeBuyable(time, out displacement) && displacement.Ticks == 0L;
  }

  public bool IsVisibleAtTime(DateTime time)
  {
    TimeSpan displacement;
    return this.TryGetTimeDisplacementRequiredToBeVisible(time, out displacement) && displacement.Ticks == 0L;
  }

  public bool TryGetTimeDisplacementRequiredToBeBuyable(DateTime time, out TimeSpan displacement)
  {
    if (!this.m_isNever)
      return ProductAvailabilityRange.TryGetDisplacementToRange(time, this.StartDateTime, this.EndDateTime, out displacement);
    displacement = new TimeSpan(0L);
    return false;
  }

  public bool TryGetTimeDisplacementRequiredToBeVisible(DateTime time, out TimeSpan displacement)
  {
    if (!this.m_isNever)
      return ProductAvailabilityRange.TryGetDisplacementToRange(time, this.StartDateTime, this.SoftEndDateTime, out displacement);
    displacement = new TimeSpan(0L);
    return false;
  }

  public static bool TryGetDisplacementToRange(
    DateTime time,
    DateTime? start,
    DateTime? end,
    out TimeSpan displacement)
  {
    if (start.HasValue && end.HasValue && start.Value >= end.Value)
    {
      displacement = new TimeSpan(0L);
      return false;
    }
    displacement = !start.HasValue || !(time <= start.Value) ? (!end.HasValue || !(time >= end.Value) ? new TimeSpan(0L) : end.Value - time) : start.Value - time;
    return true;
  }

  public TimeSpan GetDuration()
  {
    if (this.m_isNever)
      return new TimeSpan(0L);
    DateTime? nullable = this.StartDateTime;
    if (nullable.HasValue)
    {
      nullable = this.EndDateTime;
      if (nullable.HasValue)
      {
        nullable = this.EndDateTime;
        DateTime dateTime1 = nullable.Value;
        nullable = this.StartDateTime;
        DateTime dateTime2 = nullable.Value;
        return dateTime1 - dateTime2;
      }
    }
    return new TimeSpan(long.MaxValue);
  }

  public static bool AreOverlapping(ProductAvailabilityRange a, ProductAvailabilityRange b)
  {
    if (a.IsNever || b.IsNever)
      return false;
    if (a.IsAlways || b.IsAlways)
      return true;
    DateTime? startDateTime1 = a.StartDateTime;
    DateTime? endDateTime1 = a.EndDateTime;
    DateTime? startDateTime2 = b.StartDateTime;
    DateTime? endDateTime2 = b.EndDateTime;
    return startDateTime1.HasValue && b.IsBuyableAtTime(startDateTime1.Value) || endDateTime1.HasValue && b.IsBuyableAtTime(endDateTime1.Value) || startDateTime2.HasValue && a.IsBuyableAtTime(startDateTime2.Value) || endDateTime2.HasValue && a.IsBuyableAtTime(endDateTime2.Value);
  }

  public static int CompareNullableStartDateTimes(DateTime? a, DateTime? b)
  {
    if (!a.HasValue || !b.HasValue)
    {
      if (a.HasValue)
        return 1;
      return b.HasValue ? -1 : 0;
    }
    if (a.Value < b.Value)
      return -1;
    return a.Value > b.Value ? 1 : 0;
  }

  public static int CompareNullableEndDateTimes(DateTime? a, DateTime? b)
  {
    if (!a.HasValue || !b.HasValue)
    {
      if (a.HasValue)
        return -1;
      return b.HasValue ? 1 : 0;
    }
    if (a.Value < b.Value)
      return -1;
    return a.Value > b.Value ? 1 : 0;
  }

  public void UnionWith(ProductAvailabilityRange other)
  {
    if (ProductAvailabilityRange.CompareNullableStartDateTimes(other.StartDateTime, this.StartDateTime) <= 0)
      this.Start = other.Start;
    if (ProductAvailabilityRange.CompareNullableEndDateTimes(other.EndDateTime, this.EndDateTime) < 0)
      return;
    this.End = other.End;
    this.SoftEnd = other.SoftEnd;
  }

  public void IntersectWith(ProductAvailabilityRange other)
  {
    if (ProductAvailabilityRange.CompareNullableStartDateTimes(other.StartDateTime, this.StartDateTime) >= 0)
      this.Start = other.Start;
    if (ProductAvailabilityRange.CompareNullableEndDateTimes(other.EndDateTime, this.EndDateTime) > 0)
      return;
    this.End = other.End;
    this.SoftEnd = other.SoftEnd;
  }

  public override string ToString()
  {
    ProductAvailabilityRange.Moment moment1 = this.Start;
    string sourceEventTimingName1 = moment1.SourceEventTimingName;
    moment1 = this.End;
    string sourceEventTimingName2 = moment1.SourceEventTimingName;
    ProductAvailabilityRange.Moment moment2;
    if (sourceEventTimingName1 == sourceEventTimingName2)
    {
      moment2 = this.Start;
      long sourceSaleId1 = moment2.SourceSaleId;
      moment2 = this.End;
      long sourceSaleId2 = moment2.SourceSaleId;
      if (sourceSaleId1 == sourceSaleId2)
      {
        if (this.IsAlways)
        {
          moment2 = this.Start;
          return string.Format("always[{0}]", (object) moment2.GetSourceAsString());
        }
        if (this.IsNever)
        {
          moment2 = this.Start;
          return string.Format("never[{0}]", (object) moment2.GetSourceAsString());
        }
        moment2 = this.Start;
        string dateTimeAsString1 = moment2.GetDateTimeAsString();
        moment2 = this.End;
        string dateTimeAsString2 = moment2.GetDateTimeAsString();
        moment2 = this.Start;
        string sourceAsString = moment2.GetSourceAsString();
        return string.Format("({0} - {1})[{2}]", (object) dateTimeAsString1, (object) dateTimeAsString2, (object) sourceAsString);
      }
    }
    object[] objArray = new object[4];
    moment2 = this.Start;
    objArray[0] = (object) moment2.GetDateTimeAsString();
    moment2 = this.End;
    objArray[1] = (object) moment2.GetDateTimeAsString();
    moment2 = this.Start;
    objArray[2] = (object) moment2.GetSourceAsString();
    moment2 = this.End;
    objArray[3] = (object) moment2.GetSourceAsString();
    return string.Format("({0} - {1})[{2} - {3}])", objArray);
  }

  public struct Moment
  {
    public DateTime? DateTimeUtc { get; set; }

    public string SourceEventTimingName { get; set; }

    public long SourceSaleId { get; set; }

    public string GetDateTimeAsString() => !this.DateTimeUtc.HasValue ? "none" : this.DateTimeUtc.Value.ToLocalTime().ToString("g");

    public string GetSourceAsString()
    {
      if (!string.IsNullOrEmpty(this.SourceEventTimingName))
        return this.SourceEventTimingName;
      return this.SourceSaleId != 0L ? string.Format("Sale {0}", (object) this.SourceSaleId) : "";
    }
  }
}
