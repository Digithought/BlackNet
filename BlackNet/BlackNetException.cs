using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Digithought.BlackNet
{
	[Serializable]
	public class BlackNetException : Exception, ISerializable
	{
		public BlackNetException(string message) : base(message)
		{
		}

		protected BlackNetException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}