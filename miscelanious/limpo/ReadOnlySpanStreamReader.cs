using System;
using System.IO;

public class ReadOnlySpanStreamReader : IDisposable
{
  private StreamReader m_streamReader;
  private char[] m_readBuffer;
  private readonly int m_readBufferSize;
  private int m_totalBufferRead;
  private int m_streamTotalFetched;
  private int m_streamTotalRead;
  private bool m_fileHasMoreContent;
  private bool m_pendingNewLineChar;

  public ReadOnlySpanStreamReader(string path)
  {
    this.m_readBufferSize = 1024;
    this.Init(path);
  }

  public void Dispose() => this.m_streamReader.Dispose();

  public ReadOnlySpan<char> ReadLine()
  {
    if (!this.m_fileHasMoreContent && this.m_streamTotalRead >= this.m_streamTotalFetched)
      return (ReadOnlySpan<char>) (char[]) null;
    if (this.m_totalBufferRead == this.m_readBufferSize)
    {
      this.m_totalBufferRead = 0;
      this.ReadFromFile();
    }
    if (this.m_streamTotalFetched == -1)
      return (ReadOnlySpan<char>) (char[]) null;
    int index1 = this.IndexOfNewLine();
    if (index1 == -1)
    {
      int num = this.m_readBufferSize - this.m_totalBufferRead;
      int totalBufferRead = this.m_totalBufferRead;
      int index2 = 0;
      while (totalBufferRead < this.m_readBufferSize)
      {
        this.m_readBuffer[index2] = this.m_readBuffer[totalBufferRead];
        ++totalBufferRead;
        ++index2;
      }
      this.m_totalBufferRead = num;
      this.ReadFromFile();
      index1 = this.IndexOfNewLine();
      if (index1 == -1)
        index1 = this.m_streamTotalFetched - this.m_streamTotalRead;
    }
    int length = index1 - this.m_totalBufferRead;
    if (!this.m_fileHasMoreContent && length + this.m_streamTotalRead > this.m_streamTotalFetched)
      length = this.m_streamTotalFetched - this.m_streamTotalRead + 1;
    ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(this.m_readBuffer, this.m_totalBufferRead, length);
    int num1 = length + 1;
    if (this.m_readBuffer[index1] == '\r')
    {
      int index3 = index1 + 1;
      if (index3 >= this.m_readBufferSize)
        this.m_pendingNewLineChar = true;
      else if (this.m_readBuffer[index3] == '\n')
        ++num1;
    }
    this.m_totalBufferRead += num1;
    this.m_streamTotalRead += num1;
    return readOnlySpan;
  }

  public bool CanReadLine() => this.m_fileHasMoreContent || this.m_streamTotalRead < this.m_streamTotalFetched;

  private void Init(string path)
  {
    this.m_readBuffer = new char[this.m_readBufferSize];
    this.m_streamReader = new StreamReader(path);
    this.m_fileHasMoreContent = true;
    this.m_pendingNewLineChar = false;
    this.m_streamTotalFetched = -1;
    this.m_totalBufferRead = 0;
    this.m_streamTotalRead = 0;
    this.ReadFromFile();
  }

  private void ReadFromFile()
  {
    int count = this.m_readBufferSize - this.m_totalBufferRead;
    if (count == 0)
      count = this.m_totalBufferRead;
    int num = this.m_streamReader.Read(this.m_readBuffer, this.m_totalBufferRead, count);
    if (num == -1 || num < count)
      this.m_fileHasMoreContent = false;
    this.m_streamTotalFetched += num;
    this.m_totalBufferRead = 0;
    if (!this.m_pendingNewLineChar)
      return;
    if (this.m_readBuffer[0] == '\n')
      ++this.m_totalBufferRead;
    this.m_pendingNewLineChar = false;
  }

  private int IndexOfNewLine()
  {
    for (int totalBufferRead = this.m_totalBufferRead; totalBufferRead < this.m_readBufferSize; ++totalBufferRead)
    {
      if (this.m_readBuffer[totalBufferRead] == '\r' || this.m_readBuffer[totalBufferRead] == '\n')
        return totalBufferRead;
    }
    return -1;
  }
}
