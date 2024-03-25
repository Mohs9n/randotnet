using System.Net;
using System.Net.Sockets;
using System.Text;

// IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("nixos.localhost");
IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("nixos");
IPAddress ipAddress = ipHostInfo.AddressList[0];

IPEndPoint ipEndPoint = new(ipAddress, 11_000);

using Socket listener = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp
);

listener.Bind(ipEndPoint);
listener.Listen(100);
// ...
List<Socket> clients = [];
List<string> messages = [];

while (true)
{
    Console.WriteLine("Waiting...");
    var handler = await listener.AcceptAsync();
    clients.Add(handler);
    _ = Task.Run(async () =>
    {
      var buffer = new byte[1024];
      var nameRec = await handler.ReceiveAsync(buffer, SocketFlags.None);
      var username = Encoding.UTF8.GetString(buffer, 0, nameRec);
      Console.WriteLine(username);
      await SendBacklog(handler);
      while (true)
      {
          var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
          if (received == 0)
          {
              Console.WriteLine($"{username} disconnected");
              break; // Connection closed by the client
          }
          try {
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            response = username+": "+response + "\n";
            _ = Task.Run(async () => BroadcastMessage(response, handler));
          }
          catch (SocketException ex)
          {
            if (ex.Message == "Broken Pipe")
              clients.Remove(handler);
            Console.WriteLine(ex.Message);
            break;
          }
        }
    });
}

async Task BroadcastMessage(string msg, Socket sender)
{
  messages.Add(msg);
  foreach(Socket client in clients)
  {
    if (client.Connected)
    {
      if (client.RemoteEndPoint != sender.RemoteEndPoint)
        try
        {
          var sent = await client.SendAsync(Encoding.UTF8.GetBytes(msg), SocketFlags.None);
          Console.WriteLine(sent);
        }
        catch (SocketException ex)
        {
          Console.WriteLine(ex.Message);
        }
    }
    else
    {
      Console.WriteLine("Client Disconnected");
      clients.Remove(client);
    }
  }
}

async Task SendBacklog(Socket client)
{
  foreach(var msg in messages)
    if (client.Connected)
    {
      try
      {
        var sent = await client.SendAsync(Encoding.UTF8.GetBytes(msg), SocketFlags.None);
      }
      catch (SocketException ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
    else
    {
      Console.WriteLine("Client Disconnected");
      clients.Remove(client);
    }
}
