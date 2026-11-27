using Xunit;
namespace PametnaBiblioteka.Tests;
public class PodsjetnikTestovi
{
    private static (Okruzenje o, LazniPosiljalac lazni, PodsjetnikServis servis) Pripremi()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1, "Na Drini cuprija");
        o.Posudbe.PosudiKnjigu(1, 1);
        var f = new LazniPosiljalac();
        return (o, f, new PodsjetnikServis(o.Korisnici, o.Knjige, o.Posudbe, f));
    }
    [Theory]
    [InlineData(10, 0)]
    [InlineData(11, 1)]
    [InlineData(14, 1)]
    [InlineData(15, 1)]
    public void PodsjetnikSeSaljeSamoUnutarProzoraIliZaKasnjenje(int proteklo, int ocekivano)
    {
        var (o, f, s) = Pripremi();
        Assert.Equal(ocekivano, s.PosaljiPodsjetnike(o.Dan.AddDays(proteklo)));
        Assert.Equal(ocekivano, f.Poruke.Count);
    }
    [Fact]
    public void ZamjenskiPosiljalacBiljeziTacnogPrimaocaNaslovISadrzaj()
    {
        var (o, f, s) = Pripremi();
        s.PosaljiPodsjetnike(o.Dan.AddDays(11));
        var poruka = Assert.Single(f.Poruke);
        Assert.Equal("korisnik1@example.com", poruka.Primalac);
        Assert.Equal("Podsjetnik za vracanje knjige", poruka.Naslov);
        Assert.Contains("Faris", poruka.Tekst);
        Assert.Contains("Na Drini cuprija", poruka.Tekst);
        Assert.Contains("15.11.2026", poruka.Tekst);
    }
    [Fact]
    public void ZakasnjeliPodsjetnikImaOdgovarajuciNaslov()
    {
        var (o, f, s) = Pripremi();
        s.PosaljiPodsjetnike(o.Dan.AddDays(15));
        Assert.Equal("Istekao rok vracanja", Assert.Single(f.Poruke).Naslov);
    }
    [Fact]
    public void VracenaKnjigaNeDobijaPodsjetnik()
    {
        var (o, f, s) = Pripremi();
        o.Posudbe.VratiKnjigu(1, 1);
        Assert.Equal(0, s.PosaljiPodsjetnike(o.Dan.AddDays(14)));
        Assert.Empty(f.Poruke);
    }
    [Fact]
    public void IstiDanNemaDuplikataSljedeciDanMoze()
    {
        var (o, f, s) = Pripremi();
        var dan = o.Dan.AddDays(14);
        Assert.Equal(1, s.PosaljiPodsjetnike(dan));
        Assert.Equal(0, s.PosaljiPodsjetnike(dan.AddHours(5)));
        Assert.Equal(1, s.PosaljiPodsjetnike(dan.AddDays(1)));
        Assert.Equal(2, f.Poruke.Count);
    }
    [Fact]
    public void NeuspjeloSlanjeDozvoljavaPonovniPokusaj()
    {
        var (o, f, s) = Pripremi();
        f.SimulirajGresku = true;
        Assert.Throws<IOException>(() => s.PosaljiPodsjetnike(o.Dan.AddDays(14)));
        Assert.Empty(f.Poruke);
        f.SimulirajGresku = false;
        Assert.Equal(1, s.PosaljiPodsjetnike(o.Dan.AddDays(14)));
    }
    [Fact]
    public void ViseAktivnihPosudbiDajeOdvojenePoruke()
    {
        var (o, f, s) = Pripremi();
        o.Korisnik(2);
        o.Posudbe.PosudiKnjigu(2, 1);
        Assert.Equal(2, s.PosaljiPodsjetnike(o.Dan.AddDays(14)));
        Assert.Equal(new[] { "korisnik1@example.com", "korisnik2@example.com" }, f.Poruke.Select(p => p.Primalac));
    }
    [Fact]
    public void NegativanProzorSeOdbija()
    {
        var (o, _, s) = Pripremi();
        Assert.Throws<ArgumentOutOfRangeException>(() => s.PosaljiPodsjetnike(o.Dan, -1));
    }
    [Fact]
    public void NedostajuciKorisnikIliKnjigaPrijavljujuGresku()
    {
        var (o, _, s) = Pripremi();
        var p = o.Posudbe.DajSvePosudbe()[0];
        p.KorisnikId = 99;
        Assert.Throws<KeyNotFoundException>(() => s.PosaljiPodsjetnike(o.Dan.AddDays(14)));
        p.KorisnikId = 1;
        o.Knjige.knjige.Clear();
        Assert.Throws<KeyNotFoundException>(() => s.PosaljiPodsjetnike(o.Dan.AddDays(14)));
    }
    [Fact]
    public void NullZavisnostiSeOdbijaju()
    {
        var (o, f, _) = Pripremi();
        Assert.Throws<ArgumentNullException>(() => new PodsjetnikServis(null!, o.Knjige, o.Posudbe, f));
        Assert.Throws<ArgumentNullException>(() => new PodsjetnikServis(o.Korisnici, null!, o.Posudbe, f));
        Assert.Throws<ArgumentNullException>(() => new PodsjetnikServis(o.Korisnici, o.Knjige, null!, f));
        Assert.Throws<ArgumentNullException>(() => new PodsjetnikServis(o.Korisnici, o.Knjige, o.Posudbe, null!));
    }
}
