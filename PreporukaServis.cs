namespace PametnaBiblioteka;

public sealed record PreporukaKnjige(Knjiga Knjiga, int Bodovi, string Razlog);

/// <summary>Preporuke iz stvarne historije servisa posudbi; ne koriste mrezu ni slucajne brojeve.</summary>
public sealed class PreporukaServis
{
    private readonly KnjigaServis knjige;
    private readonly KorisnikServis korisnici;
    private readonly PosudbaServis posudbe;
    private const int BodoviZaZanr = 3;
    private const int BodoviZaAutora = 2;

    public PreporukaServis(KnjigaServis knjige, KorisnikServis korisnici, PosudbaServis posudbe)
    {
        this.knjige = knjige ?? throw new ArgumentNullException(nameof(knjige));
        this.korisnici = korisnici ?? throw new ArgumentNullException(nameof(korisnici));
        this.posudbe = posudbe ?? throw new ArgumentNullException(nameof(posudbe));
    }

    public IReadOnlyList<PreporukaKnjige> PreporuciKnjige(int korisnikId, int brojPreporuka = 5)
    {
        if (korisnikId <= 0)
            throw new ArgumentOutOfRangeException(nameof(korisnikId));
        if (brojPreporuka <= 0)
            throw new ArgumentOutOfRangeException(nameof(brojPreporuka));
        if (korisnici.PronadjiKorisnika(korisnikId) == null)
            throw new KeyNotFoundException("Korisnik nije pronadjen.");

        var historija = posudbe.DajHistorijuKorisnika(korisnikId);
        var vecPosudjene = historija.Select(p => p.KnjigaId).ToHashSet();
        // Svaki procitani naslov doprinosi jednom; aktivne posudbe ne znace procitanu knjigu.
        var procitaneId = historija.Where(p => p.Vracena).Select(p => p.KnjigaId).ToHashSet();
        var procitane = knjige.knjige.Where(k => procitaneId.Contains(k.Id)).ToList();
        var zanrovi = procitane.GroupBy(k => k.Zanr.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
        var autori = procitane.GroupBy(k => k.Autor.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
        // Popularnost: broj razlicitih korisnika koji su posudili naslov.
        var popularnost = posudbe.DajSvePosudbe().GroupBy(p => p.KnjigaId)
            .ToDictionary(g => g.Key, g => g.Select(p => p.KorisnikId).Distinct().Count());

        return knjige.knjige.Where(k => k.DostupnoPrimjeraka > 0 && !vecPosudjene.Contains(k.Id))
            .Select(k =>
            {
                int zanr = zanrovi.GetValueOrDefault(k.Zanr.Trim());
                int autor = autori.GetValueOrDefault(k.Autor.Trim());
                int bodovi = BodoviZaZanr * zanr + BodoviZaAutora * autor;
                string razlog = bodovi > 0
                    ? $"Historija citanja: zanr {zanr}, autor {autor}."
                    : "Dopunska preporuka prema popularnosti i naslovu.";
                return new PreporukaKnjige(k, bodovi, razlog);
            })
            .OrderByDescending(p => p.Bodovi)
            .ThenByDescending(p => popularnost.GetValueOrDefault(p.Knjiga.Id))
            .ThenBy(p => p.Knjiga.Naslov, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Knjiga.Id)
            .Take(brojPreporuka).ToList().AsReadOnly();
    }
}
