// 
// 
//  Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
//  Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Tc.TPM.Context;
using Iaik.Connection.Packets;
using System.IO;
using Iaik.Utils;

namespace Iaik.Tc.TPM.Subsystems
{

	/// <summary>
	/// Baseclass for all subsystem responses
	/// </summary>
	public abstract class SubsystemResponse : SubsystemRequest
	{
		public enum CommonErrorCodes : int
		{
			/// <summary>
			/// No active user authentication exists 
			/// </summary>
			NotAuthenticated = 0,

			/// <summary>
			/// There is an active user authentication but no permission to perform this action 
			/// </summary>
			NotPermitted,

			/// <summary>
			/// Indicates the first error code that can be used for custom error codes
			/// </summary>
			CustomErrorCodeBase
		}
		
		/// <summary>
		/// The request this response belongs to
		/// </summary>
		protected SubsystemRequest _request;
		
		
		protected bool _succeeded = true;

		/// <summary>
		/// Indicates if the response has succeeded or if it has produced an error
		/// </summary>
		public bool Succeeded {
			get { return _succeeded; }
			set { _succeeded = value; }
		}

		protected int? _errorCode = null;

		/// <summary>
		/// The error code. If the error code is -1, then a custom message is sent right after the code in the stream
		/// The error code is only transmitted if the request did not succeed
		/// </summary>
		public int? ErrorCode {
			get { return _errorCode; }
			set { _errorCode = value; }
		}

		/// <summary>
		/// Trys to convert the error code to a human readable string
		/// </summary>
		public string ErrorText {
			get {
				if (_errorCode == null)
					return "No error"; 
				else if (_errorCode.Value == -1)
					return CustomErrorMessage; 
				else if (_errorCode.Value == (int)CommonErrorCodes.NotAuthenticated)
					return "No authentication information for your connection was found"; 
				else if (_errorCode.Value == (int)CommonErrorCodes.NotPermitted)
					return "You are not permitted to perform this operation";
				else
				{
					string internalErrorText = InternalErrorText;
					if (internalErrorText == null)
						return "Unknown error";
					else
						return internalErrorText;
				}
					
			}
		}
		
		protected abstract string InternalErrorText{ get; }

		private string _customErrorMessage = null;

		/// <summary>
		/// The custom error message, only transmitted if _errorCode is -1
		/// </summary>
		public string CustomErrorMessage {
			get { return _customErrorMessage; }
			set {
				if (value == null)
					_errorCode = null;
				else
					_errorCode = -1;
				_customErrorMessage = value;
			}
		}

		public SubsystemResponse (SubsystemRequest request, EndpointContext ctx)
			:base(ctx)
		{
			_request = request;
		}
		
		/// <summary>
		/// No responses to responses
		/// </summary>
		public override Type ResponseType 
		{
			get { return null; }
		}
		
		public override ushort RequestIdentifier 
		{
			get { return _request.RequestIdentifier; }
		}
		
		public override string Subsystem 
		{
			get { return _request.Subsystem; }
		}
		
		public override DataPacket ConvertToDataPacket ()
		{
			using (MemoryStream sink = new MemoryStream ())
			{
				StreamHelper.WriteUInt16 (RequestIdentifier, sink);
				Write (sink);
				
				sink.Flush ();
				DataPacket packet = new DataPacket (true, Subsystem, sink.ToArray ());
				packet.PacketNr = _request.PacketIdentifier.PacketNr;
				
				return packet;
			}
		
		}
		
		public void SetKnownCommonError (CommonErrorCodes commonError)
		{
			_errorCode = (int)commonError;
		}


		public override void Read(Stream src)
        {
            base.Read(src);

            _succeeded = StreamHelper.ReadBool(src);

            if (_succeeded == false)
            {
                _errorCode = StreamHelper.ReadNullableInt32(src);

                if (_errorCode != null && _errorCode.Value == -1)
                    _customErrorMessage = StreamHelper.ReadString(src);

            }
        }

        public override void Write(Stream sink)
        {
            base.Write(sink);

            StreamHelper.WriteBool(_succeeded, sink);

            if (_succeeded == false)
            {
                StreamHelper.WriteNullableInt32(_errorCode, sink);

                if (_errorCode != null && _errorCode.Value == -1)
                    StreamHelper.WriteString(_customErrorMessage, sink);
            }
        }
	
		/// <summary>
		/// Checks if the Request has succeeded. If not it throws an SubsystemRequestException
		/// </summary>
		public void AssertResponse()
		{
			if(_succeeded == false && _errorCode != null)
				throw new SubsystemResponseException(this, _errorCode.Value, ErrorText);
			else if(_succeeded == false && _errorCode == null)
				throw new SubsystemResponseException(this, -2, ErrorText);
			
		}

 
	}
}
