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

        // Key - ID objekta | Value - objekat
        static Dictionary<int, Load> recnikLoad; 
        static Dictionary<int, Audit> recnikAudit;

        // Konstruktor
        public Server()
        {
            kanal = KonekcijaBaza();
            vremeIzmedjuProvera = CitanjeVremenaIzKonfiguracije();

            recnikLoad = new Dictionary<int, Load>();
            recnikAudit = new Dictionary<int, Audit>();
        }

        #region OBRADA UPITA
        public Tuple<List<Load>, Audit> UpitOdKlijenta(DateTime datum)
        {
            List<Load> pretraga = PretraziInMemoryBazu(datum);

            // Pretraga lokalno
            if (pretraga.Count > 0)
            {
                Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum.ToString("dd.MM.yyyy.")} uspesno procitani i prosledjeni");                                
                recnikAudit.Add(audit.Id, audit);
                kanal.UpisUBazuPodataka(audit);

                Tuple<List<Load>, Audit> povratnaVrednost = new Tuple<List<Load>, Audit>(pretraga, audit);
                return povratnaVrednost;
            }
            // Pretraga XML
            else
            {
                List<Load> podaciIzBaze = kanal.ProcitajIzBazePodataka(datum);

                if (podaciIzBaze.Count == 0)
                {
                    Audit audit = NapraviAudit(MessageType.Error, $"Podaci za prosledjen datum {datum.ToString("dd.MM.yyyy.")} nisu pronadjeni");
                    recnikAudit.Add(audit.Id, audit);
                    kanal.UpisUBazuPodataka(audit);

                    Tuple<List<Load>, Audit> povratnaVrednostNeuspesno = new Tuple<List<Load>, Audit>(podaciIzBaze, audit);
                    return povratnaVrednostNeuspesno;
                }
                else
                {
                    // Upiši podatke u lokalnu memoriju, prijavi na event, prosledi ih klijentu
                    foreach (Load l in podaciIzBaze)
                    {
                        recnikLoad.Add(l.Id, l);

                        // TODO delegat za brisanje
                    }

                    Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum.ToString("dd.MM.yyyy.")} uspesno procitani i prosledjeni");
                    recnikAudit.Add(audit.Id, audit);
                    kanal.UpisUBazuPodataka(audit);

                    Tuple<List<Load>, Audit> povratnaVrednostIzXmlBaze = new Tuple<List<Load>, Audit>(podaciIzBaze, audit);
                    return povratnaVrednostIzXmlBaze;
                }
            }
        }

        private List<Load> PretraziInMemoryBazu(DateTime datum)
        {
            List<Load> trazeni = new List<Load>(24);

            foreach (Load l in recnikLoad.Values)
                if (l.Timestamp.Year == datum.Year && l.Timestamp.Month == datum.Month && l.Timestamp.Day == datum.Day)
                    trazeni.Add(l);
           
            return trazeni;
        }
        #endregion

        #region AUDIT
        private Audit NapraviAudit(MessageType tipPoruke, string poruka)
        {
            DateTime vreme = DateTime.Now;
            int auditId = kanal.NajveciAudit();
            Audit audit = new Audit(++auditId, vreme, tipPoruke, poruka);

            return audit;
        }
        #endregion

        #region DELEGATI
        // TODO napravi ga lol

        #endregion
    }
}
