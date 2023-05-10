using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Audit
    {
        private int id;
        private DateTime timestamp;
        private MessageType messageType;
        private string message;

        [DataMember]
        public int Id { get => id; set => id = value; }
        [DataMember]
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        [DataMember]
        public MessageType MessageType { get => messageType; set => messageType = value; }
        [DataMember]
        public string Message { get => message; set => message = value; }

        // Konstruktori
        public Audit() { }

        public Audit(int id, DateTime timestamp, MessageType messageType, string message)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.messageType = messageType;
            this.message = message;
        }

        // Ispis
        public override string ToString()
        {
            return $"{Timestamp} [{MessageType}]: {Message} (ID: {Id})";
        }

        // Equals
        public override bool Equals(object obj)
        {
            return obj is Audit a && Id == a.Id && Timestamp == a.Timestamp && MessageType == a.MessageType && Message == a.Message;
        }

        // GetHashCode - da ne bi izbacivao Warning
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
