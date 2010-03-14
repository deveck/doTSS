
using System;

namespace Iaik.Utils.Serialization
{

	/// <summary>
	/// Attach to members of derived classes of AutoStreamSerializable to get them
	/// automatically serialized.
	/// </summary>
	public class SerializeMeAttribute : Attribute
	{
		private int _ordinal;
		
		/// <summary>
		/// Gets the in stream position of the attached member
		/// </summary>
		public int Ordinal
		{
			get { return _ordinal; }
		}
		
		public SerializeMeAttribute (int ordinal)
		{
			_ordinal = ordinal;
		}
	}
}
