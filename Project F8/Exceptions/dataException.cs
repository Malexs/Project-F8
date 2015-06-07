using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_F8.Exeptions
{
    [Serializable]
    public class dataException:Exception
    {
        public dataException() : base() { }
        public dataException(string message) : base(message) { }
        public dataException(string message, Exception inner) : base(message, inner) { }
        protected dataException(
            System.Runtime.Serialization.SerializationInfo si,
            System.Runtime.Serialization.StreamingContext sc)
            : base(si, sc) { }

        public override string ToString()
        {
            return Message;
        }
    }
}
