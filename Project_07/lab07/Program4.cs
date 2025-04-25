using System.Text;

// Usage:
// dotnet run fileA4.txt fileB4.txt password 0
// dotnet run fileB4.txt fileA4prim.txt password 1

namespace Lab07;
using System.Security.Cryptography;

public class Program4
{
    const int SaltSize = 16; 
    const int Iterations = 10000; 

    public static void Main(string[] args)
    {
        if (!VerifyArgs(args))
        {
            return;
        }

        String? fileA = args[0];
        String? fileB = args[1];
        String? password = args[2];
        String? typeOfOperation = args[3];

        if (typeOfOperation.Equals("0"))
        {
            EncodeFileA(fileA, fileB, password);
        }
        else if (typeOfOperation.Equals("1"))
        {
            DecryptFileB(fileA, fileB, password);
        }
        
    }

    static void EncodeFileA(String? fileA, String? fileB, String? password)
    {
        String? dataToEncrypt = null;
        using (StreamReader sr = new StreamReader(fileA))
        {
            dataToEncrypt = sr.ReadToEnd();
        }

        try
        {
            //Plik, do którego zostaną zapisane zaszyfrowane dane 
            using (FileStream fileStream = new(fileB, FileMode.OpenOrCreate))
            {
                //stworzenie instancji algorytmu AES
                using (Aes aes = Aes.Create())
                {
                    //klucz, poniższy ma 256 bitów
                    byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);     
                    var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256); 
                    byte[] key = pbkdf2.GetBytes(32); 

                    aes.Key = key;
                    // wektor inicjujący
                    byte[] iv = aes.IV;
                    fileStream.Write(salt, 0, salt.Length); 
                    fileStream.Write(iv, 0, iv.Length); 

                    using (CryptoStream cryptoStream = new(
                               fileStream,
                               aes.CreateEncryptor(),
                               CryptoStreamMode.Write))
                    {
                        using (StreamWriter encryptWriter = new(cryptoStream))
                        {
                            encryptWriter.Write(dataToEncrypt);
                        }
                    }
                }
            }

            Console.WriteLine("Data enrypted");
        }
        catch
        {
            Console.WriteLine("Unexpected error while encrypting");
        }
    }


    static void DecryptFileB(String? fileA, String? fileB, String? password)
    {
        try
        {
            using (FileStream fileStream = new(fileA, FileMode.Open))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] salt = new byte[SaltSize];
                    int saltBytesRead = fileStream.Read(salt, 0, salt.Length);
                    if (saltBytesRead != salt.Length)
                    {
                        throw new CryptographicException("Could not read salt from file.");
                    }

                    byte[] iv = new byte[aes.IV.Length];
                    int ivBytesRead = fileStream.Read(iv, 0, iv.Length);
                    if (ivBytesRead != iv.Length)
                    {
                        throw new CryptographicException("Could not read IV from file.");
                    }

                    var pbkdf2 =
                        new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256); // Użyj stałej
                    byte[] key = pbkdf2.GetBytes(32); 
                    //wczytanie reszty danych i deszyfrowanie
                    using (CryptoStream cryptoStream = new(
                               fileStream,
                               aes.CreateDecryptor(key, iv),
                               CryptoStreamMode.Read))
                    {
                        string? decryptedMessage = null;
                        using (StreamReader decryptReader = new(cryptoStream))
                        {
                            decryptedMessage = decryptReader.ReadToEnd();
                        }

                        using (StreamWriter sw = new StreamWriter(fileB))
                        {
                            sw.Write(decryptedMessage);
                            Console.WriteLine("Decrypted file");
                        }
                    }
                }
            }
        }
        catch (Exception e) // Złap konkretny wyjątek
        {
            Console.WriteLine("An error occurred during decryption:");
            Console.WriteLine(e.ToString());
        }
    }


    static bool VerifyArgs(String[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Invalid number of arguments");
            return false;
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("Cannot find (a) file");
            return false;
        }

        if (String.IsNullOrEmpty(args[2]))
        {
            Console.WriteLine("Password is invalid");
            return false;
        }

        if (!(args[3] == "0" || args[3] == "1"))
        {
            Console.WriteLine("Invalid type of operation");
            return false;
        }

        return true;

    }
}