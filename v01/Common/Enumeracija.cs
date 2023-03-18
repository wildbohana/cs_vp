using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Enumeracija
    {
        [DataContract]
        public enum Zanrovi
        {
            [EnumMember]Krimi,
            [EnumMember]Ljubav,
            [EnumMember]Drama,
            [EnumMember]Scifi,
            [EnumMember]Unknown
        }
    }
}
