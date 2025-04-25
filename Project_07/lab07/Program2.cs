namespace Lab07;

// Usage:
// dotnet run 2 .\files\fileToStoreEncryptedData.txt .\files\fileToStoreDecryptedData.txt


using System.Security.Cryptography;
using System.Text;

public class Program2
{
    public static void Main(string[] args)
    {
        if (!HandleArgs(args))
        {
            return;
        }

        String? fileToHash = args[0];
        String? fileWithHash = args[1];
        String? hashAlgorithm = args[2];

        if (File.Exists(fileWithHash))
        {
            VerifyHash(fileToHash, fileWithHash, hashAlgorithm);
        }
        else
        {
            CalculateHash(fileToHash, fileWithHash, hashAlgorithm);
        }
    }
    
    static void VerifyHash(String fileToHash, String fileWithHash, String hashAlgorithm) {
        if (!(File.Exists(fileToHash) || File.Exists(fileWithHash)))
        {
            Console.WriteLine("Cannot find file to hash or file with  hash");
            return;
        }

        String? hashInFile = null;
        using (StreamReader sr = new StreamReader(fileWithHash))
        {
            hashInFile = sr.ReadToEnd();
        }

        String? expectedHash = null;
        String? dataToHash = null;

        using (StreamReader sr = new StreamReader(fileToHash))
        {
            dataToHash = sr.ReadToEnd();
        }

        switch (hashAlgorithm)
        {
            case "SHA256" :
                expectedHash = skrotSHA256(dataToHash);
                break;
            case "SHA512" :
                expectedHash = skrotSHA512(dataToHash);
                break;
            case "MD5" :
                expectedHash = skrotMD5(dataToHash);
                break;
            default:
                Console.WriteLine("Invalid hash algorithm");
                return;
        }

        if (string.Equals(expectedHash, hashInFile, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Hash is compatible");
        }
        else
        {
            Console.WriteLine("Hashes differ");
        }


    }
        
    static void CalculateHash(String fileToHash, String fileWithHash, String hashAlgorithm)
    {
        String? text = null;
        String? hash = null;
        switch (hashAlgorithm)
        {
            case "SHA256":
                using (StreamReader sr = new StreamReader(fileToHash))
                {
                    text = sr.ReadToEnd();
                }
                hash = skrotSHA256(text);
                using (StreamWriter sw = new StreamWriter(fileWithHash))
                {
                    sw.Write(hash);
                }
                Console.WriteLine($"Calculated hash using algorithm: {hashAlgorithm}");
                break;
            case "SHA512":
                using (StreamReader sr = new StreamReader(fileToHash))
                {
                    text = sr.ReadToEnd();
                }
                hash = skrotSHA512(text);
                using (StreamWriter sw = new StreamWriter(fileWithHash))
                {
                    sw.Write(hash);
                }
                Console.WriteLine($"Calculated hash using algorithm: {hashAlgorithm}");
                break;
            case "MD5":
                using (StreamReader sr = new StreamReader(fileToHash))
                {
                    text = sr.ReadToEnd();
                }
                hash = skrotMD5(text);
                using (StreamWriter sw = new StreamWriter(fileWithHash))
                {
                    sw.Write(hash);
                }
                Console.WriteLine($"Calculated hash using algorithm: {hashAlgorithm}");
                break;
            default:
                Console.WriteLine("Error while writing hash");
                break;
        }
        
    }    


    static bool HandleArgs(String[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Invalid number od parameters");
            return false;
        }

        String? fileToHash = args[0];

        if (!File.Exists(fileToHash))
        {
            Console.WriteLine($"Cannot find file on args[0] with hash : {fileToHash}");
            return false;
        }

        String? hashAlgorith = args[2];

        List<String> algorithms = new List<string>();
        algorithms.Add("SHA256");
        algorithms.Add("SHA512");
        algorithms.Add("MD5");

        if (!algorithms.Contains(hashAlgorith))
        {
            Console.WriteLine($"Invalid hash algorithm: {hashAlgorith}");
            return false;
        }

        return true;

    }
    
    
    static String skrotSHA256(String text)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = SHA256.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(text));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }
    
    static String skrotSHA512(String text)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = SHA512.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(text));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }

    static String skrotMD5(String text)
    {
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        using var hash = MD5.Create();
        byte[] result = hash.ComputeHash(enc.GetBytes(text));
        foreach (var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }
    
}