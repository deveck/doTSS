
using System;
using Iaik.Utils.Serialization;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Library.Common.KeyData
{
	
	

	public class TPMKeyParams : AutoStreamSerializable
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
		protected ITypedStreamSerializable _params;
		
		/// <summary>
		/// Parameter information dependant upon the key algorithm
		/// </summary>
		public ITypedStreamSerializable Params
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
