using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Bojana
// Rad sa datotekama treba da bude implemen�ran tako da se vodi računa o održavanju memorije, korišćenjem Dispose paterna

namespace XMLDB
{
    public class XmlBaza : IXmlDb
    {
        // XML baza podataka sadrži XML datoteke u koje se upisuju podaci
        // Svaka tabela je implemen�rana kroz jednu XML datoteku.
        // Ukoliko XML datoteka ne postoji, potrebno je da bude kreirana automatski.
        public bool UpisUXmlBazuLoad(List<Load> tabela)
        {
            string putanjaDoTabele = "TBL_LOAD.xml";

            return false;
        }

        public List<Load> CitanjeIzXmlBazeLoad(DateTime datum)
        {
            string putanjaDoTabele = "TBL_LOAD.xml";

            return null;
        }

        public bool UpisUXmlBazuAudit(Audit podatak)
        {
            string putanjaDoTabele = "TBL_AUDIT.xml";

            return false;
        }
        
        // Nisam sigurna da li nam je ovo neophodno
        public Audit CitanjeIzXmlBazeAudit(DateTime datum)
        {
            string putanjaDoTabele = "TBL_AUDIT.xml";

            return null;
        }
    }
}
