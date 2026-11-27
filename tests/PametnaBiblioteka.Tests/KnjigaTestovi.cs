using Xunit;
namespace PametnaBiblioteka.Tests;
public class KnjigaTestovi
{
    [Fact]
    public void DodavanjePostavljaKolicine()
    {
        var o = new Okruzenje();
        var k = o.Knjiga();
        Assert.Equal(3, k.UkupnoPrimjeraka);
        Assert.Equal(3, k.DostupnoPrimjeraka);
        Assert.True(o.Knjige.DaLiJeDostupna(1));
        Assert.False(o.Knjige.DaLiJeDostupna(99));
        k.DostupnoPrimjeraka = 0;
        Assert.False(o.Knjige.DaLiJeDostupna(1));
    }
    [Fact]
    public void DuplikatNeDodajeDruguKnjigu()
    {
        var o = new Okruzenje();
        o.Knjiga();
        o.Knjige.DodajKnjigu(1, "Druga", "Autor", "Zanr", 1);
        Assert.Single(o.Knjige.knjige);
    }
    [Theory]
    [InlineData(0, "Naslov", "Autor", "Zanr", 1)]
    [InlineData(1, "", "Autor", "Zanr", 1)]
    [InlineData(1, "Naslov", " ", "Zanr", 1)]
    [InlineData(1, "Naslov", "Autor", "", 1)]
    [InlineData(1, "Naslov", "Autor", "Zanr", 0)]
    [InlineData(1, "Naslov", "Autor", "Zanr", -1)]
    public void NeispravnoDodavanjeNeMijenjaListu(int id, string naslov, string autor, string zanr, int kolicina)
    {
        var s = new KnjigaServis();
        s.DodajKnjigu(id, naslov, autor, zanr, kolicina);
        Assert.Empty(s.knjige);
    }
    [Fact]
    public void IzmjenaOcuvaBrojPosudjenih()
    {
        var o = new Okruzenje();
        var k = o.Knjiga();
        k.DostupnoPrimjeraka = 1;
        o.Knjige.AzurirajKnjigu(1, "Novi", "Drugi", "Drama", 5);
        Assert.Equal(5, k.UkupnoPrimjeraka);
        Assert.Equal(3, k.DostupnoPrimjeraka);
        Assert.Equal("Novi", k.Naslov);
        Assert.Equal("Drugi", k.Autor);
        Assert.Equal("Drama", k.Zanr);
    }
    [Fact]
    public void SmanjenjeIspodPosudjenihSeOdbija()
    {
        var o = new Okruzenje();
        var k = o.Knjiga();
        k.DostupnoPrimjeraka = 1;
        o.Knjige.AzurirajKnjigu(1, "Novi", "Autor", "Zanr", 1);
        Assert.Equal(3, k.UkupnoPrimjeraka);
        Assert.Equal("Naslov", k.Naslov);
    }
    [Theory]
    [InlineData("", "Autor", "Zanr", 1)]
    [InlineData("Naslov", "", "Zanr", 1)]
    [InlineData("Naslov", "Autor", "", 1)]
    [InlineData("Naslov", "Autor", "Zanr", -1)]
    public void NeispravnaIzmjenaSeOdbija(string n, string a, string z, int k)
    {
        var o = new Okruzenje();
        o.Knjiga();
        o.Knjige.AzurirajKnjigu(1, n, a, z, k);
        Assert.Equal(3, o.Knjige.knjige[0].UkupnoPrimjeraka);
    }
    [Fact]
    public void NepostojecaKnjigaSeNeMijenjaINeBrise()
    {
        var s = new KnjigaServis();
        s.AzurirajKnjigu(99, "N", "A", "Z", 1);
        s.ObrisiKnjigu(99);
        Assert.Empty(s.knjige);
    }
    [Fact]
    public void BrisanjePosudjeneJeBlokirano()
    {
        var o = new Okruzenje();
        var k = o.Knjiga();
        k.DostupnoPrimjeraka--;
        o.Knjige.ObrisiKnjigu(1);
        Assert.Single(o.Knjige.knjige);
        k.DostupnoPrimjeraka++;
        o.Knjige.ObrisiKnjigu(1);
        Assert.Empty(o.Knjige.knjige);
    }
    [Theory]
    [InlineData(" NASLOV ", 1)]
    [InlineData("autor", 1)]
    [InlineData("roman", 1)]
    [InlineData("nema", 0)]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    public void PretragaNaslovaAutoraIZanra(string? pojam, int broj)
    {
        var o = new Okruzenje();
        o.Knjiga();
        Assert.Equal(broj, o.Knjige.PretraziKnjige(pojam).Count);
    }
}
