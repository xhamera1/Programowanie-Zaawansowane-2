#nullable disable
namespace Lab02;


public class RachunekBankowy
{
    private string numer;
    private decimal stanRachunku;
    private bool czyDozwolonyDebet;
    private List<PosiadaczRachunku> posiadaczeRachunku;
    private List<Transakcja> _Transakcje = new List<Transakcja>();

    public RachunekBankowy(string numer, decimal stanRachunku, bool czyDozwolonyDebet,
        List<PosiadaczRachunku> posiadaczeRachunku)
    {
        if (posiadaczeRachunku == null || posiadaczeRachunku.Count == 0)
        {
            throw new Exception("Brak posiadaczy rachunku");
        }
        this.numer = numer;
        this.stanRachunku = stanRachunku;
        this.czyDozwolonyDebet = czyDozwolonyDebet;
        this.posiadaczeRachunku = new List<PosiadaczRachunku>(posiadaczeRachunku);
    }

    public static void DokonajTransakcji(RachunekBankowy rachunekZrodlowy, RachunekBankowy rachunekDocelowy,
        decimal kwota, string opis)
    {
        if (kwota < 0)
        {
            throw new Exception("Kwota nie moze byc ujemna");
        }

        if (rachunekDocelowy == null && rachunekZrodlowy == null)
        {
            throw new Exception("Brak rachunkow docelowych i zrodlowych");
        }

        if (rachunekZrodlowy != null && rachunekZrodlowy.CzyDozwolonyDebet == false && kwota > rachunekZrodlowy.StanRachunku)
        {
            throw new Exception("Rachunek źródłowy nie pozwala na debet, a kwota transakcji przekroczy stanRachunku");
        }

        Transakcja nowa_transakcja;
        // wpłata gotówki
        if (rachunekZrodlowy == null)
        {
            rachunekDocelowy.StanRachunku += kwota;
            nowa_transakcja = new Transakcja(rachunekZrodlowy, rachunekDocelowy, kwota, opis);
            rachunekDocelowy._Transakcje.Add(nowa_transakcja);
            return;
        }

        // wyplata gotowki
        if (rachunekDocelowy == null)
        {
            rachunekZrodlowy.StanRachunku -= kwota;
            nowa_transakcja = new Transakcja(rachunekZrodlowy, rachunekDocelowy, kwota, opis);
            rachunekZrodlowy._Transakcje.Add(nowa_transakcja);
            return;
        }
        
        //przelew
        rachunekDocelowy.StanRachunku += kwota;
        rachunekZrodlowy.StanRachunku -= kwota;
        nowa_transakcja = new Transakcja(rachunekZrodlowy, rachunekDocelowy, kwota, opis);
        rachunekZrodlowy._Transakcje.Add(nowa_transakcja);
        rachunekDocelowy._Transakcje.Add(nowa_transakcja);
        
    }

    public static RachunekBankowy operator +(RachunekBankowy rachunekBankowy, PosiadaczRachunku posiadaczRachunku) 
    {
        if (rachunekBankowy.posiadaczeRachunku.Contains(posiadaczRachunku))
        {
            throw new Exception("Posiadacz juz jest w w liscie posiadaczy rachunku");
        }
        rachunekBankowy.posiadaczeRachunku.Add(posiadaczRachunku);
        return rachunekBankowy;
    }

    public static RachunekBankowy operator -(RachunekBankowy rachunekBankowy, PosiadaczRachunku posiadaczRachunku)
    {
        if (!rachunekBankowy.posiadaczeRachunku.Contains(posiadaczRachunku))
        {
            throw new Exception("Posiadacz nie jest w liscie posiadaczy rachunku");
        }
        if (rachunekBankowy.posiadaczeRachunku.Count == 1)
        {
            throw new Exception("Nie mozna usunac posiadacza z rachunku, poniewaz jest jedynym posiadaczem");
        }
        rachunekBankowy.posiadaczeRachunku.Remove(posiadaczRachunku);
        return rachunekBankowy;
    }

    public override string ToString()
    {
        string posiadaczeStr = string.Join("\n", posiadaczeRachunku);
        string transakcjeStr;
        if (_Transakcje.Count > 0)
        {
            transakcjeStr = string.Join("\n", _Transakcje);
        }
        else
        {
            transakcjeStr = "Brak transakcji";
        }
    
        return $"Numer rachunku: {numer}\n" +
               $"Stan rachunku: {stanRachunku}\n" +
               $"Wszyscy posiadacze rachunku:\n{posiadaczeStr}\n" +
               $"Wszystkie transakcje:\n{transakcjeStr}";
    }


    public string Numer
    {
        get => numer;
        set => numer = value;

    }

    public decimal StanRachunku
    {
        get => stanRachunku;
        set => stanRachunku = value;
    }

    public bool CzyDozwolonyDebet
    {
        get => czyDozwolonyDebet;
        set => czyDozwolonyDebet = value;
    }

    public List<PosiadaczRachunku> PosiadaczeRachunku
    {
        get => posiadaczeRachunku;
        set => posiadaczeRachunku = value;
    }
    
}