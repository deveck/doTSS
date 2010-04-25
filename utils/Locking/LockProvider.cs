// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Utils.Locking
{


	public class LockProvider
	{
		private object _lockTarget;

		public LockProvider (object lockTarget)
		{
			_lockTarget = lockTarget;
		}
		
		public LockContext AcquireLock()
		{
			return new LockContext(_lockTarget);
		}
	}
}
