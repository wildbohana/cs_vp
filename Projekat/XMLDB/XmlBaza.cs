using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Bojana
// Rad sa datotekama treba da bude implementiran tako da se vodi računa o održavanju memorije, korišćenjem Dispose paterna
// XML baza podataka sadrži XML datoteke u koje se upisuju podaci
// Svaka tabela je implemen�rana kroz jednu XML datoteku.
// Ukoliko XML datoteka ne postoji, potrebno je da bude kreirana automatski.

namespace XMLDB
{
    // AAAALLLLOOOO STREAM WRITER VALJDA PAMTI TRENUTNU POZICIJU/RED U FAJLU
    // DA LI JE PREKO POSITION ILI ČEGA VEĆ; NE ZNAM
    // PREKO TOGA SKONTATI KOJI JE RED U TABELI; I ONDA VRATI REČNIK

    public class XmlBaza : IXmlDb
    {   
        public bool UpisUXmlBazuLoad(List<Load> tabela)
        {
            string putanjaDoTabele = "TBL_LOAD.xml";

            return false;
        }

        public Dictionary<int, Load> CitanjeIzXmlBazeLoad(DateTime datum)
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
