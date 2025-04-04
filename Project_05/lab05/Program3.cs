using System.Collections.Concurrent;

namespace Lab05;

public class Program3
{
    public const string? NAPIS = "ron";
    public const string? START_DIRECTORY_PATH = "testDirectory2";
    public static BlockingCollection<string> foundFiles = new BlockingCollection<string>();

    private static Thread searchThread = new Thread(() =>
    {
        if (File.Exists(START_DIRECTORY_PATH))
        {
            ProcessFile(START_DIRECTORY_PATH);
        }
        else if (Directory.Exists(START_DIRECTORY_PATH))
        {
            ProcessDirectory(START_DIRECTORY_PATH);
        }
        else
        {
            Console.WriteLine("Directory or file does not exist.");
        }
        
        foundFiles.CompleteAdding();
    });
    
    public static void Main(string[] args)
    {
        searchThread.Start();


        while (!foundFiles.IsCompleted)
        {
            try
            {
                string filename = foundFiles.Take();
                Console.WriteLine(filename);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        
        
    }



    public static void ProcessDirectory(string targetDirectory)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        {
            ProcessFile(fileName);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        
        foreach (var subdirectory in subdirectoryEntries)
        {
            ProcessDirectory(subdirectory);
        }
    }

    public static void ProcessFile(string? path)
    {
        string? filename = Path.GetFileName(path);

        if (filename!.Contains(NAPIS!))
        {
            foundFiles.Add(filename);
        }

    }
}

