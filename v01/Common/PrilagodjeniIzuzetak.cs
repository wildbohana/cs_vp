using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class PrilagodjeniIzuzetak
    {
        // Polje
        private string poruka;

        // Properti
        [DataMember]
        public string Poruka { get => poruka; set => poruka = value; }

        // Konstruktor
        public PrilagodjeniIzuzetak(string prk)
        {
            this.Poruka = prk;
        }
    }
}
