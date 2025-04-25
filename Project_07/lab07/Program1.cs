using System.Security.Cryptography;
using System.Text;

namespace Lab07
{
    public class Program1
    {
        const string filePublicKey = "files1/publicKey.dat";
        const string filePrivateKey = "files1/privateKey.dat";

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].Equals("0"))
            {
                HandleGenerateKeys();
            }
            else if (args.Length == 3 && args[0].Equals("1"))
            {
                HandleEncrypt(args);
            }
            else if (args.Length == 3 && args[0].Equals("2"))
            {
                HandleDecrypt(args);
            }
            else
            {
                Console.WriteLine("Invalid program parameters");
            }
        }

        static void HandleGenerateKeys()
        {
             try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                   string publicKey = rsa.ToXmlString(false);
                   File.WriteAllText(filePublicKey, publicKey);

                   string privateKey = rsa.ToXmlString(true);
                   File.WriteAllText(filePrivateKey, privateKey);
                   Console.WriteLine("Keys generated successfully.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while trying to handle type 0 command : " + e.Message);
            }
        }

        static void HandleEncrypt(string[] args)
        {
            string inputFile = args[1];
            string outputFile = args[2];
            string? publicKey = null;

            if (!CheckType1(args))
            {
                return;
            }

            if (!File.Exists(filePublicKey))
            {
                Console.WriteLine($"Error, file with public key not found:  {filePublicKey}");
                return;
            }
            try
            {
                publicKey = File.ReadAllText(filePublicKey);
                if (string.IsNullOrEmpty(publicKey))
                {
                    Console.WriteLine($"Error: file with public key:  '{filePublicKey}' is empty");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error on reading from public key file:  '{filePublicKey}': {e.Message}");
                return;
            }

            try
            {
                 using (StreamReader sr = new StreamReader(inputFile))
                 {
                     String dataToEncrypt = sr.ReadToEnd();
                     EncryptText(publicKey, dataToEncrypt, outputFile);
                 }
            }
            catch (IOException ex)
            {
                 Console.WriteLine($"Error reading input file '{inputFile}': {ex.Message}");
            }
            catch (CryptographicException ex)
            {
                 Console.WriteLine($"Cryptographic error during encryption: {ex.Message}");
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"An unexpected error occurred during encryption: {ex.Message}");
            }
        }

        static void HandleDecrypt(string[] args)
        {
            string inputFile = args[1];
            string outputFile = args[2];
            string? privateKey = null;

            if (!CheckType2(args))
            {
                return;
            }
            if (!File.Exists(filePrivateKey))
            {
                Console.WriteLine($"Error, file with private key not found:  {filePrivateKey}"); // Corrected variable name here
                return;
            }

            try
            {
                privateKey = File.ReadAllText(filePrivateKey);
                if (string.IsNullOrEmpty(privateKey))
                {
                    Console.WriteLine($"Error: file with private key:  '{filePrivateKey}' is empty");
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error on reading from private key file:  '{filePrivateKey}': {e.Message}"); // Corrected typo "pprivate"
                return;
            }

            String? decryptedText = null;

            try
            {
                decryptedText = DecryptData(privateKey, inputFile);

                using (StreamWriter sw = new StreamWriter(outputFile))
                {
                    sw.Write(decryptedText);
                }
                 Console.WriteLine($"Decrypted data written to {outputFile}");
            }
            catch (IOException ex)
            {
                 Console.WriteLine($"Error reading encrypted file or writing output file: {ex.Message}");
            }
             catch (CryptographicException ex)
            {
                 Console.WriteLine($"Cryptographic error during decryption (maybe wrong key or corrupted data?): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during decryption or writing output: {ex.Message}");
            }
        }


        public static bool CheckType1(string[] args)
        {
            String? fileToEncrypt = args[1];
            if (!File.Exists(fileToEncrypt))
            {
                Console.WriteLine($"Cannot find the input file to encrypt: {fileToEncrypt}");
                return false;
            }
            return true;
        }

        public static bool CheckType2(string[] args)
        {
            String? fileToDecrypt = args[1];
            String? fileToStoreDecryptedData = args[2];
            if (!File.Exists(fileToDecrypt)) // Only check input file
            {
                 Console.WriteLine($"Cannot find the input file to decrypt: {fileToDecrypt}");
                return false;
            }
             // Optional: Check if output directory exists, but not the file itself
            var outputDir = Path.GetDirectoryName(fileToStoreDecryptedData);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                 Console.WriteLine($"Output directory does not exist: {outputDir}");
                 return false;
            }

            return true;
        }

        public static void EncryptText(string publicKeyXml, string text, string outputFileName)
        {
            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(text);

            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyXml);
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }
            File.WriteAllBytes(outputFileName, encryptedData);

            Console.WriteLine("Data encrypted successfully.");
        }

        static string DecryptData(string privateKeyXml, string encryptedFileName)
        {
            byte[] dataToDecrypt = File.ReadAllBytes(encryptedFileName);

            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }

            UnicodeEncoding byteConverter = new UnicodeEncoding();
            return byteConverter.GetString(decryptedData);
        }
    }
}
