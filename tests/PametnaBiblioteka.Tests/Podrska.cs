using PametnaBiblioteka;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace PametnaBiblioteka.Tests;

// Svaki test kreira vlastitu instancu; nema zajednickih lista ni stvarnog sata.
public sealed class Okruzenje
{
    public DateTime Dan = new(2026, 11, 1);
    public KnjigaServis Knjige { get; } = new();
    public KorisnikServis Korisnici
    {
        get;
    }
    public PosudbaServis Posudbe
    {
        get;
    }
    public PreporukaServis Preporuke
    {
        get;
    }
    public Okruzenje()
    {
        PosudbaServis? p = null;
        Korisnici = new KorisnikServis(id => p!.DajAktivnePosudbe().Any(x => x.KorisnikId == id));
        Posudbe = p = new PosudbaServis(Knjige, Korisnici, () => Dan);
        Preporuke = new PreporukaServis(Knjige, Korisnici, Posudbe);
    }
    public void Korisnik(int id = 1) => Korisnici.RegistrujKorisnika(id, "Faris", "Šišić", $"korisnik{id}@example.com");
    public Knjiga Knjiga(int id = 1, string naslov = "Naslov", string autor = "Autor", string zanr = "Roman", int kolicina = 3)
    {
        Knjige.DodajKnjigu(id, naslov, autor, zanr, kolicina);
        return Knjige.knjige.Single(k => k.Id == id);
    }
    public void Procitaj(int knjigaId, int korisnikId = 1)
    {
        Posudbe.PosudiKnjigu(korisnikId, knjigaId);
        Posudbe.VratiKnjigu(korisnikId, knjigaId);
    }
}

public sealed record SnimljenaPoruka(string Primalac, string Naslov, string Tekst);

// Zamjenski objekat (fake/spy): pamti pozive, ne salje email i ne koristi mrezu.
public sealed class LazniPosiljalac : IPosiljalacPoruka
{
    public List<SnimljenaPoruka> Poruke { get; } = new();
    public bool SimulirajGresku
    {
        get; set;
    }
    public void Posalji(string primalac, string naslov, string tekst)
    {
        if (SimulirajGresku)
            throw new IOException("Simulirano nedostupno slanje.");
        Poruke.Add(new SnimljenaPoruka(primalac, naslov, tekst));
    }
}
