using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Http;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CheatRequest
{
  public bool IsSuccessful;

  private static HttpStatusCode GetStatusCode(IDictionary<string, string> headers)
  {
    string str;
    if (headers == null || !headers.TryGetValue("STATUS", out str))
      return HttpStatusCode.NotFound;
    string[] strArray = str.Split(' ');
    int result;
    return strArray.Length < 3 || !int.TryParse(strArray[1], out result) ? HttpStatusCode.NotFound : (HttpStatusCode) result;
  }

  private IEnumerator SendGetRequestCoroutine(string url)
  {
    IHttpRequest request = HttpRequestFactory.Get().CreateGetRequest(url);
    yield return (object) request.SendRequest();
    if (request.IsNetworkError || request.IsHttpError)
    {
      if (request.ErrorString.StartsWith("Failed to connect"))
        CheatRequest.LogError("Failed to initiate cheat request. Cheat server is unreachable.");
      else
        CheatRequest.LogError(string.IsNullOrEmpty(request.ResponseAsString) ? request.ErrorString : request.ResponseAsString);
    }
    else
    {
      HttpStatusCode statusCode = CheatRequest.GetStatusCode((IDictionary<string, string>) request.ResponseHeaders);
      if (statusCode != HttpStatusCode.OK)
      {
        CheatRequest.LogError(statusCode, request.ResponseAsString);
        this.IsSuccessful = false;
      }
      else
      {
        this.IsSuccessful = true;
        UIStatus.Get().AddInfo(request.ResponseAsString);
      }
    }
  }

  private IEnumerator SendDeleteRequestCoroutine(string url)
  {
    IHttpRequest request = HttpRequestFactory.Get().CreateDeleteRequest(url);
    yield return (object) request.SendRequest();
    if (request.IsNetworkError)
    {
      if (request.ErrorString.StartsWith("Failed to connect"))
        CheatRequest.LogError("Failed to initiate cheat request. Cheat server is unreachable.");
      else
        CheatRequest.LogError(request.ErrorString);
    }
    else
    {
      string responseAsString = request.ResponseAsString;
      HttpStatusCode responseStatusCode = (HttpStatusCode) request.ResponseStatusCode;
      if (responseStatusCode != HttpStatusCode.OK)
      {
        CheatRequest.LogError(responseStatusCode, responseAsString);
        this.IsSuccessful = false;
      }
      else
      {
        this.IsSuccessful = true;
        UIStatus.Get().AddInfo(responseAsString);
      }
    }
  }

  public Coroutine SendGetRequest(string url) => Processor.RunCoroutine(this.SendGetRequestCoroutine(url));

  public Coroutine SendDeleteRequest(string url) => Processor.RunCoroutine(this.SendDeleteRequestCoroutine(url));

  public static void LogError(string message)
  {
    if (HearthstoneApplication.IsPublic())
      return;
    UIStatus.Get().AddError(message);
    Debug.LogError((object) message);
  }

  public static void LogError(HttpStatusCode statusCode, string message) => CheatRequest.LogError(string.Format("{0} (status code: {1})", (object) message, (object) (int) statusCode));
}
