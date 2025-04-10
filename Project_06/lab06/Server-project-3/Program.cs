using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab06;

public class ProgramServer3
{
    public static String? currentDirectory;
    
    public static void Main(string[] args) 
    {
        currentDirectory = Directory.GetCurrentDirectory(); 
        bool running = true;
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

        while (running)
        {
            byte[] lengthBuffer = new byte[4];
            int totalBytesReadForSize = 0;
            while (totalBytesReadForSize < 4)
            {
                int bytesReadNow = socketClient.Receive(lengthBuffer, totalBytesReadForSize, 4 - totalBytesReadForSize, SocketFlags.None);
                if (bytesReadNow <= 0)
                {
                    running = false;
                    break;
                }
                totalBytesReadForSize += bytesReadNow;
            }

            if (!running) break;

            int len = BitConverter.ToInt32(lengthBuffer, 0);
            if (len <= 0)
            {
                 Console.WriteLine("Błąd: Otrzymano nieprawidłowy rozmiar danych (<= 0).");
                 continue;
            }
            byte[] lenBuffer = new byte[len];
            int totalReceived = 0;
            while (totalReceived < len)
            {
                int receivedNow = socketClient.Receive(lenBuffer, totalReceived, len - totalReceived, SocketFlags.None);
                if (receivedNow <= 0)
                {
                    break;
                }

                totalReceived += receivedNow;
            }

            string receviedMessage = Encoding.UTF8.GetString(lenBuffer, 0, totalReceived);
            Console.WriteLine(receviedMessage);

            

            string response = HandleReceivedMessage(ref receviedMessage, ref running);
            byte[] byteMessage = Encoding.UTF8.GetBytes(response);
            int numberOfBytes = byteMessage.Length;
            byte[] numOfBytesMes = BitConverter.GetBytes(numberOfBytes);
            socketClient.Send(numOfBytesMes, SocketFlags.None);
            socketClient.Send(byteMessage, SocketFlags.None);
        }
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

    public static string HandleReceivedMessage(ref string receivedMessage, ref bool running)
    {
        switch (receivedMessage)
        {
            case "!end":
                running = false;
                return "Program ends...";
            case "list":
                string[] allEntries = Directory.GetFileSystemEntries(currentDirectory!, "*.*");
                StringBuilder sb = new StringBuilder();
                foreach (var entry in allEntries)
                {
                    sb.Append("    " + entry + "\n");
                }
                return sb.ToString();
            default:
                Regex regex = new Regex(@"^in\s+(?:\.\.|(?!\.+$)[^\s]+)$");
                int matches = regex.Match(receivedMessage).Length;
                if (matches > 0)
                {
                    string dirName = receivedMessage.Split(" ")[1];
                    if (dirName.Equals(".."))
                    {
                        currentDirectory = Path.GetDirectoryName(currentDirectory);
                    }
                    else
                    {
                        var directories = Directory.GetDirectories(currentDirectory!).ToList();
                        if (!directories.Any(d => Path.GetFileName(d).Equals(dirName)))
                        {
                            return "Folder doesnt exists in this location...";
                        }

                        currentDirectory += "\\" + dirName;
                    }
                    string[] all = Directory.GetFileSystemEntries(currentDirectory!, "*.*");
                    StringBuilder sb1 = new StringBuilder();
                    foreach (var entry in all)
                    {
                        sb1.Append("    " + entry + "\n");
                    }
                    return sb1.ToString();


                }
                
                return "Unknown command";
                
        }
    }
}