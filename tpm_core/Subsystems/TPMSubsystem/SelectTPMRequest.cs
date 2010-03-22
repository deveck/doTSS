// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Subsystems;
using Iaik.Tc.TPM.Context;
using Iaik.Utils;
using System.IO;

namespace Iaik.Tc.TPM.Subsystems.TPMSubsystem
{


	public class SelectTPMRequest : TypedSubsystemRequest<SelectTPMResponse>
	{
		
		private string _tpmIdentifier = null;
		
		/// <summary>
		/// Gets or sets the identifier of the tpm device to select on the server
		/// </summary>
		public string TPMIdentifier
		{
			get { return _tpmIdentifier; }
			set { _tpmIdentifier = value;}
		}
		
		
		public SelectTPMRequest ()
		{
		}
		
		public SelectTPMRequest (string tpmIdentifier)
		{
			_tpmIdentifier = tpmIdentifier;
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_tpmIdentifier = StreamHelper.ReadString (src);
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteString (_tpmIdentifier, sink);
		}
		
	}
	
	
	public class SelectTPMResponse : TPMResponse
	{
		
		private int _tpmSessionIdentifier;
		
		/// <summary>
		/// Gets or sets the session identifier for the last select operation
		/// </summary>
		public int TPMSessionIdentifier
		{
			get { return _tpmSessionIdentifier; }
			set { _tpmSessionIdentifier = value;}
		}
		
		public SelectTPMResponse (SubsystemRequest request, EndpointContext ctx)
			: base(request, ctx)
		{
		}
		
		public override void Read (Stream src)
		{
			base.Read (src);
			
			_tpmSessionIdentifier = StreamHelper.ReadInt32 (_tpmSessionIdentifier);
		}
		
		public override void Write (Stream sink)
		{
			base.Write (sink);
			
			StreamHelper.WriteInt32 (_tpmSessionIdentifier, sink);
		}


	}
}
