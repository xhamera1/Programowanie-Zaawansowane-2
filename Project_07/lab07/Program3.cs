using System.Runtime.Intrinsics.Arm;

// Usage:
// dotnet run fileWithData3.txt fileWithSignature3.txt

namespace Lab07;

using System.Security.Cryptography;
using System.Text;

public class Program3
{
    public static void Main(string[] args)
    {
        String? publicKeyFile = "files1/publicKey.dat";
        String? privateKeyFile = "files1/privateKey.dat";

        if (!VerifyKeyFiles(publicKeyFile, privateKeyFile))
        {
            return;
        }

        String? publicKey = null;
        String? privateKey = null;

        try
        {
            using (StreamReader sr = new StreamReader(publicKeyFile))
            {
                publicKey = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader(privateKeyFile))
            {
                privateKey = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected error while reading files");
        }
        

        if (args.Length != 2)
        {
            Console.WriteLine("Invalid number of program parameters");
            return;
        }

        String? fileWithData = args[0];
        String? fileWithSignature = args[1];

        if (!File.Exists(fileWithData))
        {
            Console.WriteLine("Cannot find file with data (a) ");
            return;
        }
        if (File.Exists(fileWithData) && File.Exists(fileWithSignature))
        {
            VerifySignature(fileWithData, fileWithSignature, publicKey, privateKey);
        }

        if (File.Exists(fileWithData) && (File.Exists(fileWithSignature) == false))
        {
            GenerateSignature(fileWithData, fileWithSignature, publicKey, privateKey);
        }
        
}

    static void GenerateSignature(String? fileWithData, String? fileWithSignature, String? publicKey,
        String? privateKey)
    {
        if (String.IsNullOrEmpty(privateKey))
        {
            Console.WriteLine("private key is empty");
            return;
        }
        if (String.IsNullOrEmpty(fileWithData) || String.IsNullOrEmpty(fileWithSignature))
        {
            Console.WriteLine("file with data or file with signature is empty");
            return;
        }
        
        byte[] dataToSign = File.ReadAllBytes(fileWithData);
        byte[] signature;
        
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(dataToSign);
            
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            } 
        } 
        File.WriteAllBytes(fileWithSignature, signature);
        Console.WriteLine("Signature generated successfully.");
    }
    
    
    
    
    static void VerifySignature(String? fileWithData, String? fileWithSignature, String? publicKey,
        String? privateKey)
    {
        if (string.IsNullOrEmpty(publicKey))
        {
            Console.WriteLine("Public key is missing or empty");
            return;
        }
        if (string.IsNullOrEmpty(fileWithData) || string.IsNullOrEmpty(fileWithSignature))
        {
            Console.WriteLine("data file path or signature file path is null or empty.");
            return;
        }

        byte[] dataToVerify = File.ReadAllBytes(fileWithData);
        byte[] signature = File.ReadAllBytes(fileWithSignature);
        bool isVerified;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(dataToVerify);
            
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                isVerified = rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            } 
        }

        if (isVerified)
        {
            Console.WriteLine("Signature successfully verified");
        }
        else
        {
            Console.WriteLine("Signatures differ");
        }


    }
    
    
    
    

    static bool VerifyKeyFiles(String publicKey, String privateKey)
    {
        if (!File.Exists(publicKey))
        {
            Console.WriteLine("Cannot find public key file");
            return false;
        }
        
        if (!File.Exists(privateKey))
        {
            Console.WriteLine("Cannot find private key file");
            return false;
        }

        return true;

    }
    
}