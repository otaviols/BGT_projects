using Blizzard.GameService.SDK.Client.Integration;
using System;
using System.IO;

public class PegasusPacket : PacketFormat
{
  public int Size;
  public int Type;
  public int Context;
  public object Body;
  private bool sizeRead;
  private bool typeRead;

  public PegasusPacket()
  {
  }

  public PegasusPacket(int type, int context, object body)
  {
    this.Type = type;
    this.Context = context;
    this.Size = -1;
    this.Body = body;
  }

  public PegasusPacket(int type, int context, int size, object body)
  {
    this.Type = type;
    this.Context = context;
    this.Size = size;
    this.Body = body;
  }

  public override bool IsLoaded() => this.Body != null;

  public override int Decode(byte[] bytes, int offset, int available)
  {
    string str = "";
    for (int index = 0; index < 8 && index < available; ++index)
      str = str + (object) bytes[offset + index] + " ";
    int num = 0;
    if (!this.typeRead)
    {
      if (available < 4)
        return num;
      this.Type = BitConverter.ToInt32(bytes, offset);
      this.typeRead = true;
      available -= 4;
      num += 4;
      offset += 4;
    }
    if (!this.sizeRead)
    {
      if (available < 4)
        return num;
      this.Size = BitConverter.ToInt32(bytes, offset);
      this.sizeRead = true;
      available -= 4;
      num += 4;
      offset += 4;
    }
    if (this.Body == null && available >= this.Size)
    {
      byte[] destinationArray = new byte[this.Size];
      Array.Copy((Array) bytes, offset, (Array) destinationArray, 0, this.Size);
      this.Body = (object) destinationArray;
      num += this.Size;
    }
    return num;
  }

  public override byte[] Encode()
  {
    if (!(this.Body is IProtoBuf))
      return (byte[]) null;
    IProtoBuf body = (IProtoBuf) this.Body;
    this.Size = (int) body.GetSerializedSize();
    byte[] numArray = new byte[this.Size + 4 + 4];
    Array.Copy((Array) BitConverter.GetBytes(this.Type), 0, (Array) numArray, 0, 4);
    Array.Copy((Array) BitConverter.GetBytes(this.Size), 0, (Array) numArray, 4, 4);
    body.Serialize((Stream) new MemoryStream(numArray, 8, this.Size));
    return numArray;
  }

  public override string ToString() => "PegasusPacket Type: " + (object) this.Type;

  public override bool IsFatalOnError() => this.Type == 168;
}
