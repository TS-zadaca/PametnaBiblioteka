using Xunit;
namespace PametnaBiblioteka.Tests;

public class KorisnikTestovi
{
    [Fact]
    public void RegistracijaNormalizujePodatkeIDodjeljujeClana()
    {
        var s = new KorisnikServis();
        var k = s.RegistrujKorisnika(1, " Faris ", " Šišić ", " faris@example.com ");
        Assert.Equal("Faris", k.Ime);
        Assert.Equal("Šišić", k.Prezime);
        Assert.Equal("faris@example.com", k.Email);
        Assert.Equal(UlogaKorisnika.Clan, k.Uloga);
        Assert.Same(k, s.PronadjiKorisnika(1));
    }

    // DATA DRIVEN: jedna testna metoda, vise razlicitih ulaznih skupova.
    [Theory]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("A", false)]
    [InlineData("Al", true)]
    [InlineData("Faris2", false)]
    [InlineData("--", false)]
    [InlineData("Ana-Marija", true)]
    [InlineData("O'Neil", true)]
    [InlineData("Šišić", true)]
    public void ValidacijaImenaRazlicitiUlazi(string ime, bool ispravno)
    {
        if (ispravno)
            Assert.Equal(ime, new Korisnik(1, ime, "Prezime", "a@example.com").Ime);
        else
            Assert.Throws<ArgumentException>(() => new Korisnik(1, ime, "Prezime", "a@example.com"));
    }
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(50, true)]
    [InlineData(51, false)]
    public void GranicneDuzineImena(int duzina, bool ispravno)
    {
        string ime = new('a', duzina);
        if (ispravno)
            Assert.Equal(duzina, new Korisnik(1, ime, "Prezime", "a@example.com").Ime.Length);
        else
            Assert.Throws<ArgumentException>(() => new Korisnik(1, ime, "Prezime", "a@example.com"));
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("a b@example.com")]
    [InlineData("a@localhost")]
    [InlineData("Ime <a@example.com>")]
    [InlineData("a@@example.com")]
    public void NeispravniEmailSeOdbija(string? email) =>
        Assert.Throws<ArgumentException>(() => new Korisnik(1, "Faris", "Šišić", email!));

    [Fact]
    public void ModelOdbijaNullImePredugEmailNeispravanIdIUlogu()
    {
        Assert.Throws<ArgumentException>(() => new Korisnik(1, null!, "Prezime", "a@example.com"));
        Assert.Throws<ArgumentException>(() => new Korisnik(1, "Faris", "", "a@example.com"));
        Assert.Throws<ArgumentException>(() => new Korisnik(1, "Faris", "Prezime", new string('a', 255) + "@example.com"));
        Assert.Throws<ArgumentException>(() => new Korisnik(0, "Faris", "Prezime", "a@example.com"));
        Assert.Throws<ArgumentException>(() => new Korisnik(1, "Faris", "Prezime", "a@example.com", (UlogaKorisnika)9));
    }
    [Fact]
    public void DupliIdSeProvjeravaPrijeKreiranjaObjekta()
    {
        var o = new Okruzenje();
        o.Korisnik();
        var ex = Assert.Throws<ArgumentException>(() => o.Korisnici.RegistrujKorisnika(1, "", "", ""));
        Assert.Equal("id", ex.ParamName);
        Assert.Single(o.Korisnici.DajSveKorisnike());
    }
    [Fact]
    public void DupliEmailNeZavisiOdVelicineSlova()
    {
        var o = new Okruzenje();
        o.Korisnik();
        Assert.Throws<ArgumentException>(() => o.Korisnici.RegistrujKorisnika(2, "Ana", "Anić", "KORISNIK1@EXAMPLE.COM"));
        Assert.Single(o.Korisnici.DajSveKorisnike());
    }
    [Fact]
    public void IzmjenaCuvaIdIUloguIDozvoljavaVlastitiEmail()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Korisnici.DodijeliUlogu(1, UlogaKorisnika.Administrator);
        var k = o.Korisnici.AzurirajKorisnika(1, "Amir", "Hadžić", "korisnik1@example.com");
        Assert.Equal(1, k.Id);
        Assert.Equal(UlogaKorisnika.Administrator, k.Uloga);
        Assert.Equal("Amir", k.Ime);
    }
    [Fact]
    public void NeispravnaIzmjenaIDupliEmailNeMijenjajuKorisnika()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Korisnik(2);
        var prije = o.Korisnici.PronadjiKorisnika(1);
        Assert.Throws<ArgumentException>(() => o.Korisnici.AzurirajKorisnika(1, "Ana", "Anić", "korisnik2@example.com"));
        Assert.Throws<ArgumentException>(() => o.Korisnici.AzurirajKorisnika(1, "", "Anić", "novi@example.com"));
        Assert.Same(prije, o.Korisnici.PronadjiKorisnika(1));
    }
    [Fact]
    public void NepostojeciINeispravanIdSeOdbijaju()
    {
        var s = new KorisnikServis();
        Assert.Throws<ArgumentException>(() => s.ObrisiKorisnika(-1));
        Assert.Throws<KeyNotFoundException>(() => s.ObrisiKorisnika(1));
        Assert.Throws<KeyNotFoundException>(() => s.DodijeliUlogu(1, UlogaKorisnika.Clan));
        Assert.Null(s.PronadjiKorisnika(1));
    }
    [Fact]
    public void AktivnaPosudbaBlokiraBrisanjePaVracanjeOmogucava()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga();
        o.Posudbe.PosudiKnjigu(1, 1);
        Assert.Throws<InvalidOperationException>(() => o.Korisnici.ObrisiKorisnika(1));
        o.Posudbe.VratiKnjigu(1, 1);
        o.Korisnici.ObrisiKorisnika(1);
        Assert.Empty(o.Korisnici.DajSveKorisnike());
    }
    [Fact]
    public void BrisanjeBezProvjerePosudbiIPregledKaoSnimak()
    {
        var s = new KorisnikServis();
        s.RegistrujKorisnika(1, "Ana", "Zorić", "a@example.com");
        s.RegistrujKorisnika(2, "Faris", "Anić", "b@example.com");
        var lista = s.DajSveKorisnike();
        Assert.Equal(new[] { 2, 1 }, lista.Select(k => k.Id));
        Assert.Throws<NotSupportedException>(() => ((IList<Korisnik>)lista).Clear());
        s.ObrisiKorisnika(1);
        Assert.Equal(2, lista.Count);
        Assert.Single(s.DajSveKorisnike());
    }
}
