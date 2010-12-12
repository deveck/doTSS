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
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{
	
	
	[TypedStreamSerializable("TPMKeyParams")]
	public class TPMKeyParams : AutoStreamSerializable, ITypedParameter
	{

		[SerializeMe(0)]
		protected TPMAlgorithmId _algorithmId;

		/// <summary>
		/// Key algorithm in use
		/// </summary>
		public TPMAlgorithmId AlgorithmId
		{
			get { return _algorithmId; }
		}
		
		[SerializeMe(1)]
		protected TPMEncScheme _encScheme;
		
		/// <summary>
		/// Encryption scheme in use
		/// </summary>
		public TPMEncScheme EncScheme
		{
			get { return _encScheme; }
		}
		
		[SerializeMe(2)]
		protected TPMSigScheme _sigScheme;
		
		/// <summary>
		/// Signature scheme in use
		/// </summary>
		public TPMSigScheme SigScheme
		{
			get { return _sigScheme; }
		}
		
		[SerializeMe(3)]
		protected ITPMAlgorithmSpecificKeyParams _params;
		
		/// <summary>
		/// Parameter information dependant upon the key algorithm
		/// </summary>
		public ITPMAlgorithmSpecificKeyParams Params
		{
			get { return _params;}
		}
		
		protected TPMKeyParams ()
		{
		}
		
		public TPMKeyParams (Stream src)
		{
			Read (src);
		}
		
		public override string ToString ()
		{
			return string.Format ("AlgorithmId: {0}\nEncScheme: {1}\nSigScheme:{2}\nParams:\n{3}", AlgorithmId, EncScheme, 
				SigScheme, Params == null?"   <null>":StringHelper.IndentPerLine(Params.ToString(), "   "));
		}

	}
}
