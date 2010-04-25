// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Threading;

namespace Iaik.Utils.Locking
{


	/// <summary>
	/// Provides a disposable lock 
	/// </summary>
	public class LockContext : IDisposable
	{
		private object _lockTarget;
		
		public LockContext (object lockTarget)
		{
			_lockTarget = lockTarget;
			Monitor.Enter(_lockTarget);
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			Monitor.Exit(_lockTarget);
		}
		
		#endregion

		
	}
}
