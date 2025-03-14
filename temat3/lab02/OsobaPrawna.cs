// #nullable disable
namespace Lab02;


public class OsobaPrawna : PosiadaczRachunku
{
    private string? nazwa;
    private string? siedziba;

    public OsobaPrawna(string? nazwa, string? siedziba)
    {
        this.nazwa = nazwa;
        this.siedziba = siedziba;
    }

    public override string ToString()
    {
        return $"Osoba prawna: {nazwa} {siedziba}";
    }

    public string? Nazwa
    {
        get => nazwa;
    }

    public string? Siedziba
    {
        get => siedziba;
    }
}