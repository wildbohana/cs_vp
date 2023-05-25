using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Load
    {
        private int id;
        private DateTime timestamp;
        private double forecastValue;
        private double measuredValue;

        [DataMember]
        public int Id { get => id; set => id = value; }
        [DataMember]
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        [DataMember]
        public double ForecastValue { get => forecastValue; set => forecastValue = value; }
        [DataMember]
        public double MeasuredValue { get => measuredValue; set => measuredValue = value; }
        
        // Konstruktori
        public Load() { }

        public Load(int id, DateTime timestamp, double forecastValue, double measuredValue)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.forecastValue = forecastValue;
            this.measuredValue = measuredValue;
        }

        // Ispis
        public override string ToString()
        {
            return $"[{Id}]: {Timestamp} - {MeasuredValue} ({ForecastValue})";
        }

        // Equals
        public override bool Equals(object obj)
        {
            return obj is Load l && Id == l.Id && Timestamp == l.Timestamp && MeasuredValue == l.MeasuredValue && ForecastValue == l.forecastValue;
        }

        // GetHashCode - da ne bi izbacivao Warning
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
