using Common;
using Common.Datoteke;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

// Bojana
// Rad sa datotekama treba da bude implementiran tako da se vodi računa o održavanju memorije, korišćenjem Dispose paterna
// XML baza podataka sadrži XML datoteke u koje se upisuju podaci
// Svaka tabela je implementirana kroz jednu XML datoteku.
// Ukoliko XML datoteka ne postoji, potrebno je da bude kreirana automatski.

namespace XMLDB
{
    public class XmlBaza : IXmlDb
    {
        #region OTVARANJE XML
        // Za otvaranje XML datoteke
        public IRadSaDatotekom OtvoriDatoteku(string putanjaDatoteke)
        {
            // Ako XML datoteka ne postoji, treba je automatski napraviti
            if (!File.Exists(putanjaDatoteke))
            {
                // U zavisnosti od prosleđene putanje će biti drugačiji root element (po uzoru na primere)
                string rootElement = (putanjaDatoteke.ToLower().Contains("audit")) ? "STAVKE" : "rows";
                XDocument novi = new XDocument(new XDeclaration("1.0", "utf-8", "no"), new XElement(rootElement));
                novi.Save(putanjaDatoteke);
            }

            // Disposable pattern + MemoryStream
            MemoryStream ms = new MemoryStream();

            using (FileStream xml = new FileStream(putanjaDatoteke, FileMode.Open, FileAccess.Read))
            {
                xml.CopyTo(ms);
                xml.Dispose();
            }

            ms.Position = 0;

            return new RadSaDatotekom(ms, Path.GetFileName(putanjaDatoteke));
        }
        #endregion

        #region ČITANJE LOAD
        // Za čitanje iz Load objekata baze
        public List<Load> ProcitajIzBazePodataka(DateTime trazeniDatum)
        {
            List<Load> procitano = new List<Load>();

            using (IRadSaDatotekom datoteka = new XmlBaza().OtvoriDatoteku(ConfigurationManager.AppSettings["LoadBaza"]))
            {
                XmlDocument baza = new XmlDocument();
                baza.Load(((RadSaDatotekom)datoteka).TokPodataka);

                // citanje podataka samo za tekuci dan
                string datum = trazeniDatum.ToString("yyyy-MM-dd");
                
                // Pronađe sve podatke iz baze koji u svom TIMESTAMP imaju prosleđeni datum
                XmlNodeList podaci = baza.SelectNodes("//row[TIME_STAMP[contains(., '" + datum + "')]]");

                foreach (XmlNode podatak in podaci)
                {
                    Load novi = new Load
                    {
                        Id = int.Parse(podatak.SelectSingleNode("ID").InnerText),
                        Timestamp = DateTime.Parse(podatak.SelectSingleNode("TIME_STAMP").InnerText),
                        MeasuredValue = double.Parse(podatak.SelectSingleNode("MEASURED_VALUE").InnerText),
                        ForecastValue = double.Parse(podatak.SelectSingleNode("FORECAST_VALUE").InnerText)
                    };
                    procitano.Add(novi);
                }

                // Dispose resursa za datoteku
                datoteka.Dispose();
            }

            return procitano;
        }
        #endregion

        #region NAJVEĆI ID ZA AUDIT
        public int NajveciAudit()
        {
            int najveciId = 0;
            string putanjaAudit = ConfigurationManager.AppSettings["AuditBaza"];

            using (IRadSaDatotekom datoteka = new XmlBaza().OtvoriDatoteku(putanjaAudit))
            {
                XDocument baza = XDocument.Load(((RadSaDatotekom)datoteka).TokPodataka);
                IEnumerable<XElement> sviId = baza.Descendants("ID");

                try
                {
                    najveciId = sviId.Max(e => int.Parse(e.Value));
                }
                catch 
                {

                }
            }

            return najveciId;
        }
        #endregion

        #region UPIS LOAD
        public void UpisUBazuPodataka(List<Audit> auditi)
        {
            string putanjaAudit = ConfigurationManager.AppSettings["AuditBaza"];

            using (IRadSaDatotekom datoteka = new XmlBaza().OtvoriDatoteku(putanjaAudit))
            {
                XDocument xmlAudit = XDocument.Load(((RadSaDatotekom)datoteka).TokPodataka);

                foreach (Audit a in auditi)
                {
                    XElement stavke = xmlAudit.Element("STAVKE");
                    XElement novi = new XElement("row");

                    novi.Add(new XElement("ID", a.Id));
                    novi.Add(new XElement("TIME_STAMP", a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    novi.Add(new XElement("MESSAGE_TYPE", a.MessageType));
                    novi.Add(new XElement("MESSAGE", a.Message));

                    stavke.Add(novi);
                    xmlAudit.Save(putanjaAudit);
                }

                datoteka.Dispose();
            }
        }
        #endregion
    }
}
