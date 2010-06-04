// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Utils.Locking
{


	public class LockProvider
	{
		private object _lockTarget;
		private string _description;

		public LockProvider (object lockTarget, string description)
		{
			_lockTarget = lockTarget;
			_description = description;
		}
		
		public LockContext AcquireLock()
		{
			return new LockContext(_lockTarget, _description);
		}
	}
}
