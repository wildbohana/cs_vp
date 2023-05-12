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
        // Kanal za komunikaciju sa XML bazom podataka
        public static IXmlDb kanal = null;

        // u ovom recniku se skladiste merenja za svaki dan tj. jedan dan predstavlja jedan dictionary<int, Load>  
        static Dictionary<DateTime, Dictionary<int, Load>> recnikSvihMerenja = new Dictionary<DateTime, Dictionary<int, Load>>(); 
        
        static Dictionary<DateTime ,Audit> auditRecnik = new Dictionary<DateTime, Audit>();

        // Uroš
        public Tuple<List<Load>, Audit> UpitOdKlijenta(DateTime datum)
        {

            Dictionary<int, Load> loadRecnik = PretraziInMemoryBazu(datum);

            if (loadRecnik == null)
            {
                Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum} uspesno procitani i prosledjeni");                                
                auditRecnik.Add(datum, audit);

                Tuple<List<Load>, Audit> povratnaVrednost = new Tuple<List<Load>, Audit>(loadRecnik.Values.ToList(), audit);
                return povratnaVrednost;
                
            }
            else //u slucaju da nismo nasli podatke u In-Mem bazi prelazimo na pretragu XML baze podataka
            {

                Dictionary<int, Load> podaciIzBaze = kanal.CitanjeIzXmlBazeLoad(datum);
                if(podaciIzBaze == null)
                {

                    Audit audit = NapraviAudit(MessageType.Error, $"Podaci za prosledjen datum {datum} nisu pronadjeni");
                    auditRecnik.Add(datum, audit);

                    Tuple<List<Load>, Audit> povratnaVrednostZaNullLoad = new Tuple<List<Load>, Audit>(podaciIzBaze.Values.ToList(), audit);
                    return povratnaVrednostZaNullLoad;
                }
                else
                {
                   
                    Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum} uspesno procitani i prosledjeni");
                    auditRecnik.Add(datum, audit);

                    Tuple<List<Load>, Audit> povratnaVrednostIzXmlBaze = new Tuple<List<Load>, Audit>(podaciIzBaze.Values.ToList(), audit);
                    return povratnaVrednostIzXmlBaze;
                }
                

            }            
        }

        private Dictionary<int, Load> PretraziInMemoryBazu(DateTime datum)
        {
            Dictionary<int, Load> loadRecnik = null;

            
            if (recnikSvihMerenja.Keys.Contains(datum))
                loadRecnik = recnikSvihMerenja[datum];
            else
                loadRecnik = null;
            
            return loadRecnik;

        }        
        private Audit NapraviAudit(MessageType tipPoruke, string poruka)
        {
            //Treba proveriti da li ovaj nacin pravljena ID-a vlaja kad se program bude testirao
            DateTime trenutnoVreme = DateTime.Now;
            int auditId = trenutnoVreme.Millisecond + trenutnoVreme.Minute + trenutnoVreme.Second;
            Audit audit = new Audit(auditId, trenutnoVreme, tipPoruke, poruka);

            return audit;
        }
    }
}
