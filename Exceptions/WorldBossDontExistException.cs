using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Exceptions
{
    internal class WorldBossDontExistException : Exception
    {
        public WorldBossDontExistException()
        {
        }

        public WorldBossDontExistException(string message)
            : base(message)
        {
        }

        public WorldBossDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WorldBossDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
