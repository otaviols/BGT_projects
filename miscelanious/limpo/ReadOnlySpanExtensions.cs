using System;

public static class ReadOnlySpanExtensions
{
  public static ReadOnlySpanExtensions.SplitEnumerator Split(
    this ReadOnlySpan<char> span,
    char delimiter)
  {
    return new ReadOnlySpanExtensions.SplitEnumerator(span, delimiter);
  }

  public static bool HasNonSpaceCharacter(this ReadOnlySpan<char> span)
  {
    int length = span.Length;
    bool flag = false;
    for (int index = 0; index < length; ++index)
    {
      if (span[index] != ' ')
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  public ref struct SplitEnumerator
  {
    private ReadOnlySpan<char> m_stringSpan;
    private readonly char m_delimiter;

    public ReadOnlySpan<char> Current { get; private set; }

    public SplitEnumerator(ReadOnlySpan<char> stringSpan, char delimiter)
    {
      this.m_stringSpan = stringSpan;
      this.m_delimiter = delimiter;
      this.Current = new ReadOnlySpan<char>();
    }

    public ReadOnlySpanExtensions.SplitEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
      ReadOnlySpan<char> stringSpan = this.m_stringSpan;
      if (stringSpan.Length == 0)
        return false;
      int length = stringSpan.IndexOf<char>(this.m_delimiter);
      if (length == -1)
      {
        this.m_stringSpan = ReadOnlySpan<char>.Empty;
        this.Current = stringSpan;
        return true;
      }
      this.Current = stringSpan.Slice(0, length);
      this.m_stringSpan = stringSpan.Slice(length + 1);
      return true;
    }
  }
}
