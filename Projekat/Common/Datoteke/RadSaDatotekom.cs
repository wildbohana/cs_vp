﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Datoteke
{
    [DataContract]
    public class RadSaDatotekom : IRadSaDatotekom
    {
        private MemoryStream tokPodataka;       // Tok bajtova u kojoj se nalazi trenutno obrađivana datoteka
        private string nazivDatoteke;           // Naziv datoteke koja je trenutno otvorena

        [DataMember]
        public MemoryStream TokPodataka { get => tokPodataka; set => tokPodataka = value; }
        [DataMember]
        public string NazivDatoteke { get => nazivDatoteke; set => nazivDatoteke = value; }

        public RadSaDatotekom(MemoryStream tokPodataka, string nazivDatoteke)
        {
            TokPodataka = tokPodataka;
            NazivDatoteke = nazivDatoteke;
        }

        // Disposable šablon za rad sa datotekama
        public void Dispose()
        {
            if (TokPodataka != null)
            {
                try
                {
                    TokPodataka.Dispose();
                    TokPodataka.Close();
                    TokPodataka = null;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Greška prilikom oslobadjanja memorije datoteke {NazivDatoteke}");
                }
            }
        }
    }
}
