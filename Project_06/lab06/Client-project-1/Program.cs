using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab06; 

public class ProgramClient1
{
    public static void Main(string[] args)
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        Socket socket = new Socket(
            localEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        socket.Connect(localEndPoint); 

        string? message = Console.ReadLine();
        byte[] byteMessage = Encoding.UTF8.GetBytes(message!);
        int bytesToSendClient = Math.Min(byteMessage.Length, 1024);
        socket.Send(byteMessage, 0, bytesToSendClient, SocketFlags.None);
        Console.WriteLine("\nYour message bytes count: " + byteMessage.Length + "\n");

        var buffer = new byte[1024];

        int bytesCount = socket.Receive(buffer, SocketFlags.None);
        string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesCount);
        Console.WriteLine(serverResponse);

        try
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch{}


    }
}