namespace Lab05
{
    public class Program1
    {
        private const int PRODUCER_THREADS = 3;
        private const int CONSUMER_THREADS = 3;
        public static volatile bool stop = false;
        public static Queue<Resource> queue = new Queue<Resource>();
        public static Semaphore binarySemaphore = new Semaphore(1, 1);
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
            
            List<Thread> producerThreads = new List<Thread>();
            List<Thread> consumerThreads = new List<Thread>();

            for (int i = 0; i < PRODUCER_THREADS; i++)
            {
                Random random = new Random(Environment.TickCount);
                MyThread producer = new MyThread("Producer", i, random.Next(10000));
                Thread t = new Thread(producer.AddResource);
                producerThreads.Add(t);
                t.Start();
            }
            
            for (int i = 0; i < CONSUMER_THREADS; i++)
            {
                Random random = new Random(Environment.TickCount);
                MyThread consumer = new MyThread("Consumer", i, random.Next(10000));
                Thread t = new Thread(consumer.RemoveResource);
                consumerThreads.Add(t);
                t.Start();
            }
            
            inputThread.Join();
            foreach (var t in producerThreads)
                t.Join();
            foreach (var t in consumerThreads)
                t.Join();
            
        }
    }

    public class Resource
    {
        public int ResourceId { get; set; }
        public int ThreadId { get; set; }
        
        public Resource(int resourceId, int threadId)
        {
            ResourceId = resourceId;
            ThreadId = threadId;
        }
        
        public override string ToString()
        {
            return $"Resource ID: {ResourceId}, Thread ID: {ThreadId}";
        }
    }
    
    public class MyThread
    {
        public string? Type { get; set; }
        public int Number { get; set; }
        public int Delay { get; set; }
        private int resourceCounter = 0;
        public Dictionary<int, int> consumedDictionary = new Dictionary<int, int>();

        public MyThread(string type, int number, int delay)
        {
            Type = type;
            Number = number;
            Delay = delay;

        }

        public void AddResource()
        {
            while (!Program1.stop)
            {
                if (CheckAndSleep(Delay))
                    break;
                Resource resource = new Resource(resourceCounter++, Number);
                Program1.binarySemaphore.WaitOne();

                try
                {
                    Program1.queue.Enqueue(resource);
                    Console.WriteLine("Producer {0} added resource: {1}", Number, resource);
                    // Console.Write("Queue state: ");
                    // foreach (var item in Program1.queue)
                    // {
                    //     Console.Write(item + " ");
                    // }
                    // Console.WriteLine("");
                    Console.WriteLine("Queue count: " + Program1.queue.Count);
                    Console.WriteLine();
                   
                }
                finally
                {
                    Program1.binarySemaphore.Release();
                }
                
            }

        }
        
        public void RemoveResource()
        {
            while (!Program1.stop)
            {
                if (CheckAndSleep(Delay))
                    break;
                Resource? resource = null;
                Program1.binarySemaphore.WaitOne();

                try
                {
                    if (Program1.queue.Count > 0)
                    {
                        resource = Program1.queue.Dequeue();
                        Console.WriteLine("Consumer {0} consumed resource: {1}", Number, resource);
                        if (consumedDictionary.ContainsKey(resource.ThreadId))
                        {
                            consumedDictionary[resource.ThreadId]++;
                        }
                        else
                        {
                            consumedDictionary[resource.ThreadId] = 1;
                        }
                    }
                    else 
                    {
                        Console.WriteLine("Consumer {0} found no resources to consume.", Number);
                    }
                    // Console.Write("Queue state: ");
                    // foreach (var item in Program1.queue)
                    // {
                    //     Console.Write(item + " ");
                    // }
                    // Console.WriteLine("");
                    Console.WriteLine("Queue count: " + Program1.queue.Count);
                    Console.WriteLine();
                }
                finally
                {
                    Program1.binarySemaphore.Release();
                }
            }
            foreach (var item in consumedDictionary)
            {
                Console.WriteLine("Consumer {0} consumed: " + " from Producent {1} : {2} resources", Number, item.Key, item.Value);
            }
        }

        // funckja zeby spalo w kawalkach i sprawdzalo czy nie ma przerwania
        private bool CheckAndSleep(int total)
        {
            int step = 100;
            for (int i=0; i< total; i+=step)
            {
                if (Program1.stop)
                {
                    return true;
                }
                Thread.Sleep(Math.Min(step, total - i));
            }

            return false;
        }
    }
}
