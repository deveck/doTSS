/* Copyright 2010 Andreas Reiter <andreas.reiter@student.tugraz.at>, 
 *                Georg Neubauer <georg.neubauer@student.tugraz.at>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */



using System;
using Iaik.Tc.TPM.Lowlevel.Data;

namespace Iaik.Tc.TPM.Library
{

	/// <summary>
	/// Implemented by classes that can serilize to tpm blobs
	/// </summary>
	public interface ITPMBlobWritable
	{
		void WriteToTpmBlob(TPMBlob blob);
	}
	
	public static class TPMBlobWriteableHelper
	{
		/// <summary>
		/// Writes target to sink with the uint size preceding
		/// </summary>
		/// <param name="sink"></param>
		/// <param name="target"></param>
		public static void WriteITPMBlobWritableWithUIntSize (TPMBlob sink, ITPMBlobWritable target)
		{
			if (target == null)
			{
				sink.WriteUInt32 (0);
			}
			else
			{
				using (TPMBlob tempBlob = new TPMBlob ())
				{	
					target.WriteToTpmBlob (tempBlob);
					sink.WriteUInt32 ((uint)tempBlob.Length);
					sink.Write (tempBlob.ToArray (), 0, (int)tempBlob.Length);
				}
			}
		}
	}
}
