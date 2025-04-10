using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab06; 

public class ProgramClient2 
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
        int numberOfBytes = byteMessage.Length;
        byte[] numOfBytesMes = BitConverter.GetBytes(numberOfBytes);
        socket.Send(numOfBytesMes, SocketFlags.None);
        socket.Send(byteMessage, SocketFlags.None);
        Console.WriteLine("\nYour message bytes count: " + byteMessage.Length + "\n");

        
        
        
        byte[] lengthBuffer = new byte[4];
        int totalBytesReadForSize = 0;
        while (totalBytesReadForSize < 4)
        {
            int bytesReadNow = socket.Receive(lengthBuffer, totalBytesReadForSize, 4 - totalBytesReadForSize, SocketFlags.None); // Dla serwera: socketClient.Receive(...)

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
            int receivedNow = socket.Receive(buffer, totalReceived, len - totalReceived, SocketFlags.None);
            if (receivedNow <= 0)
            {
                break;
            }

            totalReceived += receivedNow;
        }

        string receviedMessage = Encoding.UTF8.GetString(buffer, 0, totalReceived);
        Console.WriteLine(receviedMessage);

        try
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch{}


    }
}