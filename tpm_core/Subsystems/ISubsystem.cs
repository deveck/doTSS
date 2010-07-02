//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.Packets;

namespace Iaik.Tc.TPM.Subsystems
{

	/// <summary>
	/// Implemented by subsystems that receive packets. The subsystems
	/// are registered in the ClientContext where the apropriate handler methods get called
	/// </summary>
	/// <remarks>
	/// The HAndlePAcket method just takes a DataPacket which contains the raw payload transmitted
	/// over the wire. So in general a subsystem could implement its completly own request and no one
	/// else needs to know how they are organized.
	/// 
	/// Nevertheless it is also possible to use a 'predefined-request-format' where extraction methods
	/// are already available
	/// </remarks>
	public interface ISubsystem : IDisposable
	{
		/// <summary>
		/// Gets the unique Subsystem identifier e.g: auth, admin, tpm, ...
		/// </summary>
		string SubsystemIdentifier{ get; }

		/// <summary>
		/// Tells the subsystem to handle the given DataPacket
		/// </summary>
		void HandlePacket(DataPacket packet);
	}
}
