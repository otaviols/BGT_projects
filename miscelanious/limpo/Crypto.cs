using System;
using System.Security.Cryptography;
using System.Text;

public static class Crypto
{
  public static class SHA1
  {
    public static string Calc(byte[] bytes, int start, int count)
    {
      byte[] hash = System.Security.Cryptography.SHA1.Create().ComputeHash(bytes, start, count);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in hash)
        stringBuilder.Append(num.ToString("x2"));
      return stringBuilder.ToString();
    }

    public static string Calc(byte[] bytes) => Crypto.SHA1.Calc(bytes, 0, bytes.Length);

    public static string Calc(string message)
    {
      byte[] numArray = new byte[message.Length * 2];
      Buffer.BlockCopy((Array) message.ToCharArray(), 0, (Array) numArray, 0, numArray.Length);
      return Crypto.SHA1.Calc(numArray);
    }
  }

  public static class Rijndael
  {
    public static byte[] Encrypt(byte[] whatToEncrypt, byte[] secretKey)
    {
      int num = secretKey == null ? 0 : secretKey.Length;
      if (num != 32)
        throw new CryptographicException(string.Format("Key size ({0}) not supported by algorithm - expected {1} bytes", (object) num, (object) 32));
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Key = secretKey;
      rijndaelManaged.Mode = CipherMode.ECB;
      rijndaelManaged.Padding = PaddingMode.PKCS7;
      return rijndaelManaged.CreateEncryptor().TransformFinalBlock(whatToEncrypt, 0, whatToEncrypt.Length);
    }

    public static byte[] Decrypt(byte[] whatToDecrypt, byte[] secretKey)
    {
      int num = secretKey == null ? 0 : secretKey.Length;
      if (num != 32)
        throw new CryptographicException(string.Format("Key size ({0}) not supported by algorithm - expected {1} bytes", (object) num, (object) 32));
      RijndaelManaged rijndaelManaged = new RijndaelManaged();
      rijndaelManaged.Key = secretKey;
      rijndaelManaged.Mode = CipherMode.ECB;
      rijndaelManaged.Padding = PaddingMode.PKCS7;
      return rijndaelManaged.CreateDecryptor().TransformFinalBlock(whatToDecrypt, 0, whatToDecrypt.Length);
    }
  }
}
