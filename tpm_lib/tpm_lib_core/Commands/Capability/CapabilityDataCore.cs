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
using Iaik.Tc.TPM.Library.Common;
using Iaik.Tc.TPM.Lowlevel.Data;
using Iaik.Utils;
using Iaik.Utils.Serialization;
using System.IO;

namespace Iaik.Tc.TPM.Library.Commands
{


	public static class CapabilityDataCore
	{
		[TypedStreamSerializable("TPMSelectSize")]
		public class TPMSelectSizeCore : CapabilityData.TPMSelectSize, ITPMBlobWritable
		{
			public static TPMSelectSizeCore CreateVersion12(ushort size)
			{
				TPMSelectSizeCore selectSize = new TPMSelectSizeCore();
				selectSize._major = 1;
				selectSize._minor = 2;
				selectSize._reqSize = size;
				return selectSize;
			}
			
			private TPMSelectSizeCore()
			{
			}
			
			public TPMSelectSizeCore(Stream src)
				:base(src)
			{
			}
			
			#region ITPMBlobWritable implementation
			public void WriteToTpmBlob (TPMBlob blob)
			{
				blob.WriteByte(_major);
				blob.WriteByte(_minor);
				blob.WriteUInt16(_reqSize);
			}
			
			#endregion
			
		}
		
		[TypedStreamSerializable(CapabilityData.PARAM_TPM_VERSION_INFO)]
		public class TPMCapVersionInfoCore : CapabilityData.TPMCapVersionInfo, ITPMBlobReadable
		{
			public TPMCapVersionInfoCore (TPMBlob blob)
			{
				ReadFromTpmBlob(blob);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TPMBlob blob)
			{
				/*uint responseSize = */blob.ReadUInt32 ();
				
				this._tag = blob.ReadUInt16 ();
				this._version = new TPMVersionCore (blob);
				this._specLevel = blob.ReadUInt16 ();
				this._errataRev = blob.ReadByte ();
				this._tpmVendorId = new byte[4];
				blob.Read (_tpmVendorId, 0, _tpmVendorId.Length);
				
				ushort vendorSize = blob.ReadUInt16 ();
				_vendorSpecific = blob.ReadBytes (vendorSize);				
				
			}
			
			#endregion
			
		}
		
		[TypedStreamSerializable("TPMVersion")]
		public class TPMVersionCore : CapabilityData.TPMVersion, ITPMBlobReadable, ITPMBlobWritable
		{
			/// <summary>
			/// Creates a default tpm v1.2 version structure
			/// </summary>
			/// <returns</returns>
			public static TPMVersionCore CreateVersion12 ()
			{
				CapabilityDataCore.TPMVersionCore version = new CapabilityDataCore.TPMVersionCore ();
				version._major = 0x01;
				version._minor = 0x02;
				version._revMajor = 0;
				version._revMinor = 0;
				
				return version;
			}
			
			public static TPMVersionCore CreateVersion11 ()
			{
				CapabilityDataCore.TPMVersionCore version = new CapabilityDataCore.TPMVersionCore ();
				version._major = 0x01;
				version._minor = 0x01;
				version._revMajor = 0;
				version._revMinor = 0;
				
				return version;
			}
			
			public static TPMVersionCore CreateFromTPMBlob(TPMBlob blob)
			{
				TPMVersionCore version = new TPMVersionCore();
				version.ReadFromTpmBlob(blob);
				return version;
			}
			
			private TPMVersionCore ()
			{
			}
			
			public TPMVersionCore (Stream src)
			{
				Read(src);
			}
			
			#region ITpmBlobReadable implementation
			public void ReadFromTpmBlob (TPMBlob blob)
			{
				_major = blob.ReadByte ();
				_minor = blob.ReadByte ();
				_revMajor = blob.ReadByte ();
				_revMinor = blob.ReadByte ();
			}
			
			#endregion
			#region ITPMBlobWritable implementation
			public void WriteToTpmBlob (TPMBlob blob)
			{
				blob.WriteByte (_major);
				blob.WriteByte (_minor);
				blob.WriteByte (_revMajor);
				blob.WriteByte (_revMinor);
			}
			
			#endregion
		}
	}
}
