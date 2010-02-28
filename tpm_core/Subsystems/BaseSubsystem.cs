//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.Packets;
using Iaik.Utils;
using Iaik.Tc.Tpm.Context;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Iaik.Tc.Tpm.Subsystems
{

	public delegate void HandleSubsystemRequestDelegate<T, U>(RequestContext<T, U> requestCtx)
		where T: SubsystemRequest
		where U: SubsystemResponse;
	/// <summary>
	/// Implements a basic subsystem, where all requests are
	/// build like the standard requests
	/// </summary>
	/// <remarks>
	/// The BaseSubsystem implements a default behaviour and default extraction method
	/// for packet handling and packet extraction
	/// </remarks>
	public abstract class BaseSubsystem<TRequest> : ISubsystem
	{
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	
		/// <summary>
		/// The Client/Server context this subsystm runs in
		/// </summary>
		protected EndpointContext _context;
		
		/// <summary>
		/// Contains information for every request type, how to process them
		/// </summary>
		protected Dictionary<TRequest, RequestExecutionInfo> _requestExecutionInfos = 
			new Dictionary<TRequest, RequestExecutionInfo>();
		
		public BaseSubsystem(EndpointContext context)
		{
			_context = context;
			
			//The ctor needs to check if T is a enum type and if T has ushort as base type,
			//because this can not be done with type-constraints
			Type requestType =  typeof(TRequest);
			
			if(requestType.IsEnum == false || Enum.GetUnderlyingType(requestType) != typeof(ushort))
				throw new ArgumentException("TypeArgument TRequest is not valid, it has to be an enum type with ushort as the underlying type");
		}
		
		/// <summary>
		/// Builds a RequestExecutionInfo object needed to register a new request in the BaseSubsystem
		/// </summary>
		protected RequestExecutionInfo BuildRequestExecutionInfo<T, U>(
		        HandleSubsystemRequestDelegate<T, U> callback)
			where T: SubsystemRequest
			where U: SubsystemResponse
		{
			return new RequestExecutionInfo(typeof(T), callback);
			                                
		}
		
		#region ISubsystem implementation
		public virtual void HandlePacket (DataPacket packet)
		{
			using(ByteStream src = new ByteStream(packet.Payload))
			{
				TRequest requestTypeIdentifier = (TRequest)Enum.ToObject(typeof(TRequest), StreamHelper.ReadUInt16(src));
				SubsystemRequest request = CreateRequestFromIdentifier(requestTypeIdentifier);
				request.Read(src);
				request.PacketIdentifier = new PacketIdentifier(packet.PacketNr);
				
				ExecuteRequest(requestTypeIdentifier, request);
			}
		}
		
		public abstract string SubsystemIdentifier { get; }		
		#endregion

		
		private void ExecuteRequest(TRequest requestTypeIdentifier, SubsystemRequest request)
		{
			if(_requestExecutionInfos.ContainsKey(requestTypeIdentifier) == false)
				throw new NotSupportedException("Not supported request detected");
			
			
			Type requestCtxType = typeof(RequestContext<,>);
			requestCtxType = requestCtxType.MakeGenericType(request.GetType(), request.ResponseType);
			
			ConstructorInfo ctor = requestCtxType.GetConstructor(new Type[]{request.GetType(), typeof(EndpointContext)});
			if(ctor == null)
				throw new NotSupportedException("RequestContext does not contain apropriate constructor");
			
			object requestContext = ctor.Invoke(new object[]{request, _context});
			
			_logger.DebugFormat("Executing request '{0}'", request);
			_requestExecutionInfos[requestTypeIdentifier].Callback.DynamicInvoke(requestContext);
		}
		
		/// <summary>
		/// Creates a SubsystemRequest from an identifier extracted from the DataPacket
		/// </summary>
		protected virtual SubsystemRequest CreateRequestFromIdentifier (TRequest requestType)
		{
			if(_requestExecutionInfos.ContainsKey(requestType))
			{
				Type t = _requestExecutionInfos[requestType].RequestType;
				ConstructorInfo ctor = t.GetConstructor(new Type[]{typeof(EndpointContext)});
				
				if(ctor == null)
					throw new NotSupportedException(string.Format("'{0}' does not have a default ctor!",t));
				
				return (SubsystemRequest)ctor.Invoke(new object[]{_context});
			}
			else
				throw new NotImplementedException(string.Format("Request type '{0}' not implemented", requestType));
		}
		
		
		/// <summary>
		/// Holds some information for the available requests, this
		/// subsystem can process
		/// </summary>
		protected class RequestExecutionInfo
		{
			/// <summary>
			/// Defines the type of the request
			/// </summary>
			private Type _requestType;
			
			public Type RequestType
			{
				get{ return _requestType; }
			}
			
			/// <summary>
			/// Defines the callback which is called to handle such a request
			/// </summary>
			private Delegate _callback;
			
			public Delegate Callback
			{
				get{ return _callback;}
			}
			
			public RequestExecutionInfo(Type requestType, Delegate callback)
			{
				_requestType = requestType;
				_callback = callback;
			}
		}
	}
}
