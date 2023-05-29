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

namespace XMLDB
{
    public class XmlBaza : IXmlDb
    {
        #region OTVARANJE XML
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

            // Otvaramo XML datoteku i sve podatke iz nje kopiramo u Memory Stream koji vraćamo pozivaocu metode
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
        public List<Load> ProcitajIzBazePodataka(DateTime trazeniDatum)
        {
            List<Load> procitano = new List<Load>(24);

            // Pri instanciranju promenljive datoteka pozivamo metodu OtvoriDatoteku
            // Ona učitava sve bajtove iz XML baze za Load podatke
            using (IRadSaDatotekom datoteka = new XmlBaza().OtvoriDatoteku(ConfigurationManager.AppSettings["LoadBaza"]))
            {
                XmlDocument baza = new XmlDocument();
                baza.Load(((RadSaDatotekom)datoteka).TokPodataka);

                // Pretraga podataka sa traženim datumom (TIME_STAMP sadrži taj datum)
                string datum = trazeniDatum.ToString("yyyy-MM-dd");
                XmlNodeList podaci = baza.SelectNodes("//row[TIME_STAMP[contains(., '" + datum + "')]]");

                foreach (XmlNode podatak in podaci)
                {
                    Load novi = new Load
                    {
                        Id = int.Parse(podatak.SelectSingleNode("ID").InnerText),
                        Timestamp = DateTime.Parse(podatak.SelectSingleNode("TIME_STAMP").InnerText),
                        MeasuredValue = Convert.ToDouble(podatak.SelectSingleNode("MEASURED_VALUE").InnerText.Replace(".", ",")),
                        ForecastValue = Convert.ToDouble(podatak.SelectSingleNode("FORECAST_VALUE").InnerText.Replace(".", ","))
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

        #region UPIS AUDIT
        public void UpisUBazuPodataka(Audit a)
        {
            string putanjaAudit = ConfigurationManager.AppSettings["AuditBaza"];

            using (IRadSaDatotekom datoteka = new XmlBaza().OtvoriDatoteku(putanjaAudit))
            {
                XDocument xmlAudit = XDocument.Load(((RadSaDatotekom)datoteka).TokPodataka);

                XElement stavke = xmlAudit.Element("STAVKE");
                XElement novi = new XElement("row");

                novi.Add(new XElement("ID", a.Id));
                novi.Add(new XElement("TIME_STAMP", a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                novi.Add(new XElement("MESSAGE_TYPE", a.MessageType));
                novi.Add(new XElement("MESSAGE", a.Message));

                stavke.Add(novi);
                xmlAudit.Save(putanjaAudit);
                
                datoteka.Dispose();
            }
        }
        #endregion
    }
}
