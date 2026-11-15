using System;

namespace PametnaBiblioteka
{
    public class Posudba
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public int KnjigaId { get; set; }
        public DateTime DatumPosudbe { get; set; }
        public DateTime RokVracanja { get; set; }
        public DateTime? DatumVracanja { get; set; }
        public bool Vracena { get; set; }
        public decimal Zakasnina { get; set; }

        public Posudba(int id, int korisnikId, int knjigaId,
            DateTime datumPosudbe, DateTime rokVracanja)
        {
            Id = id;
            KorisnikId = korisnikId;
            KnjigaId = knjigaId;
            DatumPosudbe = datumPosudbe;
            RokVracanja = rokVracanja;
            DatumVracanja = null;
            Vracena = false;
            Zakasnina = 0;
        }
    }
}
