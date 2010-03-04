// ///
// ///
// /// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// /// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;
using Iaik.Utils;
using System.IO;
using Iaik.Tc.Tpm.Configuration;
using Iaik.Tc.Tpm.Configuration.DotNetConfiguration;
using System.Configuration;

namespace Iaik.Tc.Tpm
{


	public class Tests
	{
	
		public delegate void TestInvoker();
		
		public static void TestIt()
		{
			Tests t = new Tests();
			
			RunTest("TestByteStream", t.TestByteStream);
			RunTest("Test .net ACL", t.TestDotNetCfgAccessControlList);
		}
		
		private static void RunTest(string testName, TestInvoker methodToTest)
		{
			Console.Write("Running test '{0}'.......", testName);
			
			try
			{
				methodToTest();
				Console.WriteLine("PASSED");
			}
			catch(Exception ex)
			{
				Console.WriteLine("FAILED Reason: {0}", ex);
			}
		}
		
		public void TestByteStream()
		{
			byte[] data = new byte[1024];
			for(int i = 0; i<1024; i++)
				data[i] = (byte)(i%256);
			
			ByteStream bStream = new ByteStream(data);
			
			byte[] buffer = new byte[512];
			bStream.Read(buffer, 0, 512);
			for(int bufferIndex = 0; bufferIndex<buffer.Length; bufferIndex++)
			{
				if(buffer[bufferIndex] != bufferIndex % 256)
					throw new Exception("Expecting " + (bufferIndex).ToString());
			}
			
			bStream.Seek(0, SeekOrigin.Begin);
			if(bStream.ReadByte() != 0)
				throw new Exception("Expecting expected 0");
			
			if(bStream.ReadByte() != 1)
				throw new Exception("Expecting expected 1");
			
			bStream.Seek(510, SeekOrigin.Current);
			bStream.Read(buffer, 0, 512);
			for(int i = 0; i<buffer.Length; i++)
			{
				if(buffer[i] != (i+512)%256)
					throw new Exception("Expecting " + (i+512).ToString());
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public void TestDotNetCfgAccessControlList()
		{
			Console.WriteLine("Make sure that the test configuration file is in place!");
			AccessControlList acl = new DotNetCfgAccessControlList();	
			User user1 = acl.FindUserById("1");
			User user1_1 = acl.FindUserById("1_1");
			User user2 = acl.FindUserById("2");
			User user3 = acl.FindUserById("3");
			
			ExternalUser externalUser1 = new ExternalUser("ex_1", "ex_gid_1");
			ExternalUser externalUser2 = new ExternalUser("ex_2", "ex_gid_2");
			ExternalUser externalUser3 = new ExternalUser("ex_3", "ex_gid_3");
			
			if(acl.IsUserAllowed("test", "user_1_only", user1) != true)
				throw new Exception("pid=user_1_only user 1 does not have access");
			if(acl.IsUserAllowed("test", "user_1_only", user2) != false)
				throw new Exception("pid=user_1_only user 2 has also access");
			
			
			if(acl.IsUserAllowed("test", "group_2_and_user_1", user2) != true)
				throw new Exception("pid=group_2_and_user_1 user2 has no access");
			if(acl.IsUserAllowed("test", "group_2_and_user_1", user1) != true)
				throw new Exception("pid=group_2_and_user_1 user1 has no access");
			if(acl.IsUserAllowed("test", "group_2_and_user_1", user3) != false)
				throw new Exception("pid=group_2_and_user_1 user3 has access");
			
			if(acl.IsUserAllowed("test", "allow_group_1_deny_user_1", user1_1) != true)
				throw new Exception("pid=allow_group_1_deny_user_1 user1_1 has no access");
			if(acl.IsUserAllowed("test", "allow_group_1_deny_user_1", user1) != false)
				throw new Exception("pid=allow_group_1_deny_user_1 user1 has access");
			if(acl.IsUserAllowed("test", "allow_group_1_deny_user_1", user2) != false)
				throw new Exception("pid=allow_group_1_deny_user_1 user2 has access");
			
			if(acl.IsUserAllowed("test", "all_internal", user1) != true)
				throw new Exception("pid=all_internal user1 has no access");
			if(acl.IsUserAllowed("test", "all_internal", user2) != true)
				throw new Exception("pid=all_internal user2 has no access");
			if(acl.IsUserAllowed("test", "all_internal", user3) != true)
				throw new Exception("pid=all_internal user3 has no access");
			if(acl.IsUserAllowed("test", "all_internal", externalUser1) != false)
				throw new Exception("pid=all_internal externalUser1 has access");
			
			if(acl.IsUserAllowed("test", "all_external", externalUser1) != true)
				throw new Exception("pid=all_external externalUser1 has no access");
			if(acl.IsUserAllowed("test", "all_external", externalUser2) != true)
				throw new Exception("pid=all_external externalUser2 has no access");
			if(acl.IsUserAllowed("test", "all_external", externalUser3) != true)
				throw new Exception("pid=all_external externalUser3 has no access");
			if(acl.IsUserAllowed("test", "all_external", user1) != false)
				throw new Exception("pid=all_external user1 has access");
			
		}
	}
}
