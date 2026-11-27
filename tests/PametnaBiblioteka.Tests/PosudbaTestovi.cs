using Xunit;
namespace PametnaBiblioteka.Tests;
public class PosudbaTestovi
{
    [Fact]
    public void PosudbaSmanjujeDostupnostIRacunaRok()
    {
        var o = new Okruzenje();
        o.Korisnik();
        var k = o.Knjiga();
        o.Posudbe.PosudiKnjigu(1, 1);
        var p = Assert.Single(o.Posudbe.DajSvePosudbe());
        Assert.Equal(1, p.Id);
        Assert.Equal(o.Dan, p.DatumPosudbe);
        Assert.Equal(o.Dan.AddDays(14), p.RokVracanja);
        Assert.False(p.Vracena);
        Assert.Null(p.DatumVracanja);
        Assert.Equal(0m, p.Zakasnina);
        Assert.Equal(2, k.DostupnoPrimjeraka);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NeispravanKorisnikIdSeOdbija(int id)
    {
        var o = new Okruzenje();
        Assert.Throws<ArgumentException>(() => o.Posudbe.PosudiKnjigu(id, 1));
        Assert.Empty(o.Posudbe.DajSvePosudbe());
    }
    [Fact]
    public void NepostojeciKorisnikIKnjigaSeOdbijaju()
    {
        var o = new Okruzenje();
        Assert.Throws<KeyNotFoundException>(() => o.Posudbe.PosudiKnjigu(1, 1));
        o.Korisnik();
        Assert.Throws<KeyNotFoundException>(() => o.Posudbe.PosudiKnjigu(1, 1));
    }
    [Fact]
    public void NedostupnaKnjigaSeNeMozePosuditi()
    {
        var o = new Okruzenje();
        o.Korisnik();
        var k = o.Knjiga();
        k.DostupnoPrimjeraka = 0;
        Assert.Throws<InvalidOperationException>(() => o.Posudbe.PosudiKnjigu(1, 1));
        Assert.Empty(o.Posudbe.DajSvePosudbe());
    }
    [Fact]
    public void DuplaAktivnaPosudbaSeOdbija()
    {
        var o = new Okruzenje();
        o.Korisnik();
        var k = o.Knjiga();
        o.Posudbe.PosudiKnjigu(1, 1);
        Assert.Throws<InvalidOperationException>(() => o.Posudbe.PosudiKnjigu(1, 1));
        Assert.Single(o.Posudbe.DajSvePosudbe());
        Assert.Equal(2, k.DostupnoPrimjeraka);
    }
    [Theory]
    [InlineData(0, 0)]
    [InlineData(14, 0)]
    [InlineData(15, 2)]
    [InlineData(17, 6)]
    public void ZakasninaPremaBrojuDana(int proteklo, int ocekivano)
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga();
        o.Posudbe.CijenaPoDanu = 2;
        o.Posudbe.PosudiKnjigu(1, 1);
        o.Dan = o.Dan.AddDays(proteklo);
        Assert.Equal((decimal)ocekivano, o.Posudbe.ObracunajZakasninu(1));
        o.Posudbe.VratiKnjigu(1, 1);
        var p = Assert.Single(o.Posudbe.DajSvePosudbe());
        Assert.True(p.Vracena);
        Assert.Equal(o.Dan, p.DatumVracanja);
        Assert.Equal((decimal)ocekivano, p.Zakasnina);
        o.Dan = o.Dan.AddDays(20);
        Assert.Equal((decimal)ocekivano, o.Posudbe.ObracunajZakasninu(1));
        Assert.Empty(o.Posudbe.DajAktivnePosudbe());
    }
    [Fact]
    public void VracanjeBezAktivnePosudbeSeOdbija()
    {
        var o = new Okruzenje();
        Assert.Throws<InvalidOperationException>(() => o.Posudbe.VratiKnjigu(1, 1));
    }
    [Fact]
    public void VracanjeOdbijaNestaluKnjiguINekonzistentnoStanje()
    {
        var o = new Okruzenje();
        o.Korisnik();
        var k = o.Knjiga();
        o.Posudbe.PosudiKnjigu(1, 1);
        o.Knjige.knjige.Clear();
        Assert.Throws<KeyNotFoundException>(() => o.Posudbe.VratiKnjigu(1, 1));
        o.Knjige.knjige.Add(k);
        k.DostupnoPrimjeraka = k.UkupnoPrimjeraka;
        Assert.Throws<InvalidOperationException>(() => o.Posudbe.VratiKnjigu(1, 1));
        Assert.False(o.Posudbe.DajSvePosudbe()[0].Vracena);
    }
    [Fact]
    public void HistorijaJeFiltriranaSortiranaIVracenaKaoKopijaListe()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Korisnik(2);
        o.Knjiga();
        o.Knjiga(2);
        o.Procitaj(1);
        o.Dan = o.Dan.AddDays(1);
        o.Posudbe.PosudiKnjigu(1, 2);
        o.Posudbe.PosudiKnjigu(2, 1);
        Assert.Equal(new[] { 2, 1 }, o.Posudbe.DajHistorijuKorisnika(1).Select(p => p.Id));
        Assert.Equal(2, o.Posudbe.DajAktivnePosudbe().Count);
        var lista = o.Posudbe.DajSvePosudbe();
        lista.Clear();
        Assert.Equal(3, o.Posudbe.DajSvePosudbe().Count);
    }
    [Fact]
    public void ParametriPosudbeMorajuBitiIspravni()
    {
        var o = new Okruzenje();
        Assert.Throws<ArgumentOutOfRangeException>(() => o.Posudbe.CijenaPoDanu = -1);
        Assert.Throws<ArgumentOutOfRangeException>(() => o.Posudbe.BrojDanaPosudbe = 0);
        o.Posudbe.BrojDanaPosudbe = 7;
        Assert.Equal(7, o.Posudbe.BrojDanaPosudbe);
        Assert.Throws<KeyNotFoundException>(() => o.Posudbe.ObracunajZakasninu(99));
    }
    [Fact]
    public void KonstruktorOdbijaNullZavisnosti()
    {
        var o = new Okruzenje();
        Assert.Throws<ArgumentNullException>(() => new PosudbaServis(null!, o.Korisnici));
        Assert.Throws<ArgumentNullException>(() => new PosudbaServis(o.Knjige, null!));
    }
    [Fact]
    public void IspisHistorijeObuhvataPraznuAktivnuIVracenu()
    {
        var o = new Okruzenje();
        o.Korisnik();
        o.Knjiga();
        var stari = Console.Out;
        var izlaz = new StringWriter();
        try
        {
            Console.SetOut(izlaz);
            o.Posudbe.PrikaziHistorijuKorisnika(1);
            o.Posudbe.PosudiKnjigu(1, 1);
            o.Posudbe.PrikaziHistorijuKorisnika(1);
            o.Posudbe.VratiKnjigu(1, 1);
            o.Posudbe.PrikaziHistorijuKorisnika(1);
        }
        finally { Console.SetOut(stari); }
        Assert.Contains("nema historiju", izlaz.ToString());
        Assert.Contains("Vracena: Ne", izlaz.ToString());
        Assert.Contains("Vracena: Da", izlaz.ToString());
    }
    [Fact]
    public void ModelPosudbeOmogucavaEvidentiranjePodataka()
    {
        var p = new Posudba(1, 2, 3, new DateTime(2026, 1, 1), new DateTime(2026, 1, 15));
        p.Id = 4;
        p.KorisnikId = 5;
        p.KnjigaId = 6;
        p.DatumPosudbe = new DateTime(2026, 2, 1);
        p.RokVracanja = new DateTime(2026, 2, 15);
        Assert.Equal(4, p.Id);
        Assert.Equal(5, p.KorisnikId);
        Assert.Equal(6, p.KnjigaId);
        Assert.Equal(2, p.DatumPosudbe.Month);
        Assert.Equal(2, p.RokVracanja.Month);
    }
}
