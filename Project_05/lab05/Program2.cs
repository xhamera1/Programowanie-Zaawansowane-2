namespace Lab05;

public class Program2
{
    public static volatile bool stop = false;
    private static Thread inputThread = new Thread(() =>
    {
        while (!stop)
        {
            string? input = Console.ReadLine();
            if (input?.ToLower() == "q")
            {
                stop = true;
            }
        }
    });
    
    
    public static void Main(string[] args)
    {
        inputThread.Start();
        
        using (FileSystemWatcher watcher = new FileSystemWatcher("testDirectory1"))
        {
            watcher.Filter = "*.*"; // wszystkie pliki
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.IncludeSubdirectories = false;

            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;

            watcher.EnableRaisingEvents = true;
            
            while (!stop)
            {
                Thread.Sleep(100); 
            }
        }
    }
    
    private static void OnCreated(object source, FileSystemEventArgs e)
    {
        Console.WriteLine("New file created: " + Path.GetFileName(e.FullPath));
    }

    private static void OnDeleted(object source, FileSystemEventArgs e)
    {
        Console.WriteLine("File deleted: " + Path.GetFileName(e.FullPath));
    }
    
}