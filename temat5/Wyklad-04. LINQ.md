# Funkcje anonimowe, Wyrażenia lambda, Language-Integrated Query (LINQ)

## Autor: Tomasz Hachaj

Treść:

- [Funkcje anonimowe](#funkcje-anonimowe)
- [Wyrażenia lambda](#wyrażenia-lambda)
- [LINQ](#linq)
- [Podkwerendy](#podkwerendy)
- [Złączenia](#złączenia)
- [Agregacje](#agregacje)
- [Literatura](#literatura)


## Funkcje anonimowe


Metoda / funkcja anonimowa to metoda bez nazwy. Metody anonimowe w C# mogą być zdefiniowane przy użyciu słowa kluczowego delegate i mogą być przypisane do zmiennej typu delegate. 

```cs

var kwadrat_5 = delegate(double x)
{
    //Console.WriteLine(x * x);
    return x * x;
};

Console.WriteLine(kwadrat_5(4));

kwadrat_6 k6 = delegate(double x)
{
    return x * x * x;
};
Console.WriteLine(k6(4));

public delegate double kwadrat_6(double value);

```

Anonimowe metody mogą być również przekazywane do metody, która akceptuje delegata jako parametr. 

```cs

public delegate void Print(int value);
class Program
{
    public static void PrintHelperMethod(Print printDel,int val)
    { 
        val += 10;
        printDel(val);
    }

    static void Main(string[] args)
    {
        PrintHelperMethod(delegate(int val) { Console.WriteLine("Anonymous method: {0}", val); }, 100);
    }
}

```

Metody anonimowe mogą być używane do obsługi zdarzeń (event handlers). 

```cs

saveButton.Click += delegate(Object o, EventArgs e)
{ 
    Console.WriteLine("Klik! :-)"); 
};

```


Będzie to szczegółowo omówione w dalszej części wykładów.


Podsumowanie:
- Metoda anonimowa może być zdefiniowana przy użyciu słowa kluczowego delegate.
- Metoda anonimowa musi być przypisana do zmiennej typu delegate.
- Metoda anonimowa może uzyskać dostęp do zewnętrznych zmiennych lub funkcji.
- Metoda anonimowa może być przekazywana jako parametr.
- Metoda anonimowa może być używana jako obsługa zdarzeń.

Ograniczenia metod anonimowych:
- Nie może zawierać instrukcji skoku jak goto, break oraz continue.
- Nie może mieć dostępu do parametru ref lub out zewnętrznej metody.
- Nie może mieć dostępu do niebezpiecznego kodu (unsafe).
- Nie może być używany po lewej stronie operatora is.

```cs

int a = 7;
bool b = a is int;

```

## Wyrażenia lambda

Wyrażenie lambda jest krótszym sposobem reprezentowania anonimowej metody za pomocą pewnej specjalnej składni. 

Do definiowania wyrażeń lambda używa się operatora => . Ten operator nie może zostać przeciążony. Jest on obsługiwany w dwóch formach: jako operator lambda i jako separator nazwy funkcji anonimowej i jej implementacji.

```cs

// operator lambda
metoda(x => Console.WriteLine(x), 45);
void metoda(printResults p, double x)
    {
        double yy = x * 123.3 - Math.PI;
        p(yy.ToString());
    }
delegate void printResults(String val);

// separator nazwy funkcji anonimowej i jej implementacji

Func<int, int> kwadrat = x => x * x;
Console.WriteLine(kwadrat(5));

```

```cs
Func<String, int, double> fun = (x_string, x_int) => {
    Console.WriteLine(x_string.GetType());
    Console.WriteLine(x_int.GetType());
    return 34.43;
};
Console.WriteLine(fun("napis", 11));

```

Aby utworzyć wyrażenie lambda, określasz parametry wejściowe (jeśli istnieją) po lewej stronie operatora lambda i wyrażenie lub blok instrukcji po drugiej stronie.

Każde wyrażenie lambda może zostać przekonwertowane na typ delegate. Typ delegate, na który może zostać przekonwertowane wyrażenie lambda jest określony przez typy jego parametrów i wartości zwracane. Jeżeli wyrażenie lambda nie zwraca wartości, to może zostać przekonwertowane na jeden z typów delegate Action; w przeciwnym razie może zostać przekonwertowane na jeden z typów delegate Func. 

```cs

Func<String, int, double> func_test;

func_test = (x, y) => 3.4;
Console.WriteLine(func_test("a",1));

func_test = (x, y) => 
{
    double z = double.Parse(x) * (double)y;
    return z;
};

Console.WriteLine(func_test("2",3));

Action<double, int, String> actionTest;

// akcje nie zwracają wartości
actionTest = (x,y,z) => Console.WriteLine(x);
actionTest(1.1,1,"1");

actionTest = (x,y,z) => 
{
    Console.WriteLine(x);
    Console.WriteLine(y);  
    Console.WriteLine(z);
};
actionTest(1.11,1,"1.111");

```

Parametry wejściowe wyrażenia lambda są silnie typowane w czasie kompilacji. Kiedy kompilator może wywnioskować typy parametrów wejściowych, jak w poprzednim przykładzie, możesz pominąć deklaracje typu. Jeśli musisz określić typ parametrów wejściowych, musisz to zrobić dla każdego parametru.

```cs
Func<double, double> kwadrat = x => x * x;
var kwadrat_2 = (double x, int y, String z) => x * x;
//dodatkowe wskazanie zwracanego typu przez lambda
var kwadrat_3 = object (double x) => x * x;

Console.WriteLine(kwadrat(4));
Console.WriteLine(kwadrat_2(4,6,"jj"));
Console.WriteLine(kwadrat_3(4));
```

Natomiast poniższa deklaracja jest błędna, ponieważ nie można wywnioskować typu delegowania (error CS8917).

```cs

var kwadrat_4 = (x) => x * x; //BŁĄD!

```

Poniższy kod liczy pierwiastek ze wskazanej liczby double jeśli nie jest ona ujemna. Jeśli jest ujemna, zwraca null.

```cs

var pierwiastek = object (double x) => x >= 0 ? Math.Sqrt(x) : null; 
Console.WriteLine(pierwiastek(-4));
```


Funkcja zdefiniowana przy pomocy lambda może mieć oczywiście więcej niż jeden parametr:

```cs

Func<double, double, double> pomnoz = (x, y) => x * y;
Console.WriteLine(pomnoz(-4, 4));

```

Funkcje zdefiniowane przy pomocy wyrażeń lambda mogą mieć wiele linijek jak również mogą deklarować zmienne lokalne.

```cs

Func<double, double, double> pomnoz = (x, y) => 
{
    double z = x * y;
    return z;
};

Console.WriteLine(pomnoz(3,4));

```

W przeciwieństwie do delegata Func, delegat Action może mieć tylko parametry wejściowe. Delegata Action używa się wtedy, gdy nie potrzebujesz zwracać żadnej wartości z wyrażenia lambda. 

```cs

Action<Pracownik> WypiszPracownika = s => Console.WriteLine("Imie: {0}, nazwisko: {1} ", s.imie, s.nazwisko);
Pracownik prac = new Pracownik(){ imie = "Bill", nazwisko = "Kill"};
WypiszPracownika(prac);
class Pracownik
{
    public String imie;
    public String nazwisko;
}

```

Bardzo częstym miejscem zastosowania wyrażeń lambda są kwerendy LINQ.

## LINQ

LINQ to zestaw konstrukcji programistycznych, które pozwalają na używanie zapytań na obiektach języka.

Składnia kwerendy (query syntax):

```cs

using System.Text.Json;
String jsonString = System.IO.File.ReadAllText("D:\\test\\data\\people.json");
Persons ?persons = JsonSerializer.Deserialize<Persons>(jsonString);

var results = from p in persons.data
            where p.city == "Port Reaganfort" && p.mac == "c5:32:09:5a:f7:15"
            orderby p.name
            select new {S = p};
foreach (var p in results.ToList())
    Console.WriteLine(p.S.ToString());

public class Person
{
    public String ?name {get; set;}
    public String ?email {get; set;}
    public String ?city {get; set;}
    public String ?mac {get; set;}
    public String ?timestamp {get; set;}
    public String ?creditcard {get; set;}
    public override String ToString(){
        return name + " " + " " + email + " " + city + " " + mac + " " + timestamp + " " + creditcard;
    }
}

public class Persons
{
    public List<Person> data { get; set; }
} 


```

Składnia LINQ może być napisana przy użyciu deklaratywnej składni zapytań LINQ. Składnia zapytań LINQ finalnie musi być przetłumaczona na wywołania metod dla .NET Common Language Runtime (CLR), kiedy kod jest kompilowany. Te wywołania metod wywołują standardowe operatory zapytań, które mają nazwy takie jak Where, Select, GroupBy, Join, Max i Average. Metody te można wywołać bezpośrednio, używając składni metody zamiast składni zapytania.


Składnia metody (method syntax) dla poprzedniego zapytania LINQ jest postaci:

```cs

var results2 = persons.data.Where(p => p.city == "Port Reaganfort" && p.mac == "c5:32:09:5a:f7:15")
    .OrderBy(p => p.name);
foreach (var p in results2.ToList())
    Console.WriteLine(p.ToString());


```

## Podkwerendy

W linq można tworzyć złożone kwerendy, również zagnieżdżone. Załóżmy, że mamy następującą strukturę danych, odpowiadającą powiązaniom relacyjnym, w których każdy pracownik pracuje na jakimś etacie oraz w jakimś zespole oraz każdy pracownik może być szefem lub / i podwładnym innego pracownika.

```cs

public class Pracownik 
{
    public int id {get; set;}
    public String imie {get; set;}
    public String nazwisko {get; set;}
    public DateTime rokUrodzenia {get; set;}
    public int idEtatu {get; set;}
    public int idZespolu {get; set;}
    public double placa {get; set;}
}

public class Zespol
{
    public int id {get; set; }
    public String nazwa {get; set; }
}

public class Etat
{
    public int id {get; set; }
    public String nazwa {get; set; }
    public double placaMin {get; set;}

    public double placaMax {get; set;}
}

public class Hierarchia
{
    public int id  {get; set; }
    public int idSzefa  {get; set; }
    public int idPodwladnego  {get; set; }
}

```

Stwórzmy przykładowe dane:

```cs

List<Pracownik>pracownicy = new List<Pracownik>
{ new Pracownik(){
    id = 1,
    imie = "Jan",
    nazwisko = "Kowalski",
    rokUrodzenia = DateTime.Parse("10-10-1990"),
    idEtatu = 1,
    idZespolu = 1,
    placa = 10000
    },
    new Pracownik(){
    id = 2,
    imie = "Marcin",
    nazwisko = "Nowak",
    rokUrodzenia = DateTime.Parse("10-10-1990"),
    idEtatu = 1,
    idZespolu = 2,
    placa = 8000
    },
    new Pracownik(){
    id = 3,
    imie = "Jan",
    nazwisko = "Figo",
    rokUrodzenia = DateTime.Parse("10-10-1990"),
    idEtatu = 2,
    idZespolu = 1,
    placa = 7000
    },
    new Pracownik(){
    id = 4,
    imie = "Grzegorz",
    nazwisko = "Masztalski",
    rokUrodzenia = DateTime.Parse("10-10-1990"),
    idEtatu = 2,
    idZespolu = 2,
    placa = 6000
    }
};


List<Zespol>zespoly = new List<Zespol>
{
    new Zespol(){id = 1, nazwa = "Z1"},
    new Zespol(){id = 2, nazwa = "Z2"}
};

List<Etat>etaty = new List<Etat>
{
    new Etat(){id = 1, nazwa = "E1", placaMin = 5000, placaMax = 9000},
    new Etat(){id = 2, nazwa = "E2", placaMin = 4000, placaMax = 8000}
};

List<Hierarchia>hierarchia = new List<Hierarchia>
{
    new Hierarchia(){id=1, idPodwladnego=2, idSzefa=1},
    new Hierarchia(){id=2, idPodwladnego=3, idSzefa=1},
    new Hierarchia(){id=3, idPodwladnego=4, idSzefa=1},
    new Hierarchia(){id=4, idPodwladnego=4, idSzefa=2},
};


```

Kwerenda wybierająca nazwisko oraz płacę pracownika, który zarabia najwięcej.

```cs

var results = from p in pracownicy
    where p.placa == (from p2 in pracownicy
    select p2.placa).Max()
    select new {nazwisko = p.nazwisko, placa = p.placa};

foreach (var p in results.ToList())
    Console.WriteLine(p.nazwisko + " " + p.placa);

//składnia metody 

var results2 = pracownicy.
Where(p => p.placa == (pracownicy.Max(p2 => p2.placa)))
.Select(p => new {p.nazwisko, p.placa});
```

## Złączenia

Kwerenda wybierająca nazwisko pracownika, jego nazwę zespołu, nazwę etatu oraz nazwisko szefa.

```cs
var results = from p in pracownicy
    join e in etaty on p.idEtatu equals e.id
    join z in zespoly on p.idZespolu equals z.id
    join h in hierarchia on p.id equals h.idPodwladnego
    join p2 in pracownicy on h.idSzefa equals p2.id
    select new {nazwisko = p.nazwisko, 
        etat = e.nazwa,
        zespol = z.nazwa,
        szef = p2.nazwisko};

foreach (var p in results.ToList())
    Console.WriteLine(p.nazwisko + " " 
        + p.etat + " " + p.zespol + " " + p.szef);


//składnia metody

var results2 = pracownicy
    .Join(etaty, 
    p => p.idEtatu,
    e => e.id,
    (p, e) => new {nazwisko = p.nazwisko, etat = e.nazwa,
        id_zespolu = p.idZespolu, id_prac = p.id})
    .Join(zespoly, 
    p => p.id_zespolu,
    z => z.id,
    (p, z) => new {nazwisko = p.nazwisko, 
        etat = p.etat, zespol = z.nazwa, id_prac = p.id_prac})
    .Join(hierarchia,
    p => p.id_prac,
    h => h.idPodwladnego,
    (p, h) => new {nazwisko = p.nazwisko, 
        etat = p.etat, zespol = p.zespol, id_szefa = h.idSzefa})
    .Join(pracownicy,
    p => p.id_szefa,
    p2 => p2.id,
    (p, p2) => new {nazwisko = p.nazwisko, 
        etat = p.etat, zespol = p.zespol, szef = p2.nazwisko});


        
```

Złączenie grupowe (groupjoin) jest przydatne do tworzenia hierarchicznych struktur danych. Paruje ono każdy element z pierwszego zbioru ze zbiorem skorelowanych elementów z drugiego zbioru.

Każdy element z pierwszego zbioru pojawia się w zbiorze wyników złączenia grupowego niezależnie od tego, czy skorelowane elementy znajdują się w drugim zbiorze. W przypadku, gdy nie zostaną znalezione żadne skorelowane elementy, sekwencja skorelowanych elementów dla tego elementu jest pusta. Selektor wyników ma więc dostęp do każdego elementu pierwszej kolekcji. Różni się to od selektora wyników w nie-grupowym złączeniu, który nie ma dostępu do elementów z pierwszej kolekcji, które nie mają odpowiednika w drugiej kolekcji.

GroupJoin nie ma bezpośredniego odpowiednika w tradycyjnych terminach relacyjnych baz danych.

```cs

etaty.Add(new Etat(){id = 3, nazwa = "E3", placaMin = 5000, placaMax = 9000});

var results = etaty.GroupJoin(pracownicy,
e => e.id,
p => p.idEtatu,
(e, p) =>
    new
    {
    etat = e,
    prac = p.Select(pr => pr)
    });

foreach (var et in results)
{
    Console.WriteLine("{0}:", et.etat.nazwa);
    foreach (var pr in et.prac)
    {
        Console.WriteLine("  {0}", pr.nazwisko);
    }
}

//wersja LINQ

var results2 = from p2 in (from e in etaty
    join p in pracownicy on e.id equals p.idEtatu
    select new {prac = p, etat = e})
    group p2 by p2.etat.nazwa into zgrupowane
    select (zgrupowane);

foreach (var et in results2)
{
    Console.WriteLine("{0}:", et.Key);
    foreach (var pr in et)
    {
        Console.WriteLine("  {0}", pr.prac.nazwisko);
        Console.WriteLine("  {0}", pr.etat.nazwa);
    }
}


```


SelectMany jest operacją "spłaszczającą" elementy typu IEnumerable zawierające elementy IEnumerable do pojedynczego elementu IEnumerable.

```cs

etaty.Add(new Etat(){id = 3, nazwa = "E3", placaMin = 5000, placaMax = 9000});


var results = etaty.GroupJoin(pracownicy,
    e => e.id,
    p => p.idEtatu,
    (e, p) =>
        new
        {
        etat = e.nazwa,
        prac = p.Select(pr => pr)
        });

foreach (var et in results)
{
    Console.WriteLine("{0}:", et.etat);
    foreach (var pr in et.prac)
    {
        Console.WriteLine("  {0}", pr.nazwisko);
    }
}


var flat = results.SelectMany(
    p => p.prac,
    (p1, p2) =>  new {etat = p1.etat, nazwisko = p2.nazwisko}
);

foreach (var p in flat)
{
    Console.WriteLine("etat: {0}, nazwisko: {1}", p.etat, p.nazwisko);
}

```



Kwerenda stosująca złączenie zewnętrzne (left outer join):


```cs

var results = from p in pracownicy
    join e in etaty on p.idEtatu equals e.id
    join z in zespoly on p.idZespolu equals z.id
    join h in hierarchia on p.id equals h.idPodwladnego into hierarchia_oj
    from h_o in hierarchia_oj.DefaultIfEmpty()
    join p2 in pracownicy on (h_o == null ? 0 : h_o.idSzefa) equals p2.id into pracownicy_oj
    from p_o in pracownicy_oj.DefaultIfEmpty()
    select new {nazwisko = p.nazwisko, 
        etat = e.nazwa,
        zespol = z.nazwa,
        szef = p_o?.nazwisko ?? "Brak szefa"
        };


foreach (var p in results.ToList())
    Console.WriteLine(p.nazwisko + " " 
        + p.etat + " " + p.zespol + " "
        + p.szef
        );

//składnia metody

var results2 = pracownicy
    .Join(etaty, 
    p => p.idEtatu,
    e => e.id,
    (p, e) => new {nazwisko = p.nazwisko, etat = e.nazwa,
        id_zespolu = p.idZespolu, id_prac = p.id})
    .Join(zespoly, 
    p => p.id_zespolu,
    z => z.id,
    (p, z) => new {nazwisko = p.nazwisko, 
        etat = p.etat, zespol = z.nazwa, id_prac = p.id_prac})
    .LeftOuterJoin(hierarchia,
    p => p.id_prac,
    h => h.idPodwladnego,
    (p, h) => new {nazwisko = p.nazwisko, 
        etat = p.etat, zespol = p.zespol, id_szefa = h?.idSzefa})
    .LeftOuterJoin(pracownicy,
    p => p == null ? 0 : p.id_szefa,// p.id_szefa,
    p2 => p2.id,
    (p, p2) => new {nazwisko = p.nazwisko, 
        etat = p.etat, zespol = p.zespol, szef = p2?.nazwisko ?? "Brak szefa"});

foreach (var p in results2.ToList())
    Console.WriteLine(p.nazwisko + " " 
        + p.etat + " " + p.zespol + " "
        + p.szef
        );

public static class LinqExt
{
    public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey,
        Func<TLeft, TRight, TResult> result)
    {
        return left.GroupJoin(right, leftKey, rightKey, (l, r) => new { l, r })
             .SelectMany(
                 o => o.r.DefaultIfEmpty(),
                 (l, r) => new { lft= l.l, rght = r })
             .Select(o => result.Invoke(o.lft, o.rght));//bez Invoke. też zadziała
    }
}


```

Kwerenda, która dla każdego szefa znajduje podwładnych, którzy zarabiają najmniej spośród jego podwładnych.

```cs


var results = from p in pracownicy
    join h in hierarchia on p.id equals h.idSzefa
    join p2 in pracownicy on h.idPodwladnego equals p2.id
    where p2.placa == 
    (from p3 in pracownicy
    join h2 in hierarchia on p3.id equals h2.idSzefa
    join p4 in pracownicy on h2.idPodwladnego equals p4.id
    where p.id == p3.id
    select (p4.placa)).Min()
    select new {nazwisko = p.nazwisko, 
        szef = p2.nazwisko,
        min_placa = p2.placa};

foreach (var p in results.ToList())
    Console.WriteLine(p.nazwisko + " " 
        + p.szef + " " + p.min_placa
        );

```

## Agregacje

Kwerenda agregująca pojedynczą kolekcję po wartości pola - podaj zespoły oraz nazwiska osób pracujących w tych zespołach:

```cs

var results = from p in pracownicy
    group p by p.idZespolu into zgrupowane
    select (zgrupowane);

foreach (var grupa_prac in results)
{
    Console.WriteLine($"Klucz: {grupa_prac.Key}");
    // wewnętrzna pętla foreach dla każdego klucza
    foreach (var prac in grupa_prac)
    {
        Console.WriteLine($"\t{prac.imie}, {prac.nazwisko}");
    }
}

```

Kwerenda agregująca wielokrotną kolekcję po wartości pola i licząca statystyki - podaj nazwy zespołów oraz średnią płacę, którą mają pracownicy pracujący w tych zespołach:

```cs

var results = from p2 in (from p in pracownicy
    join z in zespoly on p.idZespolu equals z.id
    select new {placa = p.placa, nazwa = z.nazwa}) 
    group p2 by p2.nazwa into zgrupowane
    select new {nazwa = zgrupowane.Key, 
        avg = zgrupowane.Average(zgr => zgr.placa)};

foreach (var res in results)
{
    Console.WriteLine(res.nazwa + " " + res.avg);
}

```

lub 

```cs

var res5 = from a in (from e in etaty
        join p in pracownicy on e.id equals p.idEtatu
        select new {e,p})
        group a by a.e.id into z
        select new {z, avg = z.Average(t => t.p.placa)};

foreach (var z in res5)
{
    Console.WriteLine(z.z.Key + " " + z.avg);
    foreach (var z2 in z.z)
    {
        Console.WriteLine("\t" + z2.e.nazwa);
        Console.WriteLine("\t" + z2.p.nazwisko);
    }
}

```

## Literatura 

[Wyrażenia lambda i funkcje anonimowe (learn.microsoft)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions)

[Metody anonimowe (tutorialsteacher)](https://www.tutorialsteacher.com/csharp/csharp-anonymous-method)

[Wyrażenia lambda (tutorialsteacher)](https://www.tutorialsteacher.com/linq/linq-lambda-expression)

[Praca z LINQ (learn.microsoft)](https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/working-with-linq)

[Praca z LINQ 2 (learn.microsoft)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/query-syntax-and-method-syntax-in-linq)

[Praca z LINQ 3 (learn.microsoft)](https://learn.microsoft.com/en-us/dotnet/csharp/linq/write-linq-queries)

[JOIN w linq (tutorialsteacher)](https://www.tutorialsteacher.com/linq/linq-joining-operator-groupjoin)

[Kwerendy grupujące w linq (learn.microsoft)](https://learn.microsoft.com/pl-pl/dotnet/csharp/linq/group-query-results)

