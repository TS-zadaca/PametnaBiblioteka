using Xunit;
namespace PametnaBiblioteka.Tests;
public class PreporukaTestovi
{
    [Fact]
    public void IstiZanrIAutorDajuPetBodova()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1, "Procitana", "Autor A", "Roman");
        o.Knjiga(2, "Oboje", "Autor A", "Roman");
        o.Knjiga(3, "Zanr", "Autor B", "Roman");
        o.Knjiga(4, "Autor", "Autor A", "Drama");
        o.Knjiga(5, "Bez", "Autor C", "Poezija");
        o.Procitaj(1);
        var p = o.Preporuke.PreporuciKnjige(1);
        Assert.Equal(new[] { 2, 3, 4, 5 }, p.Select(x => x.Knjiga.Id));
        Assert.Equal(new[] { 5, 3, 2, 0 }, p.Select(x => x.Bodovi));
        Assert.Contains("Historija citanja", p[0].Razlog);
    }
    [Fact]
    public void VecCitaneAktivneINedostupneSeIskljucuju()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1);
        o.Knjiga(2);
        var nedostupna = o.Knjiga(3);
        o.Knjiga(4);
        o.Procitaj(1);
        o.Posudbe.PosudiKnjigu(1, 2);
        nedostupna.DostupnoPrimjeraka = 0;
        Assert.Equal(4, Assert.Single(o.Preporuke.PreporuciKnjige(1)).Knjiga.Id);
    }
    [Fact]
    public void BezHistorijePrednostImaPopularnaKnjiga()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Korisnik(2);
        o.Knjiga(1, "A naslov");
        o.Knjiga(2, "Z naslov");
        o.Procitaj(2, 2);
        var p = o.Preporuke.PreporuciKnjige(1);
        Assert.Equal(2, p[0].Knjiga.Id);
        Assert.Equal(0, p[0].Bodovi);
        Assert.Contains("popularnosti", p[0].Razlog);
    }
    [Fact]
    public void JednakiBodoviSeRazrjesavajuNaslovomPaIdem()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(3, "B");
        o.Knjiga(2, "A");
        o.Knjiga(1, "A");
        Assert.Equal(new[] { 1, 2, 3 }, o.Preporuke.PreporuciKnjige(1).Select(p => p.Knjiga.Id));
    }
    [Fact]
    public void BrojPreporukaJeOgranicen()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1);
        o.Knjiga(2);
        Assert.Single(o.Preporuke.PreporuciKnjige(1, 1));
        Assert.Equal(2, o.Preporuke.PreporuciKnjige(1, 100).Count);
    }
    [Fact]
    public void PrazanInventarDajePraznuListu()
    {
        var o = new Okruzenje();
        o.Korisnik();
        Assert.Empty(o.Preporuke.PreporuciKnjige(1));
    }
    [Fact]
    public void PoziviNeMijenjajuInventarNiHistoriju()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga();
        o.Preporuke.PreporuciKnjige(1);
        o.Preporuke.PreporuciKnjige(1);
        Assert.Equal(3, o.Knjige.knjige[0].DostupnoPrimjeraka);
        Assert.Empty(o.Posudbe.DajSvePosudbe());
    }
    [Fact]
    public void PonovljenaPosudbaIstogNaslovaNeUdvostrucujeBodove()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1);
        o.Knjiga(2);
        o.Procitaj(1);
        o.Procitaj(1);
        Assert.Equal(5, Assert.Single(o.Preporuke.PreporuciKnjige(1)).Bodovi);
    }
    [Fact]
    public void HistorijaDrugogKorisnikaNeOdredjujeLicnePreferencije()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Korisnik(2);
        o.Knjiga(1);
        o.Knjiga(2);
        o.Procitaj(1, 2);
        Assert.All(o.Preporuke.PreporuciKnjige(1), p => Assert.Equal(0, p.Bodovi));
    }
    [Fact]
    public void PoredjenjeZanraIAutoraIgnoriseVelicinuSlovaIRazmake()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1, "Prva", " Autor ", " ROMAN ");
        o.Knjiga(2, "Druga", "autor", "roman");
        o.Procitaj(1);
        Assert.Equal(5, Assert.Single(o.Preporuke.PreporuciKnjige(1)).Bodovi);
    }
    [Fact]
    public void ObrisanaProcitanaKnjigaNeRusiPreporuke()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1);
        o.Knjiga(2);
        o.Procitaj(1);
        o.Knjige.ObrisiKnjigu(1);
        Assert.Equal(0, Assert.Single(o.Preporuke.PreporuciKnjige(1)).Bodovi);
    }
    [Fact]
    public void AktivnaPosudbaNijeDokazProcitaneKnjige()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1);
        o.Knjiga(2);
        o.Posudbe.PosudiKnjigu(1, 1);
        Assert.Equal(0, Assert.Single(o.Preporuke.PreporuciKnjige(1)).Bodovi);
    }
    [Fact]
    public void ViseProcitanihNaslovaPovecavaTezinuZanra()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga(1, "A", "Prvi");
        o.Knjiga(2, "B", "Drugi");
        o.Knjiga(3, "C", "Treci");
        o.Procitaj(1);
        o.Procitaj(2);
        Assert.Equal(6, Assert.Single(o.Preporuke.PreporuciKnjige(1)).Bodovi);
    }
    [Fact]
    public void NeispravniParametriSeOdbijaju()
    {
        var o = new Okruzenje();
        Assert.Throws<ArgumentOutOfRangeException>(() => o.Preporuke.PreporuciKnjige(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => o.Preporuke.PreporuciKnjige(1, 0));
        Assert.Throws<KeyNotFoundException>(() => o.Preporuke.PreporuciKnjige(1));
    }
    [Fact]
    public void NullZavisnostiSeOdbijaju()
    {
        var o = new Okruzenje();
        Assert.Throws<ArgumentNullException>(() => new PreporukaServis(null!, o.Korisnici, o.Posudbe));
        Assert.Throws<ArgumentNullException>(() => new PreporukaServis(o.Knjige, null!, o.Posudbe));
        Assert.Throws<ArgumentNullException>(() => new PreporukaServis(o.Knjige, o.Korisnici, null!));
    }
}
