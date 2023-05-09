using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum MessageType
    {
        [EnumMember] Error,
        [EnumMember] Info,
        [EnumMember] Warning
    }
}
