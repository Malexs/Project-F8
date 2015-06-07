using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_F8.Exeptions
{
    [Serializable]
    public class inputException:Exception
    {
        public inputException() : base() { }
        public inputException(string message) : base(message) { }
        public inputException(string message, Exception inner) : base(message, inner) { }
        protected inputException(
            System.Runtime.Serialization.SerializationInfo si,
            System.Runtime.Serialization.StreamingContext sc)
            : base(si, sc) { }

        public override string ToString()
        {
            return Message;
        }
    }
}
