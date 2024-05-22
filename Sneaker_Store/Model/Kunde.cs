namespace Sneaker_Store.Model
{
    public class Kunde
    {
        // Instansfelter
        private int _kundeId;
        private string _navn;
        private string _efternavn;
        private string _email;
        private string _adresse;
        private string _by;
        private int _postnr;
        private string _kode;
        private bool _admin;

        // Egenskaber
        public int KundeId
        {
            get { return _kundeId; }
            set { _kundeId = value; }
        }

        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        public string Efternavn
        {
            get { return _efternavn; }
            set { _efternavn = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string Adresse
        {
            get { return _adresse; }
            set { _adresse = value; }
        }

        public string By
        {
            get { return _by; }
            set { _by = value; }
        }

        public int Postnr
        {
            get { return _postnr; }
            set { _postnr = value; }
        }

        public string Kode
        {
            get { return _kode; }
            set { _kode = value; }
        }

        public bool Admin
        {
            get { return _admin; }
            set { _admin = value; }
        }

        // Konstruktører
        public Kunde()
        {
            _kundeId = 0;
            _navn = "";
            _efternavn = "";
            _email = "";
            _adresse = "";
            _by = "";
            _postnr = 0;
            _kode = "";
            _admin = false;
        }

        public Kunde(int kundeId, string navn, string efternavn, string email, string adresse, string by, int postnr, string kode, bool admin)
        {
            _kundeId = kundeId;
            _navn = navn;
            _efternavn = efternavn;
            _email = email;
            _adresse = adresse;
            _by = by;
            _postnr = postnr;
            _kode = kode;
            _admin = admin;
        }

        // ToString-metode
        public override string ToString()
        {
            return $"{nameof(KundeId)}: {KundeId}, {nameof(Navn)}: {Navn}, {nameof(Efternavn)}: {Efternavn}, {nameof(Email)}: {Email}, {nameof(Adresse)}: {Adresse}, {nameof(By)}: {By}, {nameof(Postnr)}: {Postnr}, {nameof(Kode)}: {Kode}, {nameof(Admin)}: {Admin}";
        }
    }
}
