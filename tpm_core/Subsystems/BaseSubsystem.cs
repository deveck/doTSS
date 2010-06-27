//
//
// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Connection.Packets;
using Iaik.Utils;
using Iaik.Tc.TPM.Context;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Iaik.Tc.TPM.Configuration;

namespace Iaik.Tc.TPM.Subsystems
{

    /// <summary>
    /// Specifies a generic, typed delegate for handling default requests.
    /// </summary>
    /// <remarks>
    /// The typed parameters eliminate the need to cast the incoming parameters in the handler method, they come in with the correct
    /// type for the request
    /// </remarks>
    /// <typeparam name="Z">The Type of the subsystem this request belongs to</typeparam>
    /// <typeparam name="T">The type of request</typeparam>
    /// <typeparam name="U">The type of response the specified request generates</typeparam>
    /// <param name="subsystem"></param>
    /// <param name="requestCtx"></param>
    public delegate void HandleSubsystemRequestDelegate<Z, T, U>(Z subsystem, RequestContext<T, U> requestCtx)
		where T: SubsystemRequest
		where U: SubsystemResponse
        where Z: ISubsystem;

	/// <summary>
	/// Implements a basic subsystem, where all requests follow a predefined request-format.
	/// </summary>
    /// <typeparam name="TRequest">Specifies the type used to differentiate between different requests (typically an enum type), 
    /// needs to be of type ushort or an enum type with ushort base</typeparam>
	/// <remarks>
	/// The BaseSubsystem implements a default behaviour and default extraction method
	/// for packet handling and packet extraction, it can not be instantiated but eases the way of subsystem implementation
    /// 
    /// See the example for a common way of using the BaseSubsystem
	/// </remarks>
    /// 
    /// <code>
    ///
    /// public class TestSubsystem : BaseSubsystem<TestSubsystem.TestSubsystemRequests>
    /// {
    ///    // Specifies the different requests
    ///    public enum TestSubsystemRequests : ushort
    ///    {
    ///        TestRequest = 0x0001
    ///    }

    ///    public override string SubsystemIdentifier
    ///    {
    ///        get { return "TEST_SUBSYSTEM"; }
    ///    }


    ///    public TestSubsystem(EndpointContext ctx)
    ///    {
    ///        _requestExecutionInfos.Add(TestSubsystemRequests.TestRequest,
    ///              BuildRequestExecutionInfo<TestSubsystem, TestRequest, TestResponse>(HandleTestResponse));
    ///    }


    ///    private void HandleTestResponse(TestSubsystem subsystem,
    ///            RequestContext<TestRequest, TestResponse> requestCtx)
    ///    {
    ///        //Handle request here
    ///    }

    ///}
    /// </code>
	public abstract class BaseSubsystem<TRequest> : ISubsystem
	{
		/// <summary>
		/// Logger
		/// </summary>
		protected ILog _logger;
	
		/// <summary>
		/// The Client/Server context this subsystm runs in
		/// </summary>
		protected EndpointContext _context;
		
		/// <summary>
		/// Contains information for every request type, how to process them
		/// </summary>
		protected Dictionary<TRequest, RequestExecutionInfo> _requestExecutionInfos = 
			new Dictionary<TRequest, RequestExecutionInfo>();

        
        /// <summary>
        /// Returns the associated <see>EndpointContext</see>
        /// </summary>
        internal EndpointContext EndpointContext
        {
            get { return _context; }
        }

		public BaseSubsystem(EndpointContext context)
		{
			_logger = LogManager.GetLogger(SubsystemIdentifier + "-subsystem");
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
		protected RequestExecutionInfo BuildRequestExecutionInfo<Z, T, U>(
		        HandleSubsystemRequestDelegate<Z, T, U> callback)
			where T: SubsystemRequest
			where U: SubsystemResponse
            where Z: ISubsystem
		{
			return new RequestExecutionInfo(typeof(T), callback);
			                                
		}
		
		#region ISubsystem implementation
        /// <summary>
        /// Reads the RequestType from the stream, creates the appropriate request object, populates it and executes the request (calls the handler)
        /// </summary>
        /// <param name="packet"></param>
		public virtual void HandlePacket (DataPacket packet)
		{
			_logger.DebugFormat("Handling request in '{0}'", SubsystemIdentifier);
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
			
			_logger.DebugFormat("Executing request in '{0}'", SubsystemIdentifier);
			
			Type requestCtxType = typeof(RequestContext<,>);
			requestCtxType = requestCtxType.MakeGenericType(request.GetType(), request.ResponseType);
			
			ConstructorInfo ctor = requestCtxType.GetConstructor(new Type[]{request.GetType(), typeof(EndpointContext)});
			if(ctor == null)
				throw new NotSupportedException("RequestContext does not contain apropriate constructor");
			
			object requestContext = ctor.Invoke(new object[]{request, _context});
			
			_logger.DebugFormat("Executing request '{0}'", request);
			_requestExecutionInfos[requestTypeIdentifier].Callback.DynamicInvoke(this, requestContext);
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
