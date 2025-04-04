namespace Lab05;

public class Program4
{
    public static int N = 5;
    public static CountdownEvent countdownEventStart = new CountdownEvent(N);
    public static CountdownEvent countdownEventStop = new CountdownEvent(N);
    public static void Main(string[] args)
    {
        List<MyThread2> threads = new List<MyThread2>();
        for (int i = 0; i < N; i++)
        {
            MyThread2 myThread2 = new MyThread2(i, 500);
            myThread2.Thread = new Thread(new ThreadStart(myThread2.Start));
            threads.Add(myThread2);
            myThread2.Thread.Start();
        }

        countdownEventStart.Wait();
        Console.WriteLine("All threads have started.");

        foreach (var thread in threads)
        {
            thread.EndMe = true;
        }
        countdownEventStop.Wait();
        Console.WriteLine("All threads have been stopped.");
        Console.WriteLine("Main thread is exiting.");
    }
}

public class MyThread2
{
    public int Number { get; set; }
    public int Delay { get; set; }
    public Thread? Thread = null;
    public bool EndMe = false;

    public MyThread2(int number, int delay)
    {
        Number = number;
        Delay = delay;
    }

    public void Start()
    {
        Program4.countdownEventStart.Signal();
        while (!EndMe)
        {
            
        }
        Program4.countdownEventStop.Signal();
    }
}
