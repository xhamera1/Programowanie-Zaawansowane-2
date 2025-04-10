using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab06;

public class ProgramServer2 
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

        byte[] lengthBuffer = new byte[4];
        int totalBytesReadForSize = 0;
        while (totalBytesReadForSize < 4)
        {
            int bytesReadNow = socketClient.Receive(lengthBuffer, totalBytesReadForSize, 4 - totalBytesReadForSize, SocketFlags.None); 

            if (bytesReadNow <= 0)
            {
                Console.WriteLine("Połączenie zerwane lub błąd podczas odczytu rozmiaru wiadomości.");
                return; 
            }
            totalBytesReadForSize += bytesReadNow;
        }
        int len = BitConverter.ToInt32(lengthBuffer, 0);
        byte[] buffer = new byte[len];
        int totalReceived = 0;
        while (totalReceived < len)
        {
            int receivedNow = socketClient.Receive(buffer, totalReceived, len - totalReceived, SocketFlags.None);
            if (receivedNow <= 0)
            {
                break;
            }

            totalReceived += receivedNow;
        }

        string receviedMessage = Encoding.UTF8.GetString(buffer, 0, totalReceived);
        Console.WriteLine(receviedMessage);

        string response = "Odczytalem : " + receviedMessage;
        byte[] byteMessage = Encoding.UTF8.GetBytes(response);
        int numberOfBytes = byteMessage.Length;
        byte[] numOfBytesMes = BitConverter.GetBytes(numberOfBytes);
        socketClient.Send(numOfBytesMes, SocketFlags.None);
        socketClient.Send(byteMessage, SocketFlags.None);
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