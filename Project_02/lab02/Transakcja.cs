// #nullable disable
namespace Lab02;


public class Transakcja
{
    private RachunekBankowy? rachunekZrodlowy;
    private RachunekBankowy? rachunekDocelowy;
    private decimal kwota;
    private string? opis;

    public Transakcja(RachunekBankowy? rachunekZrodlowy, RachunekBankowy? rachunekDocelowy, decimal kwota, string? opis)
    {
        if (rachunekZrodlowy == null && rachunekDocelowy == null)
        {
            throw new Exception("Nie podano rachunku zrodlowego lub docelowego");
        }
        RachunekZrodlowy = rachunekZrodlowy;
        RachunekDocelowy = rachunekDocelowy;
        Kwota = kwota;
        Opis = opis;
    }

    public RachunekBankowy? RachunekZrodlowy
    {
        get => rachunekZrodlowy;
        set
        {
            if (value == null)
            {
                throw new Exception("Nie podano rachunku zrodlowego");
            }
            rachunekZrodlowy = value;
        }
    }

    public RachunekBankowy? RachunekDocelowy
    {
        get => rachunekDocelowy;
        set
        {
            if (value == null)
            {
                throw new Exception("Nie podano rachunku docelowego");
            }
            rachunekDocelowy = value;
        }
    }
    
    
    public decimal Kwota
    {
        get => kwota;
        set => kwota = value;
    }

    public string? Opis
    {
        get => opis;
        set => opis = value;   
    }

    
    // przeciazenie? nie chodzilo o override?
    public override string ToString()
    {
        string nrZrodlowy = rachunekZrodlowy?.Numer ?? "BRAK (wpłata)";
        string nrDocelowy = rachunekDocelowy?.Numer ?? "BRAK (wypłata)";
        return $"Numer rachunku zrodlowego: {nrZrodlowy} " +
               $"\nNumer rachunku docelowego: {nrDocelowy} " +
               $"\nKwota: {this.kwota} " +
               $"\nOpis: {this.opis}";
    }
    
    
}
