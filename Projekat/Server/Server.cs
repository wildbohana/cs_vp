using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Service
{
    
    public class Server : IServer
    {
        
        // u ovom recniku se skladiste merenja za svaki dan tj. jedan dan predstavlja jedan dictionary<int, Load>  
        static Dictionary<DateTime, Dictionary<int, Load>> recnikSvihMerenja = new Dictionary<DateTime, Dictionary<int, Load>>(); 
        static Dictionary<DateTime ,Audit> auditRecnik = new Dictionary<DateTime, Audit>();
        // Uroš
        public Tuple<List<Load>, Audit> UpitOdKlijenta(DateTime datum)
        {

           Dictionary<int, Load> loadRecnik = PretraziInMemoryBazu(datum);


            if (loadRecnik.Count != 0)
            {
                
                
                
                DateTime trenutnoVreme =  DateTime.Now;
                //napravio sam da se Id generise ovako jer su sanse jakoooo male da se ID ponovi bar mislim 
                //sa id counterom milim da nece raditi kao sto mi je bila prvobitna ideja
                int auditId = trenutnoVreme.Millisecond + trenutnoVreme.Minute + trenutnoVreme.Second;
                Audit audit = new Audit(auditId, trenutnoVreme, MessageType.Info, "Podaci uspesno procitani i prosledjeni");
                //kreiranje audit objekta u kojem se nalazi poruka koja obavestava klijenta o uspesnom nalazenju merenja
                //E sad nisam bas siguran za timestamp.. da li treba da se stavi vreme kad je zahtev napravljen ili datum merenja
                //logicnije mi je da bude vreme kad je request napravljen jer datum merenja
                auditRecnik.Add(datum, audit);
                Tuple<List<Load>, Audit> tuple = new Tuple<List<Load>, Audit>(loadRecnik.Values.ToList(), audit);
                return tuple;
                //slucaj kada se podaci nalaze u In-Mem bazi podataka, podaci se prosledjuju i tu se funkcija zavrsava
            }
            else//u slucaju da nismo nasli podatke u In-Mem bazi prelazimo na pretragu XML baze podataka
            {
            //ovde treba implementirati logiku pretrage xml baze
            }
            
        }

        private Dictionary<int, Load> PretraziInMemoryBazu(DateTime datum)
        {
            Dictionary<int, Load> loadRecnik = null;

            foreach (DateTime time in recnikSvihMerenja.Keys)
            {
                loadRecnik = recnikSvihMerenja[time];//uspesno pronadjeno merenje za prosledjeni datum, uzimamo recnik u kom se nalaze merenja za svaki sat                
            }
            if(loadRecnik == null)
            {
                loadRecnik = new Dictionary<int, Load>();
                return loadRecnik;
            }
            else
            {
                return loadRecnik;
            }


        }
        
    }
}
