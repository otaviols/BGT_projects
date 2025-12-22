using Blizzard.GameService.SDK.Client.Integration;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ServerConnection<PacketType> where PacketType : PacketFormat, new()
{
  private ISocket m_socket;
  private ClientConnection<PacketType> m_currentConnection;
  private bool m_listening;
  private object m_lock = new object();

  ~ServerConnection() => this.Disconnect();

  public bool Open(int port)
  {
    if (this.m_socket != null)
      return false;
    IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
    try
    {
      this.m_socket = (ISocket) new SocketAdaptor(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this.m_socket.Bind((EndPoint) localEP);
      this.m_socket.Listen(16);
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("SeverConnection: error opening inbound connection: " + ex.Message + " (this probably occurred because you have multiple game instances running)"));
      this.m_socket = (ISocket) null;
      return false;
    }
    return this.Listen();
  }

  public void Disconnect()
  {
    if (this.m_socket == null || !this.m_socket.Connected)
      return;
    this.m_socket.Shutdown(SocketShutdown.Both);
    this.m_socket.Close();
  }

  public bool Listen()
  {
    lock (this.m_lock)
    {
      if (this.m_listening)
        return true;
      this.m_listening = true;
    }
    if (this.m_socket == null)
      return false;
    try
    {
      this.m_socket.BeginAccept(new AsyncCallback(ServerConnection<PacketType>.OnAccept), (object) this);
    }
    catch (Exception ex)
    {
      lock (this.m_lock)
        this.m_listening = false;
      Debug.LogError((object) ("error listening for incoming connections: " + ex.Message));
      this.m_socket = (ISocket) null;
      return false;
    }
    return true;
  }

  private static void OnAccept(IAsyncResult ar)
  {
    ServerConnection<PacketType> asyncState = (ServerConnection<PacketType>) ar.AsyncState;
    if (asyncState == null)
      return;
    if (asyncState.m_socket == null)
      return;
    try
    {
      ISocket socket = asyncState.m_socket.EndAccept(ar);
      asyncState.m_currentConnection = new ClientConnection<PacketType>(socket);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("error accepting connection: " + ex.Message));
    }
    asyncState.m_listening = false;
  }

  public ClientConnection<PacketType> GetNextAcceptedConnection()
  {
    if (this.m_currentConnection != null)
    {
      ClientConnection<PacketType> currentConnection = this.m_currentConnection;
      this.m_currentConnection = (ClientConnection<PacketType>) null;
      return currentConnection;
    }
    this.Listen();
    return (ClientConnection<PacketType>) null;
  }
}
