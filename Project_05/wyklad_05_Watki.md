# Wątki klasy Thread, współdzielenie pamięci sekcje krytyczne i synchronizacja

## Autor: Tomasz Hachaj

Treść:
- [Tworzenia nowego wątku](#tworzenia-nowego-wątku)
- [Wątki pierwszego planu i tła](#wątki-pierwszego-planu-i-tła)
- [Ustawienia regionalne wątku](#ustawienia-regionalne-wątku)
- [Zakończenie pracy wątku](#zakończenie-pracy-wątku)
- [Współdzielenie pamięci i sekcje krytyczne, synchronizacja wątków](#współdzielenie-pamięci-i-sekcje-krytyczne-synchronizacja-wątków)
    - [Mutex](#mutex)
    - [Semafor (semaphore)](#semafor-semaphore)
    - [Sygnały synchronizujące](#sygnały-synchronizujące)
    - [Join](#join)
    - [Zamki (lock)](#zamki-lock)


## Tworzenia nowego wątku

Utworzenie nowego zarządzonego wątku odbywa się poprzez stworzenia nowej instancji obiektu klasy Thread. Klasa Thread ma konstruktor, który przyjmuje jako parametr delegat ThreadStart. Delegat opakowuje metodę wywoływaną przez nowy wątek podczas wywoływania metody Start klasy Thread. Wywołanie Start więcej niż raz powoduje wyjątek ThreadStateException. Metoda Start zwracana jest natychmiast, często przed rozpoczęciem nowego wątku.

```cs

//Klasa zawierająca metodę, która ma być uruchomiona w nowym wątku
class Watek {
    public string Nazwa;
    public int Opoznienie;
    ThreadStart ?ThreadStart = null; 
    //najprostszym sposobem przekazania danych do wątku jest użycie pól 
    //klasy, w którym znajduje się metoda uruchamiana jako nowy wątek
    public Watek(string Nazwa, int Opoznienie)
    {
        this.Nazwa = Nazwa;
        this.Opoznienie = Opoznienie;
    }
    //metoda odczekuje pewien czas i kończy swoje działanie
    public void Start()
    {
        Console.WriteLine("Start wątku " + Nazwa);
        Thread.Sleep(Opoznienie);
        Console.WriteLine("Zatrzymanie wątku " + Nazwa);
    }
}

class Program {

    public static void Main(string[]argv){
        Random random = new Random(Environment.TickCount);
        //stworzenie listy wątków, każdy z nich będzie wykonywał
        //się przez losowy czas
        List<Thread>Watki = new List<Thread>();
        for (int a = 0; a < 5; a++){
            Watek w = new Watek(a.ToString(), random.Next(10000));                        
            Watki.Add(new Thread(
                        new ThreadStart(
                            w.Start)));
        }
        //uruchomienie wątków
        foreach (Thread t in Watki)
            t.Start();
        Console.WriteLine("Koniec głównego wątku");
    }
}

```
## Wątki pierwszego planu i tła

Obiekt klasy Thread może reprezentować zarówno wątek pierwszego planu jak i wątek w tle. Wątki w tle są identyczne z wątkami pierwszego planu z jednym wyjątkiem: wątek w tle nie zachowuje działania procesu, jeśli wszystkie wątki pierwszego planu zostały zakończone. Po zatrzymaniu wszystkich wątków pierwszego planu środowisko uruchomieniowe zatrzymuje wszystkie wątki w tle i wyłącza je [link](https://learn.microsoft.com/pl-pl/dotnet/api/system.threading.thread?view=net-7.0).

Domyślnie następujące wątki są wykonywane na pierwszym planie:
- Główny wątek aplikacji.
- Wszystkie wątki utworzone przez wywołanie konstruktora klasy Thread.

W tle wykonywane są wątki:
- Wątki puli wątków, które pochodzą z puli wątków roboczych obsługiwanych przez środowisko uruchomieniowe. Ich obsługą zajmuje się klasa ThreadPool. Operacje asynchroniczne oparte na zadaniach są wykonywane automatycznie w wątkach puli wątków. Operacje asynchroniczne oparte na zadaniach używają klasy Task i Task<TResult> do implementowania wzorca asynchronicznego opartego na zadaniach.
- Wszystkie wątki, które wchodzą w zarządzane środowisko wykonywania z niezarządzanego kodu.

Możesz zmienić wątek, aby był wykonywany w tle, ustawiając w dowolnym momencie właściwości IsBackground na true. Wątki w tle są przydatne w przypadku każdej operacji, która powinna być kontynuowana tak długo, jak aplikacja jest uruchomiona, ale nie powinna uniemożliwiać aplikacji zakończenia.

Zmodyfikujmy kod poprzedniej aplikacji zmieniając typ wątków na wykonywane w tle. Jeśli tak zrobimy w momencie zakończenia pracy aplikacji wątki również zakończą swoje działanie.

```cs

//Klasa zawierająca metodę, która ma być uruchomiona w nowym wątku
class Watek {
    public string Nazwa;
    public int Opoznienie;
    ThreadStart ?ThreadStart = null; 
    //najprostszym sposobem przekazania danych do wątku jest użycie pól 
    //klasy, w którym znajduje się metoda uruchamiana jako nowy wątek
    public Watek(string Nazwa, int Opoznienie)
    {
        this.Nazwa = Nazwa;
        this.Opoznienie = Opoznienie;
    }
    //metoda odczekuje pewien czas i kończy swoje działanie
    public void Start()
    {
        Console.WriteLine("Start wątku " + Nazwa);
        Thread.Sleep(Opoznienie);
        Console.WriteLine("Zatrzymanie wątku " + Nazwa);
    }
}

class Program {

    public static void Main(string[]argv){
        Random random = new Random(Environment.TickCount);
        //stworzenie listy wątków, każdy z nich będzie wykonywał
        //się przez losowy czas
        List<Thread>Watki = new List<Thread>();
        for (int a = 0; a < 5; a++){
            Watek w = new Watek(a.ToString(), random.Next(10000));                        
            Watki.Add(new Thread(
                        new ThreadStart(
                            w.Start)));
        }
        //uruchomienie wątków
        foreach (Thread t in Watki)
        {
            t.Start();
            //ustawienie wątków na wykonywane w tle
            t.IsBackground = true;
        }
        Console.WriteLine("Koniec głównego wątku");
    }
}

```

## Ustawienia regionalne wątku

Według dokumentacji [link](https://learn.microsoft.com/pl-pl/dotnet/api/system.threading.thread?view=net-7.0):
każdy wątek ma ustawienie regionalne (CurrentCulture). Bieżące ustawienia regionalne są indywidualne dla każdego wątku. Po utworzeniu obiektu nowego wątku jego ustawienia regionalne definiowane są przez bieżące ustawienia regionalne systemu operacyjnego a nie ustawienia regionalne wątku, z którego tworzony jest nowy wątek. Oznacza to na przykład, że jeśli bieżące ustawienia regionalne systemu to angielski (Stany Zjednoczone), a bieżąca ustawienia podstawowego wątku aplikacji to polski (Polska), to ustawienia regionalne nowego wątku utworzonego przez wywołanie Thread to angielski (Stany Zjednoczone).

Powyższe zachowanie nie dotyczy wątków, które wykonują operacje asynchroniczne dla aplikacji przeznaczonych dla .NET Framework 4.6 i nowszych wersji. W takim przypadku ustawienia regionalne są częścią kontekstu operacji asynchronicznej; wątek, na którym jest wykonywana operacja asynchroniczna, domyślnie dziedziczy te ustawienia z wątku, z którego została uruchomiona operacja asynchroniczna. 

Jak jest faktycznie?

```cs

using System.Globalization;

//Klasa zawierająca metodę, która ma być uruchomiona w nowym wątku
class Watek {
    public string Nazwa;
    public int Opoznienie;
    ThreadStart ?ThreadStart = null; 
    //najprostszym sposobem przekazania danych do wątku jest użycie pól 
    //klasy, w którym znajduje się metoda uruchamiana jako nowy wątek
    public Watek(string Nazwa, int Opoznienie)
    {
        this.Nazwa = Nazwa;
        this.Opoznienie = Opoznienie;
    }
    //metoda odczekuje pewien czas i kończy swoje działanie
    public void Start()
    {
        Console.WriteLine("Start wątku " + Nazwa + " " + CultureInfo.CurrentCulture.Name);
        Thread.Sleep(Opoznienie);
        Console.WriteLine("Zatrzymanie wątku " + Nazwa);
    }
}

class Program {

    public static void Main(string[]argv){
        Random random = new Random(Environment.TickCount);
        //stworzenie listy wątków, każdy z nich będzie wykonywał
        //się przez losowy czas
        Console.WriteLine("Główny wątek: " + CultureInfo.CurrentCulture.Name);
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.NumberFormat.CurrencySymbol = "€";
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        Console.WriteLine("Główny wątek: " + CultureInfo.CurrentCulture.Name);

        List<Thread>Watki = new List<Thread>();
        for (int a = 0; a < 5; a++){
            Watek w = new Watek(a.ToString(), random.Next(10000));                        
            Watki.Add(new Thread(
                        new ThreadStart(
                            w.Start)));
        }
        //uruchomienie wątków
        foreach (Thread t in Watki)
        {
            t.Start();
            //ustawienie wątków na wykonywane w tle
            t.IsBackground = true;
        }
        Console.WriteLine("Koniec głównego wątku");
    }
}

```

## Zakończenie pracy wątku 

Wątek kończy swoje wykonanie, kiedy funkcja startowa od które rozpoczęło się jego działanie ulega zakończeniu.

```cs

class Watek {
    public string Nazwa;
    public bool EndMe = false;
    public Thread Thread = null; 
    //najprostszym sposobem przekazania danych do wątku jest użycie pól 
    //klasy, w którym znajduje się metoda uruchamiana jako nowy wątek
    public Watek(string Nazwa)
    {
        this.Nazwa = Nazwa;
    }
    //metoda odczekuje pewien czas i kończy swoje działanie
    public void Start()
    {
        while (!EndMe)
        {
            Console.WriteLine("Wątek " + Nazwa + " działa!");
            Thread.Sleep(100);
            
        }
        Console.WriteLine("Zatrzymanie wątku " + Nazwa);
    }
}

class Program {

    public static void Main(string[]argv){
        Random random = new Random(Environment.TickCount);

        List<Watek>Watki = new List<Watek>();
        for (int a = 0; a < 5; a++){
            Watek w = new Watek(a.ToString());                        
            w.Thread = new Thread(new ThreadStart(w.Start));
            Watki.Add(w);
        }
        //uruchomienie wątków
        foreach (Watek t in Watki)
        {
            t.Thread.Start();
        }
        Thread.Sleep(10000);
        foreach (Watek t in Watki)
        {
            t.EndMe = true;
        }
        Thread.Sleep(1000);
        Console.WriteLine("Koniec głównego wątku");
    }
}

```

Nie należy zatrzymywać wątków przez metodę Abort() jak jest czasem sugerowane [link](https://www.geeksforgeeks.org/how-to-terminate-a-thread-in-c-sharp/) ponieważ jest to przestarzałe i może wywołać wyjątek:

```cs

System.PlatformNotSupportedException: Thread abort is not supported on this platform.

```

Przykład:

```cs

class Watek {
    public string Nazwa;
    public bool EneMe = false;
    public Thread Thread = null; 
    //najprostszym sposobem przekazania danych do wątku jest użycie pól 
    //klasy, w którym znajduje się metoda uruchamiana jako nowy wątek
    public Watek(string Nazwa)
    {
        this.Nazwa = Nazwa;
    }
    //metoda odczekuje pewien czas i kończy swoje działanie
    public void Start()
    {
        while (!EneMe)
        {
            Console.WriteLine("Wątek " + Nazwa + " działa!");
            Thread.Sleep(100);
            
        }
        Console.WriteLine("Zatrzymanie wątku " + Nazwa);
    }
}

class Program {

    public static void Main(string[]argv){
        Random random = new Random(Environment.TickCount);

        List<Watek>Watki = new List<Watek>();
        for (int a = 0; a < 5; a++){
            Watek w = new Watek(a.ToString());                        
            w.Thread = new Thread(new ThreadStart(w.Start));
            Watki.Add(w);
        }
        //uruchomienie wątków
        foreach (Watek t in Watki)
        {
            t.Thread.Start();
        }
        Thread.Sleep(10000);
        foreach (Watek t in Watki)
        {
            t.Thread.Abort();
        }
        Thread.Sleep(1000);
        Console.WriteLine("Koniec głównego wątku");
    }
}

```

## Współdzielenie pamięci i sekcje krytyczne, synchronizacja wątków

W wypadku grupy wątków, które mają współdziałać przy realizacji wspólnego działania konieczna jest ich synchronizacja. Można użyć do tego poniższe mechanizmy.

### Mutex

Mutex używany jest do synchronizacji dostępu do chronionego zasobu. Mutex przyznaje wyłączny dostęp do współdzielonego zasobu. Stan muteksu jest sygnalizowany, jeśli żaden wątek nie jest jego właścicielem.

Ten przykład pokazuje, jak lokalny obiekt Mutex jest używany do synchronizacji dostępu do chronionego zasobu. Każdy wątek wywołujący jest zablokowany, dopóki nie uzyska własności muteksu. Aby zwolnić własność muteksu musi zostać wywołana metoda ReleaseMutex.

```cs

class Watek
{
    public bool koniec = false;
    public Mutex ?mutex = null;
    public int numer = 0;
    public List<string> ?wyniki = null;
    public void Start()
    {
        while (!koniec)
        {
            UseResource();
        }
        Console.WriteLine("## Koniec wątku " + numer + " ##");
    }

    // W tej metodzie dochodzi do dostępu do sekcji krytycznej
    public void UseResource()
    {
        Console.WriteLine("Wątek {0} chce dostęp do mutex", numer);
        //zarządzaj mutexa
        mutex.WaitOne();  
        Console.WriteLine("Wątek {0} jest w sekcji krytycznej", numer);      
        wyniki.Add("Dane od " + numer);
        //zwolniej mutex
        mutex.ReleaseMutex();
        Console.WriteLine("Wątek {0} zwalnia mutex", numer);
    }
}
class Program
{    
    static void Main()
    {
        List<Watek>watki = new List<Watek>();
        int liczbaWatkow = 10;
        Mutex mutex = new Mutex();
        List<string>wyniki = new List<string>();
        for(int i = 0; i < liczbaWatkow; i++)
        {
            Watek w = new Watek{mutex = mutex, wyniki = wyniki, numer = i};
            watki.Add(w);
            Thread watek = new Thread(new ThreadStart(w.Start));
            watek.Start();
        }
        bool koniec = false;
        //Po otrzymaniu więcej niż 100000 wyników program się kończy
        while (!koniec)
        {
            if (wyniki.Count > 10000)
            {
                koniec = true;
            }
        }
        //Można by zrobić wątki potomne wątkami tła, wtedy zakończenie głównego
        //programu zwolniły z nich pamięć. Mogłoby to spowodować nie odblokowanie mutexa
        //- w zasadzie w takim wypadku nie ma to znaczenia, bo program i tak się kończy, ale trzeba
        //być tego świadomym i wyrabiać dobre nawyki :-)
        foreach (Watek w in watki)
        {
            w.koniec = true;
        }
        Console.WriteLine("## Koniec programu ##"); 
    }
}

```

### Semafor (semaphore)

Ogranicza liczbę wątków, które mogą uzyskać dostęp do wspólnego zasobu lub puli zasobów jednocześnie. Semafor może zostać "opuszczony" (instrukcją WaitOne()) wskazaną przez konstruktor liczbę razy zmniejszając za każdym razem jego stan o 1. Jeśli stan semafora jest równy 0 to instrukcja WaitOne() blokuje wykonanie kodu. Można "podnieść" semafor więcej niż jeden raz instrukcją Release().

Poniższy kod pokazuje wykorzystanie semaforów w problemie czytelników - pisarzy. Zakładamy, że jest ograniczone liczba czytelników, którzy mogą być w sekcji krytycznej oraz tylko jeden pisarz może być w sekcji krytycznej. Może być wielu czytelników i pisarzy.

```cs

public class Czytelnik
{
    Program rodzic = null;
    public bool koniec = false;
    int numer;
    public Czytelnik(Program rodzic, int numer)
    {
        this.rodzic = rodzic;
        this.numer = numer;
    }
    public void Start()
    {
        while (!koniec)
        {
            Console.WriteLine("Czytelnik " + numer + " czeka");
            rodzic.semaforCzytelnikow.WaitOne();
            if (rodzic.dziennik.Count > 0)
                Console.WriteLine("Czytelni " + numer + " przeczytał " 
                    + rodzic.dziennik[rodzic.dziennik.Count - 1] 
                    + " rozmiar dziennika " + rodzic.dziennik.Count);
            rodzic.semaforCzytelnikow.Release();        
        }
    }
}

public class Pisarz
{
    Program rodzic = null;
    public int numer = 0;
    public bool koniec = false; 
    public Pisarz(Program rodzic, int numer)
    {
        this.rodzic = rodzic;
        this.numer = numer;
    }
    public void Start()
    {
        while (!koniec)
        {
            rodzic.semaforPisarzy.WaitOne();
            //zamknij cały semafor czytelników rezerwując wszystkie miejsca
            Console.WriteLine("Piszarz " + numer + " czeka");
            for (int a = 0; a < rodzic.rozmiarSekcjiKrytycznejCzytelnikow; a++)
            {
                rodzic.semaforCzytelnikow.WaitOne();
            }
            //zamknij semafor pisarzy
            rodzic.dziennik.Add("Pisarz " + numer);
            Thread.Sleep(rodzic.random.Next(1000));
            //uwolnij semafor pisarzy
            rodzic.semaforPisarzy.Release();
            //uwolnij cały semafor czytelników
            rodzic.semaforCzytelnikow.Release(rodzic.rozmiarSekcjiKrytycznejCzytelnikow);
        }
    }
}

public class Program
{
    public List<string>dziennik = new List<string>();
    private int liczbaCzytelnikow = 10;
    private int liczbaPisarzy = 3;
    public int rozmiarSekcjiKrytycznejCzytelnikow = 3;
    //załóżmy, że maksymalnie 3 czytelników na raz może czytać
    public Semaphore ?semaforCzytelnikow = null;
    //i że jeden pisarz na raz morze pisać
    public Semaphore ?semaforPisarzy = null;
    List<Czytelnik>czytelnicy = new List<Czytelnik>();
    List<Pisarz>pisarze = new List<Pisarz>();
    public Random random = new Random(Environment.TickCount);
    public void Start()
    {
        for (int a = 0; a < liczbaCzytelnikow; a++)
        {
            Czytelnik czytelnik = new Czytelnik(this, a);
            czytelnicy.Add(czytelnik);
            Thread t = new Thread(new ThreadStart(czytelnik.Start));
            t.Start();
        }
        for (int a = 0; a < liczbaPisarzy; a++)
        {
            Pisarz pisarz = new Pisarz(this, a);
            pisarze.Add(pisarz);
            Thread t = new Thread(new ThreadStart(pisarz.Start));
            t.Start();
        }
        bool koniec = false;
        while (!koniec)
        {
            if (dziennik.Count > 100)
            {
                koniec = true;
            }
        }
        foreach(Pisarz p in pisarze)
            p.koniec = true;
        
        foreach(Czytelnik c in czytelnicy)
            c.koniec = true;

    }

    public Program(int liczbaCzytelnikow, int liczbaPisarzy, int rozmiarSekcjiKrytycznejCzytelnikow)
    {
        this.liczbaCzytelnikow = liczbaCzytelnikow;
        this.liczbaPisarzy = liczbaPisarzy;
        this.rozmiarSekcjiKrytycznejCzytelnikow = rozmiarSekcjiKrytycznejCzytelnikow;
        //załóżmy, że maksymalnie 3 czytelników na raz może czytać
        semaforCzytelnikow = new Semaphore(initialCount: rozmiarSekcjiKrytycznejCzytelnikow, maximumCount: rozmiarSekcjiKrytycznejCzytelnikow);
        //i że jeden pisarz na raz morze pisać, można zamiast tego użyć mutexa
        semaforPisarzy = new Semaphore(initialCount: 1, maximumCount: 1);
    }

    public static void Main()
    {
        Program p = new Program(10, 3, 3);
        p.Start();
    }
}

```

### Sygnały synchronizujące

Kolejną możliwością synchronizacji wątków jest przesyłanie sygnałów. Wątek może wysłać sygnał do drugiego wątku a następnie oczekiwać na jego potwierdzenie. W poniższym przykładzie główny wątek uruchamia wątki potomne. Kiedy uruchomi je wszystkie wysyła sygnał WaitHandle.SignalAndWait(ewhDziecko, ewhRodzic). Jest to instrukcja blokująca czekająca na sygnał ewhRodzic. Wątek potomny czeka na sygnał na instrukcji blokującej ewhDziecko.WaitOne() a następnie odblokowuje wątek główny przy pomocy instrukcji ewhRodzic.Set().

W poniższym przykładzie użyjemy klasy Interlocked, która zapewnia atomowość operacji na zmiennej, która jest dzielona między wątkami.

```cs

public class Watek
{
    public int numer = 0;
    Program rodzic;
    public Watek(Program rodzic, int numer)
    {
        this.rodzic = rodzic;
        this.numer = numer;
    }
    public void Start()
    {
        //Zwiększ zmienną liczącą liczbę aktywnych wątków o 1
        Interlocked.Increment(ref rodzic.liczbaAktywnychWatkow);
        Console.WriteLine("Wątek " + numer + " czeka na sygnał");
        //Czekaj na sygnał ewhDziecko
        rodzic.ewhDziecko.WaitOne();
        //Zmniejsz zmienną liczącą liczbę aktywnych wątków o 1
        Interlocked.Decrement(ref rodzic.liczbaAktywnychWatkow);
        //Odeślij do głównego wątku sygnał, odblokuje to operację
        //WaitHandle.SignalAndWait(ewhDziecko, ewhRodzic); 
        rodzic.ewhRodzic.Set();
        Console.WriteLine("Koniec wątku numer "+ numer);
    }
}
public class Program {
    public long liczbaAktywnychWatkow = 0;
    public int maksymalnaLiczbaWatkow = 5;
    public EventWaitHandle ewhDziecko;
    public EventWaitHandle ewhRodzic;
    public Program()
    {
        ewhDziecko = new EventWaitHandle(false, EventResetMode.AutoReset);
        ewhRodzic = new EventWaitHandle(false, EventResetMode.AutoReset);
    }
    public void Start()
    {
        //Uruchom wszystkie wątki
        for (int a = 0; a < maksymalnaLiczbaWatkow; a++)
        {
            Watek w = new Watek(this, a);
            Thread t = new Thread(new ThreadStart(w.Start));
            t.Start();
        }
        //Poczekaj aż zostanie uruchomione dokładnie maksymalnaLiczbaWatkow
        while (Interlocked.Read(ref liczbaAktywnychWatkow) != maksymalnaLiczbaWatkow)
        {
            Thread.Sleep(100);
        }
        //sygnalizuj do każdego wątku potomnego, czekaj aż wątek potomny odpowie sygnałem ewhRodzic
        while (Interlocked.Read(ref liczbaAktywnychWatkow) > 0)
        {
            WaitHandle.SignalAndWait(ewhDziecko, ewhRodzic);
        }
        Console.WriteLine("Koniec głównego wątku");
    }
    public static void Main()
    {
        Program p = new Program();
        p.Start();
    }

}

```

Można również użyć sygnału ManualResetEvent, który zatrzymuje się na instrukcji blokującej WaitOne() aż do momentu ustawienia instrukcji Set() na sygnale. Odblokowuje ona wszystkie (!) WaitOne() na tym sygnale. Aby WaitOne na tym sygnale znowu mogły blokować, należy zresetować sygnał metodą Reset(). Po wywołaniu Reset() na tym sygnale WaitOne() ponownie stanie się blokująca.

```cs

public class Watek
{
    public int numer = 0;
    Program rodzic;
    public Watek(Program rodzic, int numer)
    {
        this.rodzic = rodzic;
        this.numer = numer;
    }
    public void Start()
    {
        //Zwiększ zmienną liczącą liczbę aktywnych wątków o 1
        Interlocked.Increment(ref rodzic.liczbaAktywnychWatkow);
        Console.WriteLine("Wątek " + numer + " czeka na sygnał");
        //Czekaj na sygnał mre
        rodzic.mre.WaitOne();
        //Zmniejsz zmienną liczącą liczbę aktywnych wątków o 1
        Interlocked.Decrement(ref rodzic.liczbaAktywnychWatkow);
        Console.WriteLine("Koniec wątku numer "+ numer);
    }
}
public class Program {
    public long liczbaAktywnychWatkow = 0;
    public int maksymalnaLiczbaWatkow = 5;
    public ManualResetEvent mre;
    public Program()
    {
        mre = new ManualResetEvent(false);
    }
    public void Start()
    {
        //Uruchom wszystkie wątki
        for (int a = 0; a < maksymalnaLiczbaWatkow; a++)
        {
            Watek w = new Watek(this, a);
            Thread t = new Thread(new ThreadStart(w.Start));
            t.Start();
        }
        //Poczekaj aż zostanie uruchomione dokładnie maksymalnaLiczbaWatkow
        while (Interlocked.Read(ref liczbaAktywnychWatkow) != maksymalnaLiczbaWatkow)
        {
            Thread.Sleep(100);
        }
        Console.WriteLine("Sygnalizuje do wątków potomnych");
        mre.Set();
        while (Interlocked.Read(ref liczbaAktywnychWatkow) > 0)
        {
            Thread.Sleep(100);
        }
        Console.WriteLine("Wszystkie wątki potomne zatrzymano");
        //resetuje sygnał, teraz ponownie mre.WaitOne() staje się instrukcją blokującą
        mre.Reset();
        for (int a = 0; a < maksymalnaLiczbaWatkow; a++)
        {
            Watek w = new Watek(this, a + maksymalnaLiczbaWatkow);
            Thread t = new Thread(new ThreadStart(w.Start));
            t.Start();
        }
        //Poczekaj aż zostanie uruchomione dokładnie maksymalnaLiczbaWatkow
        while (Interlocked.Read(ref liczbaAktywnychWatkow) != maksymalnaLiczbaWatkow)
        {
            Thread.Sleep(100);
        }
        mre.Set();
        Console.WriteLine("Koniec głównego wątku");
    }
    public static void Main()
    {
        Program p = new Program();
        p.Start();
    }

}

```

Jeżeli użyjemy sygnału AutoResetEvent odblokowuje on pojedynczą instrukcję WaitOne().

```cs

public class Watek
{
    public int numer = 0;
    Program rodzic;
    public Watek(Program rodzic, int numer)
    {
        this.rodzic = rodzic;
        this.numer = numer;
    }
    public void Start()
    {
        //Zwiększ zmienną liczącą liczbę aktywnych wątków o 1
        Interlocked.Increment(ref rodzic.liczbaAktywnychWatkow);
        Console.WriteLine("Wątek " + numer + " czeka na sygnał");
        //Czekaj na sygnał mre
        rodzic.mre.WaitOne();
        //Zmniejsz zmienną liczącą liczbę aktywnych wątków o 1
        Interlocked.Decrement(ref rodzic.liczbaAktywnychWatkow);
        Console.WriteLine("Koniec wątku numer "+ numer);
    }
}
public class Program {
    public long liczbaAktywnychWatkow = 0;
    public int maksymalnaLiczbaWatkow = 5;
    public AutoResetEvent mre;
    public Program()
    {
        mre = new AutoResetEvent(false);
    }
    public void Start()
    {
        //Uruchom wszystkie wątki
        for (int a = 0; a < maksymalnaLiczbaWatkow; a++)
        {
            Watek w = new Watek(this, a);
            Thread t = new Thread(new ThreadStart(w.Start));
            t.Start();
        }
        //Poczekaj aż zostanie uruchomione dokładnie maksymalnaLiczbaWatkow
        while (Interlocked.Read(ref liczbaAktywnychWatkow) != maksymalnaLiczbaWatkow)
        {
            Thread.Sleep(100);
        }
        Console.WriteLine("Sygnalizuje do wątków potomnych");
        //sygnalizuj do każdego wątku potomnego, czekaj aż wątek potomny odpowie sygnałem ewhRodzic
        while (Interlocked.Read(ref liczbaAktywnychWatkow) > 0)
        {
            mre.Set();
        }
        Console.WriteLine("Wszystkie wątki potomne zatrzymano");
        Console.WriteLine("Koniec głównego wątku");
    }
    public static void Main()
    {
        Program p = new Program();
        p.Start();
    }

}

```

### Join

Join jest instrukcją blokującą klasy Thread, która czeka aż zakończy się obiekt wątku.

```cs

public class Watek
{
    int numer = 0;
    int opoznienie = 0;
    public Watek(int opoznienie, int numer)
    {
        this.opoznienie = opoznienie;
        this.numer = numer;
    }
    public void Start()
    {
        Console.WriteLine("Wątek " + numer + " zaczyna działanie");
        Thread.Sleep(opoznienie);
        Console.WriteLine("Wątek " + numer + " kończy działanie");
    }
}
public class Program {

    public static void Main()
    {
        Random random = new Random(Environment.TickCount);
        List<Thread>watki = new List<Thread>();
        for (int a = 0; a < 10; a++)
        {
            Watek w = new Watek(random.Next(10000), a);
            watki.Add(new Thread(new ThreadStart(w.Start)));
        }
        foreach(Thread t in watki)
        {
            t.Start();
        }
        Console.WriteLine("Czekam na zakończenie wszystkich wątków");
        foreach(Thread t in watki)
        {
            t.Join();
        }
        Console.WriteLine("Wszystkie wątki potomne zakończyły działanie");
    }
}

```

### Zamki (lock)

Instrukcja lock może zostać użyta do stworzenia sekcji krytycznej gdzie obserwatorem tej sekcji jest zmienna typu object. Kiedy wątek wchodzi do sekcji krytycznej uzyskuje blokadę, wykonuje blok instrukcji, a następnie zwalnia blokadę. Gdy blokada jest utrzymywana, wątek, który przechowuje blokadę, może ponownie uzyskać i zwolnić blokadę. Każdy inny wątek nie może nabyć blokady i czeka na zwolnienie blokady. Instrukcja lock gwarantuje, że pojedynczy wątek ma wyłączny dostęp do tego obiektu. Pojedynczy obiekt może również być użyty do blokady dwóch niezależnych bloków kodu.

```cs

//zakomentuj definicję aby zobaczyć wyjątek  
#define UZYJLOCK
public class Program {
    public static List<string> lista = new List<string>();
    public static bool koniec = false;
    public static void Main()
    {
        Thread t = new Thread(new ThreadStart(dodaj));
        t.Start();
        Thread t2 = new Thread(new ThreadStart(usun));
        t2.Start();
        Thread.Sleep(100000);
        koniec = true;
        Console.WriteLine("Koniec programu");
    }
    public static void dodaj()
    {
        while (!koniec)
            #if UZYJLOCK
            lock(lista)
            #endif
            {
                lista.Add("napis");
            }
    }
    public static void usun()
    {
        while (!koniec)
            #if UZYJLOCK
            lock(lista)
            #endif
            {
                if (lista.Count > 0)
                    lista.RemoveAt(0);
            }
    }
}


```


## Literatura 

[Wątki (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-7.0)

[Tworzenie wątków (Microsoft learn)](https://learn.microsoft.com/pl-pl/dotnet/standard/threading/creating-threads-and-passing-data-at-start-time)

[Synchronizacja wątków (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/standard/threading/overview-of-synchronization-primitives)

[JOIN (Microsoft learn)](https://learn.microsoft.com/pl-pl/dotnet/api/system.threading.thread.join?view=net-7.0)

[Mutex (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/api/system.threading.mutex?view=net-7.0)

[Semafor (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphore?view=net-7.0)

[Eventwaithandle (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/api/system.threading.eventwaithandle?view=net-7.0)

[autoresetevent (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/api/system.threading.autoresetevent?view=net-7.0)

[manualresetevent (Microsoft learn)](https://learn.microsoft.com/en-us/dotnet/api/system.threading.manualresetevent?view=net-7.0)

[Lock (Microsoft learn)](https://learn.microsoft.com/pl-pl/dotnet/csharp/language-reference/statements/lock)
