using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("nixos");
IPAddress ipAddress = ipHostInfo.AddressList[0];

IPEndPoint ipEndPoint = new(ipAddress, 11_000);

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp
);

await client.ConnectAsync(ipEndPoint);

var msg = "Hi friends!\n";
var msgBytes = Encoding.UTF8.GetBytes(msg);
var buffer = new byte[1024];


// Send username
while (true)
{
  // Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
  Console.Write("Your Name: ");
  var name = Console.ReadLine();
  if (name == "" || name is null)
  {
    Console.WriteLine("Enter a Usable name :(");
    continue;
  }
  var newNameB = Encoding.UTF8.GetBytes(name);
  _ = await client.SendAsync(newNameB, SocketFlags.None);
  break;
}

// Start receiving messages
_ = Task.Run(async () => 
{
  while(true)
  {
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    if (received==0)
    {
      Console.WriteLine("Server Shutdown");
      break;
    }
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    Console.Write(response);
  }
});

// Send messages
var key = Console.ReadKey().Key;
while(key != ConsoleKey.Escape)
{
  if (key == ConsoleKey.A)
  {
    Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
    Console.Write("You: ");
    var newMsg = Console.ReadLine();
    if (newMsg == "" || newMsg is null)
      continue;
    var newMsgB = Encoding.UTF8.GetBytes(newMsg);
    _ = await client.SendAsync(newMsgB, SocketFlags.None);
  }

  key = Console.ReadKey().Key;
}

client.Shutdown(SocketShutdown.Both);
