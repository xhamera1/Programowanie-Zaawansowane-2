using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab06;

public class ProgramServer1
{
    public static void Main(string[] args)
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000); // adres, port

        Socket socketServer = new Socket(
            localEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
        
        socketServer.Bind(localEndPoint); 
        socketServer.Listen(100);

        Socket socketClient = socketServer.Accept();

        byte[] buffer = new byte[1024];
        int received = socketClient.Receive(buffer, SocketFlags.None);

        String clientMessage = Encoding.UTF8.GetString(buffer, 0, received);
        Console.WriteLine(clientMessage);

        string response = "Odczytalem : " + clientMessage;
        byte[] echoBytes = Encoding.UTF8.GetBytes(response);
        int bytesToSendServer = Math.Min(echoBytes.Length, 1024);
        socketClient.Send(echoBytes, 0, bytesToSendServer, SocketFlags.None);
        try
        {
            socketClient.Shutdown(SocketShutdown.Both); 
            socketClient.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


    }
}