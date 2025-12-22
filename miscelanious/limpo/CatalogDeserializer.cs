using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class CatalogDeserializer
{
  public static List<Network.ShopSale> DeserializeShopSaleList(string jsonString)
  {
    List<Network.ShopSale> shopSaleList = new List<Network.ShopSale>();
    if (string.IsNullOrEmpty(jsonString))
    {
      Log.Store.PrintError("Received no catalog product sale data");
      return shopSaleList;
    }
    try
    {
      if (!(Json.Deserialize(jsonString) is JsonNode jsonNode))
      {
        Log.Store.PrintError("Failed to load sale data. Invalid JSON format:\n{0}", (object) jsonString);
        return shopSaleList;
      }
      foreach (JsonNode node in ((IEnumerable) jsonNode["saleList"]).Cast<JsonNode>())
      {
        int num = CatalogDeserializer.JsonObjectToValue<int>(node["saleId"]);
        long? valueFromJsonNode1 = CatalogDeserializer.TryGetValueFromJsonNode<long>(node, "saleStartDate");
        long? valueFromJsonNode2 = CatalogDeserializer.TryGetValueFromJsonNode<long>(node, "saleSoftEndDate");
        long? valueFromJsonNode3 = CatalogDeserializer.TryGetValueFromJsonNode<long>(node, "saleHardEndDate");
        Network.ShopSale shopSale1 = new Network.ShopSale()
        {
          SaleId = num
        };
        if (valueFromJsonNode1.HasValue)
          shopSale1.StartUtc = new DateTime?(TimeUtils.UnixTimeStampMillisecondsToDateTimeUtc(valueFromJsonNode1.Value));
        DateTime? nullable1;
        DateTime dateTime1;
        if (valueFromJsonNode2.HasValue)
        {
          shopSale1.SoftEndUtc = new DateTime?(TimeUtils.UnixTimeStampMillisecondsToDateTimeUtc(valueFromJsonNode2.Value));
          nullable1 = shopSale1.StartUtc;
          if (nullable1.HasValue)
          {
            nullable1 = shopSale1.StartUtc;
            dateTime1 = shopSale1.SoftEndUtc.Value;
            if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > dateTime1 ? 1 : 0) : 0) != 0)
            {
              Logger store = Log.Store;
              object[] objArray = new object[3]
              {
                (object) num,
                null,
                null
              };
              nullable1 = shopSale1.StartUtc;
              dateTime1 = nullable1.Value;
              objArray[1] = (object) dateTime1.ToString("G");
              nullable1 = shopSale1.SoftEndUtc;
              dateTime1 = nullable1.Value;
              objArray[2] = (object) dateTime1.ToString("G");
              store.PrintWarning("Sale {0} start date exceeds the soft end date. Setting soft end to start. StartUtc={1} SoftEndUtc={2}", objArray);
              shopSale1.SoftEndUtc = shopSale1.StartUtc;
            }
          }
        }
        if (valueFromJsonNode3.HasValue)
        {
          shopSale1.HardEndUtc = new DateTime?(TimeUtils.UnixTimeStampMillisecondsToDateTimeUtc(valueFromJsonNode3.Value));
          nullable1 = shopSale1.StartUtc;
          if (nullable1.HasValue)
          {
            nullable1 = shopSale1.StartUtc;
            dateTime1 = shopSale1.HardEndUtc.Value;
            if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > dateTime1 ? 1 : 0) : 0) != 0)
            {
              Logger store = Log.Store;
              object[] objArray = new object[3]
              {
                (object) num,
                null,
                null
              };
              nullable1 = shopSale1.StartUtc;
              dateTime1 = nullable1.Value;
              objArray[1] = (object) dateTime1.ToString("G");
              nullable1 = shopSale1.HardEndUtc;
              dateTime1 = nullable1.Value;
              objArray[2] = (object) dateTime1.ToString("G");
              store.PrintWarning("Sale {0} start date exceeds the hard end date. Setting hard end to start. StartUtc={1} HardEndUtc={2}", objArray);
              Network.ShopSale shopSale2 = shopSale1;
              nullable1 = shopSale1.StartUtc;
              DateTime? nullable2 = new DateTime?(nullable1.Value);
              shopSale2.HardEndUtc = nullable2;
            }
          }
          nullable1 = shopSale1.SoftEndUtc;
          if (!nullable1.HasValue)
          {
            Logger store = Log.Store;
            object[] objArray = new object[2]
            {
              (object) num,
              null
            };
            nullable1 = shopSale1.HardEndUtc;
            dateTime1 = nullable1.Value;
            objArray[1] = (object) dateTime1.ToString("G");
            store.PrintWarning("Sale {0} has a hard end date but no soft end date. Setting soft end to hard end {1}.", objArray);
            Network.ShopSale shopSale3 = shopSale1;
            nullable1 = shopSale1.HardEndUtc;
            DateTime? nullable3 = new DateTime?(nullable1.Value);
            shopSale3.SoftEndUtc = nullable3;
          }
          else
          {
            nullable1 = shopSale1.SoftEndUtc;
            DateTime dateTime2 = nullable1.Value;
            nullable1 = shopSale1.HardEndUtc;
            DateTime dateTime3 = nullable1.Value;
            if (dateTime2 > dateTime3)
            {
              Logger store = Log.Store;
              object[] objArray = new object[3]
              {
                (object) num,
                null,
                null
              };
              nullable1 = shopSale1.SoftEndUtc;
              dateTime1 = nullable1.Value;
              objArray[1] = (object) dateTime1.ToString("G");
              nullable1 = shopSale1.HardEndUtc;
              dateTime1 = nullable1.Value;
              objArray[2] = (object) dateTime1.ToString("G");
              store.PrintWarning("Sale {0} soft end date exceeds the hard end date. Setting soft end to hard end. SoftEndUtc={1} HardEndUtc={2}", objArray);
              Network.ShopSale shopSale4 = shopSale1;
              nullable1 = shopSale1.HardEndUtc;
              DateTime? nullable4 = new DateTime?(nullable1.Value);
              shopSale4.SoftEndUtc = nullable4;
            }
          }
        }
        shopSaleList.Add(shopSale1);
      }
      Log.Store.Print("Finished deserialization of catalog sales");
    }
    catch (Exception ex)
    {
      Log.Store.PrintError(string.Format("Failed loading catalog product sale data: {0}", (object) ex));
    }
    return shopSaleList;
  }

  private static T? TryGetValueFromJsonNode<T>(JsonNode node, string fieldName) where T : struct
  {
    T? valueFromJsonNode = new T?();
    object obj;
    if (node.TryGetValue(fieldName, out obj))
      valueFromJsonNode = new T?(CatalogDeserializer.JsonObjectToValue<T>(obj));
    return valueFromJsonNode;
  }

  private static T JsonObjectToValue<T>(object obj) => (T) ((IConvertible) obj).ToType(typeof (T), (IFormatProvider) null);
}
