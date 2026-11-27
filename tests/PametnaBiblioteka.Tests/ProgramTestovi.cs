using Xunit;
namespace PametnaBiblioteka.Tests;
public class ProgramTestovi
{
    private static string Pokreni(string unos, bool prekoMain = false)
    {
        var ulaz = new StringReader(unos);
        var izlaz = new StringWriter();
        var stariUlaz = Console.In;
        var stariIzlaz = Console.Out;
        try
        {
            Console.SetIn(ulaz);
            Console.SetOut(izlaz);
            if (prekoMain)
                Program.Main();
            else
                Program.Pokreni(ulaz, izlaz);
        }
        finally { Console.SetIn(stariUlaz); Console.SetOut(stariIzlaz); }
        return izlaz.ToString();
    }
    [Fact] public void MainOmogucavaIzlaz() => Assert.Contains("PAMETNA BIBLIOTEKA", Pokreni("0\n", true));
    [Fact] public void KrajUlazaZatvaraMeni() => Assert.Contains("PAMETNA BIBLIOTEKA", Pokreni(""));
    [Fact] public void PrekidUnosaUsredOperacijeNeRusiProgram() => Assert.DoesNotContain("registrovan", Pokreni("1\n"));
    [Fact]
    public void NepoznataOpcijaINeispravanBrojSeObrade()
    {
        var izlaz = Pokreni("99\n1\nabc\n0\n");
        Assert.Contains("Nepoznata opcija", izlaz);
        Assert.Contains("Unesite cijeli broj", izlaz);
    }
    [Fact]
    public void NeispravnaUlogaSeOdbijaUMeniju()
    {
        var izlaz = Pokreni("1\n1\nAna\nAnić\na@example.com\n9\n0\n");
        Assert.Contains("Uloga mora", izlaz);
        Assert.DoesNotContain("Korisnik registrovan.", izlaz);
    }
    [Fact]
    public void NepostojeciKorisnikSeObradi()
    {
        var izlaz = Pokreni("5\n99\n1\n0\n");
        Assert.Contains("Korisnik nije pronadjen", izlaz);
    }
    [Fact]
    public void PraznePreporukeIspisujuPoruku()
    {
        var izlaz = Pokreni("1\n1\nAna\nAnić\na@example.com\n1\n8\n1\n5\n0\n");
        Assert.Contains("Nema dostupnih", izlaz);
    }
    [Fact]
    public void CijeliTokKorisnikKnjigePosudbaPreporukaIIzmjene()
    {
        var izlaz = Pokreni(string.Join("\n", new[]{
            "1","1","Faris","Šišić","f@example.com","1",
            "2", "3","1","Prva","Autor","Roman","2",
            "3","2","Druga","Autor","Roman","1",
            "4","Prva", "14","1", "14","99",
            "7","1", "5","1","1", "7","1", "6","1","1", "8","1","5",
            "9","1","Amir","Hadžić","amir@example.com", "11","1","2", "2",
            "12","2","Druga nova","Autor novi","Drama","3", "13","2", "10","1", "0",""}));
        Assert.Contains("Bodovi: 5", izlaz);
        Assert.Contains("Historija citanja", izlaz);
        Assert.Contains("Dostupna.", izlaz);
        Assert.Contains("Nije dostupna.", izlaz);
        Assert.Contains("Korisnik azuriran.", izlaz);
        Assert.Contains("Uloga promijenjena.", izlaz);
        Assert.Contains("Korisnik obrisan.", izlaz);
        Assert.Contains("Druga", izlaz);
        Assert.DoesNotContain("Greska:", izlaz);
    }
}
