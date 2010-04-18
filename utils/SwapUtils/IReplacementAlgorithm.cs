// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Utils.Replacement
{


	public interface IReplacementAlgorithm
	{
		void SwapIn(List<UInt64> ids);
		void SwapIn(UInt64 id);
		void SwapOut(List<UInt64> ids);
		void SwapOut(UInt64 id);
		bool IsSwaped(UInt64 id);
	//	List<TypeID> GetSwapables();
		void Update(List<UInt64> used);
		UInt64 RegisterNew();
		void Delete(UInt64 item);
		List<UInt64> Swapables{get;}
	//	void init(Parameters param);
	}
}
