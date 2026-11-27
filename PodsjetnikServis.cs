namespace PametnaBiblioteka;

public interface IPosiljalacPoruka
{
    void Posalji(string primalac, string naslov, string tekst);
}

/// <summary>Pravilo podsjetnika je stvarno; slanje u testovima zamjenjuje lazni posiljalac.</summary>
public sealed class PodsjetnikServis
{
    private readonly KorisnikServis korisnici;
    private readonly KnjigaServis knjige;
    private readonly PosudbaServis posudbe;
    private readonly IPosiljalacPoruka posiljalac;
    private readonly HashSet<(int PosudbaId, DateTime Dan)> poslani = new();

    public PodsjetnikServis(KorisnikServis korisnici, KnjigaServis knjige,
        PosudbaServis posudbe, IPosiljalacPoruka posiljalac)
    {
        this.korisnici = korisnici ?? throw new ArgumentNullException(nameof(korisnici));
        this.knjige = knjige ?? throw new ArgumentNullException(nameof(knjige));
        this.posudbe = posudbe ?? throw new ArgumentNullException(nameof(posudbe));
        this.posiljalac = posiljalac ?? throw new ArgumentNullException(nameof(posiljalac));
    }

    public int PosaljiPodsjetnike(DateTime danas, int danaUnaprijed = 3)
    {
        if (danaUnaprijed < 0)
            throw new ArgumentOutOfRangeException(nameof(danaUnaprijed));
        danas = danas.Date;
        DateTime granica = danas.AddDays(danaUnaprijed);
        int broj = 0;
        foreach (var p in posudbe.DajAktivnePosudbe().Where(p => p.RokVracanja.Date <= granica))
        {
            if (poslani.Contains((p.Id, danas)))
                continue;
            var korisnik = korisnici.PronadjiKorisnika(p.KorisnikId)
                ?? throw new KeyNotFoundException("Primalac podsjetnika nije pronadjen.");
            var knjiga = knjige.knjige.FirstOrDefault(k => k.Id == p.KnjigaId)
                ?? throw new KeyNotFoundException("Knjiga za podsjetnik nije pronadjena.");
            string naslov = p.RokVracanja.Date < danas ? "Istekao rok vracanja" : "Podsjetnik za vracanje knjige";
            string tekst = $"Postovani {korisnik.Ime}, knjigu '{knjiga.Naslov}' vratite do {p.RokVracanja:dd.MM.yyyy}.";
            posiljalac.Posalji(korisnik.Email, naslov, tekst);
            // Neuspjelo slanje ne zatvara mogucnost ponovnog pokusaja.
            poslani.Add((p.Id, danas));
            broj++;
        }
        return broj;
    }
}
