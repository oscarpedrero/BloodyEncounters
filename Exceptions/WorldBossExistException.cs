using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Exceptions
{
    internal class WorldBossExistException : Exception
    {
        public WorldBossExistException()
        {
        }

        public WorldBossExistException(string message)
            : base(message)
        {
        }

        public WorldBossExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WorldBossExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
