#nullable disable
namespace Lab02;

public class OsobaFizyczna : PosiadaczRachunku
{
    private string imie;
    private string nazwisko;
    private string drugieImie;
    private string PESEL;
    private string numerPaszportu;

    public OsobaFizyczna(string imie, string nazwisko, string drugieImie, string PESEL, string numerPaszportu)
    {
        if (string.IsNullOrEmpty(PESEL) && string.IsNullOrEmpty(numerPaszportu))
        {
            throw new Exception("PESEL albo numer paszportu muszą być nie null");
        }

        if (!string.IsNullOrEmpty(PESEL))
        {
            if (PESEL.Length != 11 || !long.TryParse(PESEL, out _) || PESEL == null)
            {
                throw new Exception("Niepoprawny PESEL");
            }
        }

        this.imie = imie;
        this.nazwisko = nazwisko;
        this.drugieImie = drugieImie;
        this.PESEL = PESEL;
        this.numerPaszportu = numerPaszportu;
    }

    public override string ToString()
    {
        return $"Osoba fizyczna: {imie} {nazwisko}";
    }
    
    
    public string Imie
    {
        get => imie;
        set => imie = value;
    }
    
    public string Nazwisko
    {
        get => nazwisko;
        set => nazwisko = value;
    }

    public string DrugieImie
    {
        get => drugieImie;
        set => drugieImie = value;
    }

    public string Pesel
    {
        get => PESEL;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(numerPaszportu))
                {
                    throw new Exception("PESEL albo numer paszportu muszą być nie null");
                }
            }
            else
            {
                if (value.Length != 11)
                {
                    throw new Exception("PESEL musi miec dokladnie 11 cyfr");
                }
                if (!long.TryParse(value, out _))
                {
                    throw new Exception("PESEL musi skladac sie wylacznie z cyfr");
                }
            }
            PESEL = value;
        }
    }

    public string NumerPaszportu
    {
        get => numerPaszportu;
        set
        {
            if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(PESEL))
            {
                throw new Exception("PESEL albo numer paszportu muszą być nie null");
            }
            numerPaszportu = value;
        }
    }
    
    
}