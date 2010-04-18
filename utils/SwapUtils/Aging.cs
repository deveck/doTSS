// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Utils.Replacement
{


	public sealed class Aging : IReplacementAlgorithm
	{
		const UInt64 UPDATE_MASK = 0x8000000000000000;
		const UInt64 NEW_MASK = 0xffffffffffffffff;
		List<KeyValuePair<UInt64, UInt64>> _used = new List<KeyValuePair<UInt64, UInt64>>();
		List<UInt64> _swaped = new List<UInt64>();
		UInt64 _nextID = 0;
		
		bool _isSorted;
		
		public Aging ()
		{
		}
		
		#region IReplacementAlgorithm<TypeID> implementation
		public void SwapIn(System.Collections.Generic.List<UInt64> ids)
		{
			lock(this)
			{
				foreach(UInt64 id in ids)
				{
					_swaped.Remove(id);
					InsertUsed(id);
				}
			}
		}
		
		public void SwapIn(UInt64 id)
		{
			lock(this)
			{
				_swaped.Remove(id);
				InsertUsed(id);
			}
		}
		
		public void SwapOut(System.Collections.Generic.List<UInt64> ids)
		{
			lock(this)
			{
				foreach(UInt64 id in ids)
				{
					foreach(KeyValuePair<UInt64, UInt64> pair in _used)
					{
						if(pair.Key == id)
						{
							_used.Remove(pair);
							_swaped.Add(id);
							return;
						}
					}
				}
				throw new KeyNotFoundException();
			}
		}
		
		public void SwapOut(UInt64 id)
		{
			lock(this)
			{
				foreach(KeyValuePair<UInt64, UInt64> pair in _used)
				{
					if(pair.Key == id)
					{
						_used.Remove(pair);
						_swaped.Add(id);
						return;
					}
				}
				throw new KeyNotFoundException();
			}
		}
		
		
		public bool IsSwaped (UInt64 id)
		{
			foreach(UInt64 item in _swaped)
			{
				if(item == id)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Update the age of all items in current set. Used would get younger. If used==null, all items in set would get older.
		/// </summary>
		/// <param name="used">
		/// A <see cref="List<UInt64>"/>
		/// </param>
		public void Update(List<UInt64> used)
		{
			lock(this)
			{
				_isSorted = false;
				// create a new list, because KeyValuePairs can't be changed in place
				List<KeyValuePair<UInt64, UInt64>> helper = new List<KeyValuePair<UInt64, UInt64>>();
				// step through all items
				foreach(KeyValuePair<UInt64, UInt64> pair in _used)
				{
					KeyValuePair<UInt64, UInt64> newpair;
					// update if it was used
					if(used != null && used.Contains(pair.Key))
					{
						newpair =  new KeyValuePair<UInt64,UInt64>(pair.Key, (pair.Value >> 1) | UPDATE_MASK);
						used.Remove(pair.Key);
					}
					// also if not
					else
					{
						newpair =  new KeyValuePair<UInt64,UInt64>(pair.Key, pair.Value >> 1);
					}
					helper.Add(newpair);
				}
				_used = helper;
			}
		}
		
		public List<UInt64> Swapables
		{
			get
			{
				lock(this)
				{
					if(!_isSorted)
						_used.Sort(delegate(KeyValuePair<UInt64, UInt64> firstpair,
						                    KeyValuePair<UInt64, UInt64> nextpair){
						return firstpair.Value.CompareTo(nextpair.Value);
					});
					_isSorted = true;
					List<UInt64> ret = new List<UInt64>();
					foreach(KeyValuePair<UInt64, UInt64> pair in _used)
						ret.Add(pair.Key);
					return ret;
				}
			}
		}
		
		public UInt64 RegisterNew()
		{
			lock(this)
			{
				InsertUsed(_nextID);
				++_nextID;
				return _nextID - 1;
			}
		}
		
		
		public void Delete (UInt64 item)
		{
			lock(this)
			{
				foreach(KeyValuePair<UInt64, UInt64> pair in _used)
				{
					if(pair.Key == item)
					{
						_used.Remove(pair);
						return;
					}
				}
				foreach(UInt64 id in _swaped)
				{
					if(id == item)
					{
						_swaped.Remove(id);
						return;
					}
				}
			}
		}
		
		#endregion
		
		private void InsertUsed(UInt64 id)
		{
			KeyValuePair<UInt64, UInt64> pair = new KeyValuePair<UInt64, UInt64>(id, NEW_MASK);
			_used.Add(pair);
			_isSorted = false;
		}
	}
}
