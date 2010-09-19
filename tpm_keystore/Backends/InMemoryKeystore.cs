// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using System.Collections.Generic;

namespace Iaik.Tc.TPM.Keystore.Backends
{

	/// <summary>
	/// Simple keystore for in memory key storage.
	/// All keys are lost on disposal.
	/// </summary>
	[TPMKeystore("InMemoryKeystore")]
	public class InMemoryKeystore : TPMKeystoreProvider
	{

		/// <summary>
		/// Contains mapping from identifier->key info
		/// </summary>
		private Dictionary<string, InMemoryKeyInfo> _identifierMapper = 
			new Dictionary<string, InMemoryKeyInfo>();
		
		/// <summary>
		/// Contains mapping from friendly name->keyInfo
		/// </summary>
		private Dictionary<string, InMemoryKeyInfo> _friendlyNameMapper =
			new Dictionary<string, InMemoryKeyInfo>();
			
		
		public InMemoryKeystore()
		{
		}
		
		public InMemoryKeystore(IDictionary<string, string> parameters)
		{
		}
		
		
		public override long KeyCount 
		{
			get { return _identifierMapper.Count; }
		}

		
		public override void AddKey (string friendlyName, string identifier, string parentFriendlyName, byte[] keyData)
		{
			if(_identifierMapper.ContainsKey(identifier))
				throw new ArgumentException("The key store already contains a key with the same identifier");
			
			if(_friendlyNameMapper.ContainsKey(friendlyName))
				throw new ArgumentException("The key store already contains a key with the same friendly name");
			
			InMemoryKeyInfo keyInfo = new InMemoryKeyInfo(friendlyName, identifier, parentFriendlyName, keyData);
			_identifierMapper.Add(identifier, keyInfo);
			_friendlyNameMapper.Add(friendlyName, keyInfo);
		}
		
		public override void RemoveKey (string friendlyName)
		{
			if(_friendlyNameMapper.ContainsKey(friendlyName))
			{
				InMemoryKeyInfo keyInfo = _friendlyNameMapper[friendlyName];
				_friendlyNameMapper.Remove(friendlyName);
				_identifierMapper.Remove(keyInfo.Identifier);
			}
		}

		public override KeyValuePair<string, string>? FindParentKeyByFriendlyName (string friendlyName)
		{
			if(_friendlyNameMapper.ContainsKey(friendlyName) == false)
				throw new ArgumentException(string.Format("Key with friendly name='{0}' is not contained in the keystore", friendlyName));
			
			InMemoryKeyInfo myKey = _friendlyNameMapper[friendlyName];
			
			if(myKey.ParentFriendlyName == null)
				return null;
			
			if(_friendlyNameMapper.ContainsKey(myKey.ParentFriendlyName))
				throw new ArgumentException(string.Format("Key store inconsistency detected, key with friendly name '{0}' not found", myKey.ParentFriendlyName));

			
			InMemoryKeyInfo parentKey = _friendlyNameMapper[myKey.ParentFriendlyName];
						
			return new KeyValuePair<string, string>(parentKey.FriendlyName, parentKey.Identifier);
		}
		
		public override KeyValuePair<string, string>? FindParentKeyByIdentifier (string identifier)
		{
			if(_identifierMapper.ContainsKey(identifier) == false)
				throw new ArgumentException(string.Format("Key with identifier='{0}' is not contained in the keystore", identifier));
			
			InMemoryKeyInfo myKey = _identifierMapper[identifier];
			
			if(myKey.ParentFriendlyName == null)
				return null;

			
			if(_friendlyNameMapper.ContainsKey(myKey.ParentFriendlyName) == false)
				throw new ArgumentException(string.Format("Key store inconsistency detected, key with friendly name '{0}' not found", myKey.ParentFriendlyName));

			InMemoryKeyInfo parentKey = _friendlyNameMapper[myKey.ParentFriendlyName];
			
			
			return new KeyValuePair<string, string>(parentKey.FriendlyName, parentKey.Identifier);
		}


		public override string FriendlyNameToIdentifier (string friendlyName)
		{
			if(_friendlyNameMapper.ContainsKey(friendlyName) == false)
				throw new ArgumentException("Key not found in key store");
			
			return _friendlyNameMapper[friendlyName].Identifier;
		}

		public override byte[] GetKeyBlob (string identifier)
		{
			if(_identifierMapper.ContainsKey(identifier) == false)
				throw new ArgumentException("Identifier not found in key store");
			
			return _identifierMapper[identifier].Data;
		}

		public override string IdentifierToFriendlyName (string identifier)
		{
			if(_identifierMapper.ContainsKey(identifier) == false)
				throw new ArgumentException("Identifier not found in key store");
			
			return _identifierMapper[identifier].FriendlyName;
		}


		public override bool ContainsIdentifier (string identifier)
		{
			return _identifierMapper.ContainsKey(identifier);
		}


		public override string[] EnumerateFriendlyNames ()
		{
			List<string> friendlyNames = new List<string>();
			
			foreach(string friendlyName in _friendlyNameMapper.Keys)
				friendlyNames.Add(friendlyName);
			
			return friendlyNames.ToArray();
		}

		public override string[] EnumerateIdentifiers ()
		{
			List<string> identifiers = new List<string>();
			
			foreach(string identifier in _identifierMapper.Keys)
				identifiers.Add(identifier);
			
			return identifiers.ToArray();
		}


		/// <summary>
		/// Contains internal informations of a key
		/// </summary>
		private class InMemoryKeyInfo
		{
			private string _friendlyName;
			
			public string FriendlyName
			{
				get{ return _friendlyName;}
			}
			
			private string _identifier;
			
			public string Identifier
			{
				get{ return _identifier;}
			}
			
			private string _parentFriendlyName;
			
			public string ParentFriendlyName
			{
				get{ return _parentFriendlyName;}
			}
			
			private byte[] _data;
			
			public byte[] Data
			{
				get{ return _data;}
			}
			
			public InMemoryKeyInfo(string friendlyName, string identifier, string parentFriendlyName, byte[] data)
			{
				_friendlyName = friendlyName;
				_identifier = identifier;
				_parentFriendlyName = parentFriendlyName;
				_data = data;
			}
		}
	}
}
