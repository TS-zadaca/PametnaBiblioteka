namespace PametnaBiblioteka
{
    public class Knjiga
    {
        public int Id
        {
            get; set;
        }
        public string Naslov
        {
            get; set;
        }
        public string Autor
        {
            get; set;
        }
        public string Zanr
        {
            get; set;
        }
        public int UkupnoPrimjeraka
        {
            get; set;
        }
        public int DostupnoPrimjeraka
        {
            get; set;
        }

        public Knjiga(int id, string naslov, string autor, string zanr, int ukupnoPrimjeraka)
        {
            Id = id;
            Naslov = naslov;
            Autor = autor;
            Zanr = zanr;
            UkupnoPrimjeraka = ukupnoPrimjeraka;
            DostupnoPrimjeraka = ukupnoPrimjeraka;
        }
    }
}
