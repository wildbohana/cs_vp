using Common;
using Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Service
{
    public class Server : IServer
    {
        #region SPAJANJE SA BAZOM
        // Kanal za komunikaciju sa XML bazom podataka
        private static IXmlDb kanal;

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

        #region UČITAVANJE VREMENA IZ KONFIGURACIJE
        private static TimeSpan timeoutZaBrisanje;
        private TimeSpan CitanjeVremenaIzKonfiguracije()
        {
            int sati = 0;
            int minute = 15;
            int sekunde = 0;

            try
            {
                sati = int.Parse(ConfigurationManager.AppSettings["brisanjeSati"]);
                minute = int.Parse(ConfigurationManager.AppSettings["brisanjeMinute"]);
                sekunde = int.Parse(ConfigurationManager.AppSettings["brisanjeSekunde"]);
            }
            catch
            {

            }

            TimeSpan vreme = TimeSpan.FromHours(sati) + TimeSpan.FromMinutes(minute) + TimeSpan.FromSeconds(sekunde);
            return vreme;
        }

        #endregion

        // Key - ID objekta | Value - objekat
        static Dictionary<int, LoadServis> recnikLoad; 
        static Dictionary<int, Audit> recnikAudit;

        // Konstruktor
        public Server()
        {
            kanal = KonekcijaBaza();
            timeoutZaBrisanje = CitanjeVremenaIzKonfiguracije();

            recnikLoad = new Dictionary<int, LoadServis>();
            recnikAudit = new Dictionary<int, Audit>();

            // Nit koja se stalno vrti
            Task.Factory.StartNew(() => ProveraIstekaTajmauta());
        }

        #region OBRADA UPITA
        public Tuple<List<Load>, Audit> UpitOdKlijenta(DateTime datum)
        {
            List<Load> pretraga = PretraziInMemoryBazu(datum);

            // Pretraga In-Memory baze
            if (pretraga.Count > 0)
            {
                Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum.ToString("dd.MM.yyyy.")} uspesno procitani iz In-Memory baze i prosledjeni.");                                
                recnikAudit.Add(audit.Id, audit);
                kanal.UpisUBazuPodataka(audit);

                Tuple<List<Load>, Audit> povratnaVrednost = new Tuple<List<Load>, Audit>(pretraga, audit);
                return povratnaVrednost;
            }
            // Pretraga XML baze
            else
            {
                List<Load> podaciIzBaze = kanal.ProcitajIzBazePodataka(datum);

                if (podaciIzBaze.Count == 0)
                {
                    Audit audit = NapraviAudit(MessageType.Error, $"Podaci za prosledjen datum {datum.ToString("dd.MM.yyyy.")} nisu pronadjeni!");
                    recnikAudit.Add(audit.Id, audit);
                    kanal.UpisUBazuPodataka(audit);

                    Tuple<List<Load>, Audit> povratnaVrednostNeuspesno = new Tuple<List<Load>, Audit>(podaciIzBaze, audit);
                    return povratnaVrednostNeuspesno;
                }
                else
                {
                    // Upiši podatke u lokalnu memoriju (thread-safe)
                    lock (recnikLoad)
                    {
                        foreach (Load l in podaciIzBaze)
                        {
                            recnikLoad.Add(l.Id, new LoadServis { Load = l, VremeDodavanja = DateTime.Now });
                        }
                    }
                    
                    Audit audit = NapraviAudit(MessageType.Info, $"Podaci za datum {datum.ToString("dd.MM.yyyy.")} uspesno procitani iz XML baze i prosledjeni.");
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

            foreach (LoadServis l in recnikLoad.Values)
                if (l.Load.Timestamp.Year == datum.Year && l.Load.Timestamp.Month == datum.Month && l.Load.Timestamp.Day == datum.Day)
                    trazeni.Add(l.Load);
           
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

        #region BRISANJE POSLE ISTEKA TIMEOUTA
        // Funkcija za nit
        private void ProveraIstekaTajmauta()
        {
            List<int> idLista;
            Brisanje += new Operacija(BrisanjePodatka);

            while (true)
            {
                // Mora bar malo da se uspori rad niti
                // U suprotnom puca program posle prvog Invoke eventa
                Thread.Sleep(1000);

                idLista = new List<int>();

                foreach (LoadServis ls in recnikLoad.Values)
                {
                    if ((DateTime.Now - ls.VremeDodavanja) > timeoutZaBrisanje)
                    {
                        idLista.Add(ls.Load.Id);
                    }
                }

                // Pozivanje eventa tek kada se desi da je bar jedan element dodat u listu za brisanje
                if (idLista.Count > 0)
                {
                    Brisanje.Invoke(idLista);
                }
            }
        }

        // Definisanje delegata i metode sa istim potpisom kao i delegat
        private delegate void Operacija(List<int> id);

        private void BrisanjePodatka(List<int> id)
        {
            lock (recnikLoad)
            {
                foreach (int i in id)
                    recnikLoad.Remove(i);
            }

            Console.WriteLine($"Obrisano {id.Count} starih podataka!");
        }

        // Definisanje eventa
        private static event Operacija Brisanje;
        #endregion
    }
}
