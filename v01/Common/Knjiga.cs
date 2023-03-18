using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Knjiga
    {
        // Brojač za ID knjige
        static int brojac = 0;

        // Polja
        private int idKnjige;
        private string imeKnjige;
        private string imeAutora;
        private string prezimeAutora;
        private Enumeracija.Zanrovi zanr;
        private DateTime datumIzdavanja;

        // Propertiji
        [DataMember]
        public int IdKnjige { get => idKnjige; set => idKnjige = value; }
        [DataMember]
        public string ImeKnjige { get => imeKnjige; set => imeKnjige = value; }
        [DataMember]
        public string ImeAutora { get => imeAutora; set => imeAutora = value; }
        [DataMember]
        public string PrezimeAutora { get => prezimeAutora; set => prezimeAutora = value; }
        [DataMember]
        public Enumeracija.Zanrovi Zanr { get => zanr; set => zanr = value; }
        [DataMember]
        public DateTime DatumIzdavanja { get => datumIzdavanja; set => datumIzdavanja = value; }

        // Konstruktori (bez i sa parametrima)
        public Knjiga()
        {
            this.idKnjige = ++brojac;
            this.imeKnjige = "";
            this.imeAutora = "";
            this.prezimeAutora = "";
            this.zanr = Enumeracija.Zanrovi.Unknown;
            this.datumIzdavanja = DateTime.Now;
        }

        public Knjiga(string imeKnjige, string imeAutora, string prezimeAutora, Enumeracija.Zanrovi zanr, DateTime datumIzdavanja)
        {
            this.idKnjige = ++brojac;
            this.imeKnjige = imeKnjige;
            this.imeAutora = imeAutora;
            this.prezimeAutora = prezimeAutora;
            this.zanr = zanr;
            this.datumIzdavanja = datumIzdavanja;
        }

        // Redefinisan ispis
        public override string ToString()
        {
            string retval = "";

            retval += "ID: " + idKnjige + "\t";
            retval += imeKnjige + ", " + prezimeAutora + " " + imeAutora;
            retval += ", " + datumIzdavanja + "\n";

            return retval;
        }
    }
}
