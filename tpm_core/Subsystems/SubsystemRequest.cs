//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Tc.Tpm.Connection.Packets;
using Iaik.Utils;

namespace Iaik.Tc.Tpm.Subsystems
{
	/// <summary>
	/// Implements request base class for all default subsystem implementations.
	/// </summary>
	public abstract class SubsystemRequest : IStreamSerializable
	{

		/// <summary>
		/// Converts this request to a default representation of a DataPacket
		/// </summary>
		/// <returns>
		/// A <see cref="DataPacket"/>
		/// </returns>
		public virtual DataPacket ConvertToDataPacket()
		{
			
			using(MemoryStream sink = new MemoryStream())
			{
				StreamHelper.WriteUInt16(RequestIdentifier, sink);
				Write(sink);
				
				sink.Flush();
				DataPacket packet = new DataPacket(false, Subsystem, sink.ToArray());
				return packet;
			}
			
			
			
		}

		public abstract string Subsystem{get;}
		public abstract ushort RequestIdentifier{get;}
		
		#region IStreamSerializable implementation
		public virtual void Write (Stream sink)
		{
		}
		
		
		public virtual void Read (Stream src)
		{
		}
		
		#endregion
		
	}
}
