using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Service
{
    public class Server : IServer
    {
        #region SPAJANJE SA BAZOM
        // Kanal za komunikaciju sa XML bazom podataka
        public static IXmlDb kanal;

        private IXmlDb KonekcijaBaza()
        {
            try
            {
                string adresa = "net.tcp://localhost:8002/XmlBaza";
                ChannelFactory<IXmlDb> cf = new ChannelFactory<IXmlDb>(new NetTcpBinding(), new EndpointAddress(adresa));
                IXmlDb kanal = cf.CreateChannel();

                Console.WriteLine("Uspešno spajanje Severa na XML bazu podataka.");
                return kanal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
        #endregion

        #region UCITAVANJE VREMENA IZ KONFIGURACIJE
        private TimeSpan vremeIzmedjuProvera;
        private TimeSpan CitanjeVremenaIzKonfiguracije()
        {
            int sati = 0;
            int minute = 5;
            int sekunde = 0;

            try
            {
                sati = int.Parse(ConfigurationManager.AppSettings["proveraSati"]);
                minute = int.Parse(ConfigurationManager.AppSettings["proveraMinute"]);
                sekunde = int.Parse(ConfigurationManager.AppSettings["proveraSekunde"]);
            }
            catch
            {

            }

            TimeSpan vreme = TimeSpan.FromHours(sati) + TimeSpan.FromMinutes(minute) + TimeSpan.FromSeconds(sekunde);
            return vreme;
        }

        #endregion

        // Key - red u tabeli (XML), Value - objekat
        private int redUTabeliLoad;
        private int redUTabeliAudit;
        static Dictionary<int, Load> recnikLoad; 
        static Dictionary<int, Audit> recnikAudit;

        // Konstruktor
        public Server()
        {
            kanal = KonekcijaBaza();
            vremeIzmedjuProvera = CitanjeVremenaIzKonfiguracije();

            redUTabeliAudit = 0;        // += brojDodatih u XML bazi (ili neki drugi način, ne znam), možda ovo sačuvati u externi txt?
            redUTabeliLoad = 0;         // += brojUcitanih iz XML baze (ili neki drugi način, ne znam), možda i ovo sačuvati u ecterni txt? ili dodati funkciju neku

            recnikLoad = new Dictionary<int, Load>();
            recnikAudit = new Dictionary<int, Audit>();
        }

        // Uroš
        public Tuple<List<Load>, Audit> UpitOdKlijenta(DateTime datum)
        {
            Dictionary<int, Load> loadRecnik = PretraziInMemoryBazu(datum);

            if (loadRecnik == null)
            {
                Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum} uspesno procitani i prosledjeni");                                
                recnikAudit.Add(redUTabeliAudit, audit);

                Tuple<List<Load>, Audit> povratnaVrednost = new Tuple<List<Load>, Audit>(loadRecnik.Values.ToList(), audit);
                return povratnaVrednost;
                
            }
            else //u slucaju da nismo nasli podatke u In-Mem bazi prelazimo na pretragu XML baze podataka
            {
                Dictionary<int, Load> podaciIzBaze = kanal.CitanjeIzXmlBazeLoad(datum);
                if(podaciIzBaze == null)
                {

                    Audit audit = NapraviAudit(MessageType.Error, $"Podaci za prosledjen datum {datum} nisu pronadjeni");
                    recnikAudit.Add(redUTabeliAudit, audit);

                    Tuple<List<Load>, Audit> povratnaVrednostZaNullLoad = new Tuple<List<Load>, Audit>(podaciIzBaze.Values.ToList(), audit);
                    return povratnaVrednostZaNullLoad;
                }
                else
                {  
                    Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum} uspesno procitani i prosledjeni");
                    recnikAudit.Add(redUTabeliAudit, audit);

                    Tuple<List<Load>, Audit> povratnaVrednostIzXmlBaze = new Tuple<List<Load>, Audit>(podaciIzBaze.Values.ToList(), audit);
                    return povratnaVrednostIzXmlBaze;
                }
            }
        }

        private Dictionary<int, Load> PretraziInMemoryBazu(DateTime datum)
        {
            Dictionary<int, Load> loadRecnik = null;
            int i = 0;

            foreach (KeyValuePair<int, Load> kvp in recnikLoad)
            {
                if (kvp.Value.Timestamp.Year == datum.Year && kvp.Value.Timestamp.Month == datum.Month && kvp.Value.Timestamp.Day == datum.Day)
                {
                    loadRecnik.Add(kvp.Key, kvp.Value);
                }
            }

            return loadRecnik;
        }

        private Audit NapraviAudit(MessageType tipPoruke, string poruka)
        {
            // TODO napravi ID za audit
            DateTime trenutnoVreme = DateTime.Now;
            int auditId = 0;
            Audit audit = new Audit(auditId, trenutnoVreme, tipPoruke, poruka);

            return audit;
        }
    }
}
