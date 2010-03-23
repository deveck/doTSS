commit c28330ec7fefde87da2b05f428b6924d0000bd8d
Merge: 1d3dcda e3e5aa0
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Tue Mar 16 15:32:48 2010 +0100

    Merge branch 'tmplib' into tpmlib

commit 1d3dcdab01cf2e5e3028561e3ff5518339750ef5
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Tue Mar 16 15:31:55 2010 +0100

    Added low level support for linux tpm devices (/dev/tpmX)

commit e3e5aa0dbecaff158689c054bc0e038409936ced
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Mar 15 00:38:22 2010 +0100

    Corrected an enum related TypedPrimitive error

commit fd7c5c592514bfef9bd4b995d2f99f7ff44fc851
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Mar 14 22:59:48 2010 +0100

    First implementation of capability request (without response)

commit 3c6225272f5e7a5b2cbed6b3449057441a212bd9
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Mar 14 21:37:50 2010 +0100

    Added capability data classes

commit 1d31a1053f66d327662acd73a6ae7a83c6b8733d
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sat Mar 13 17:03:56 2010 +0100

    Extended TPMCommandRequest and Parameters to be stream serializable

commit bd66411f315234896f651b0dca607dc3e797cc7c
Merge: c9d2fc6 cbfd43e
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Fri Mar 12 22:45:06 2010 +0100

    Merge branch 'master' into tpmlib
    
    Conflicts:
    	tpm_server.sln
    	tpm_server.userprefs

commit c9d2fc6b6aaa25bbc929267c4da891db14670cb5
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Fri Mar 12 22:41:31 2010 +0100

    Added user preferences to ignore

commit 4cbda5dba7138cd2b59f9d07d2f823e947129ead
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Tue Mar 9 19:31:49 2010 +0100

    terminated some of the compile errors, introduced a little more abstraction, as defined

commit cbfd43ee03c082cd6e7095898b9a18ef298306b1
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Tue Mar 9 09:27:48 2010 +0100

    Added authentication over unix sockets

commit 6cd398bc65e09ec52ed0c08001a7479f39e5f22d
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Mon Mar 8 21:13:01 2010 +0100

    updated namespaces

commit 6a09bf46b19c65c43a24bc067ee13ecb2d549742
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Mon Mar 8 21:08:10 2010 +0100

    Found the "lost" library tpm_lib_core, sry for that
    * build up new folder and project structure
    
    library structure is now
    
    tpm_lib [Folder]
    |
    |---> tpm_lib_common [library proj. for common; request, response, param abstraction]
    |---> tpm_lib_core [library proj.; implementation of tpm commands]
    |---> tpm_lib_lowlvl [library proj.; Low level interface to TPM device]

commit d831b5c7b104a9ddc8407771ec054bed22a33f56
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Mar 8 18:09:09 2010 +0100

    Added another User_ExternalUser/Group_ExternalGroup abstraction

commit 05f42dc988b4fffa3fa26334de08369edbcf3616
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Mar 7 20:53:59 2010 +0100

    Added authentication method selection
    Extended tpm client to use it as "testing console"

commit 89069b26a0a9521d1da211ed9a6fd459eda1e827
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Sun Mar 7 02:25:53 2010 +0100

    further work on request, parameters and commands

commit 3a2fe8f26284c3baed7e18f21bc9be660affd205
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Sun Mar 7 01:22:24 2010 +0100

    renamed tpm_lib_server to tpm_lib_core, because it could be used also outside the server.
    further work on parameters and command request.

commit a5b86de8dad66095a895489db8131955be554a81
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Sun Mar 7 01:04:32 2010 +0100

    splitted tpm_lib into tpm_lib_common and tpm_lib_server
    started implementing parameter object
    
    * fixed some compile errors

commit 179d6a4cf6dd712f8c92f688423f0efacc1cd847
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Mar 7 00:13:17 2010 +0100

    Removed .suo file

commit f2d8d8a937bdb64354e308013c3d753bbe4a858b
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Mar 7 00:10:37 2010 +0100

    Added AuthenticationClient
    
    The authentication client currently can only list the supported
    authentication methods, but once completed it will be responsible
    for client authentication.

commit 0039bd89fd11a9ae83d4ee31c09d9a8ea111feb3
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Fri Mar 5 20:37:43 2010 +0100

    added tpm_test project

commit 4231d88bac678a99a78acfcdb72d1c4d86acea85
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Fri Mar 5 18:43:05 2010 +0100

    little modification

commit 5d3c304f57111a51bb939d5d430ad6626fc10c1e
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Fri Mar 5 17:11:27 2010 +0100

    Modified tpmserver config file to use "the short way of adding listeners"

commit 4c4fae997bac7f0915a10ea3d2db0dc0cc53fc8d
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Fri Mar 5 17:10:54 2010 +0100

    Removed deleted files from tracking

commit 416cf00d8a9c3603fc77747dc67ba2368fb996b8
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Fri Mar 5 17:07:45 2010 +0100

    Added Connection and authentication method configuration section
    Seperated the service base from the main program

commit 5be00ba4aab9c5069174d4a3114f00468a1d0c9b
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Fri Mar 5 15:35:29 2010 +0100

    use GenericBlob as std type, for all kinds of blobs

commit bba158b05f06253444c2f3b71759b8f6156c38e5
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Fri Mar 5 15:17:54 2010 +0100

    sry, here are the files

commit 01d91e8efc23c6a07562234d4322820862ce10d7
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Fri Mar 5 15:16:36 2010 +0100

    Introduced Request/Response
    start commandnames constants
    start rebuild of TpmBlob

commit 6b6a8cbbd486d8ae62a632e87659370e339d0be3
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Fri Mar 5 14:40:12 2010 +0100

    - Set up project hierarchy
    - Start lowlevel frontend (thanks to Johannes)
    - Start implementing library (commands)
    
    *not compiling in the moment

commit 0a6aa332738472aba2bff576854b8312bee2b3cf
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Fri Mar 5 10:38:43 2010 +0100

    Added 'connections' configuration section.
    Added listeners sub section to be able to configure the listeners the
    server listens on after boot up

commit bfe917628c010030ed8bb56b110ff741ee08182c
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Thu Mar 4 16:07:30 2010 +0100

    Access control list is working now
    Added test cases for .net acl

commit 13b3b6171cc819cd9b14dffbeadb1f65607a4ebb
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Mar 1 09:03:49 2010 +0100

    Added basic Access Control list functionality (not completed)

commit 349f3e0f43751153b014debef9f33929ab2cd2ed
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Feb 28 16:58:33 2010 +0100

    Removed static UserElement properties, they are
    not needed. There is a Bug in the MONO Configuration implementation
    if the IsRequired flag is set for a ConfigurationProperty. If the
    flagged element is missing NO EXCEPTION is thrown, but it should
    (and is thrown in Ms .net)

commit ebe23e61a6dc6ac6392a19f987f3f44e1e8bdeb0
Merge: 00e070c 943e8dd
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Feb 28 16:45:34 2010 +0100

    Merge branch 'master' of rcpe-intern.tugraz.at:TpmCSStack-impl
    
    Conflicts:
    	tpm_server.sln

commit 00e070c9adf8c0127d5243a509f4d22abfece1b3
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Feb 28 16:43:40 2010 +0100

    Merge

commit 943e8ddb302f8916aba434c7ad94301e7a75b092
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sat Feb 27 21:12:01 2010 +0100

    Added .net configuration support for virtual-users, virtual-groups and permissions

commit 2c54ae40f2516976d7bb05fda3d8bad55bf8f722
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Tue Feb 23 13:44:24 2010 +0100

    All FrontEndConnections now have attached the FrontEndConnectionAttribute
    to uniquely identify the connections without creating an instance of them.
    
    FrontEndConnections with no attribute can not be instanciated

commit ec6e97a4fb0ce1dab895ada98815ec8464a0536a
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Thu Feb 11 13:05:44 2010 +0100

    Added connection library which seperates the core from the connection and
    packet mechanism

commit 074802d703b57f9d5317c21e6fd655a0c509b039
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Wed Feb 10 15:04:51 2010 +0100

    Removed accidentaly commited debug output file

commit 8a53598506b2ac1015bf6acdc88ff281e196532b
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Wed Feb 10 15:02:24 2010 +0100

    Now transmitting packet nr for packets that don't await response, because
    response packets have a valid packet nr, they need to be transmitted but don't
    await responses (response to response is not allowed)

commit 8b0d23a695c4047f0715f1959dd881b682814c20
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Wed Feb 10 14:25:12 2010 +0100

    Now working with response packets, but still some more work is acquired (testing)

commit a59be633f453ce6569704fc0fa02b20e67ca797a
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Feb 8 16:12:13 2010 +0100

    gitignore

commit a60fedc4b9f62f85cdbdb0f20dd36b2132c2cc56
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Feb 8 16:10:25 2010 +0100

    Did some NamedPipeConnection rework

commit c4c3e13995ed6699065152c1125547a566678a4c
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Feb 8 15:30:14 2010 +0100

    Ignored some monodevelop specific files

commit f3cf06928fb7c1d29390696d27c1d43129e60d99
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Feb 8 15:26:21 2010 +0100

    Added first response-packet capabilities

commit 39d2cec74976bc9a43674e48da188ed5035e0e5e
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Feb 8 11:53:06 2010 +0100

    Changed the Subsystem packet handling to not pass SubsystemRequests.
    Now the new type RequestContext is passed to the packet handlers,
    this request contexts have the ability to create the response
    packets for the requests
    
    Added TypedSubsystemRequest, to have a nice and user friendly way
    to specify the response packet type without the need to supply
    the actual (Reflective-)Type

commit 7acff42f814fbbf733a83d81abbb1dc90141841e
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Feb 7 20:54:44 2010 +0100

    Did some general corrections, packet transmitting is working now.
    The only missing packet related feature so far is the transmission
    of response packets. By hand packets can be transmitted in both
    directions.
    
    Moved the creation of the server context away from the listener to the
    tpm server, because the server itself needs to remember all available
    contexts (connected clients), the listener does even not need to know
    that there exists something like contexts ^^

commit e6dd99d073eefc13eb694bf732c0276576eb2763
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sat Feb 6 02:08:30 2010 +0100

    Corrected some PacketTransmitter failures, but there is still something wrong

commit eba0d57b4d7ff621eec0792d0da717538a32469a
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sat Feb 6 01:10:17 2010 +0100

    Added apache log4net framework
    
    The mentioned apache log4net logging framework has been
    added. The confguration currently is hardcoded, but it should
    be extended to read the configuration from an external file,
    at least to specify additional appenders (log targets) using
    the frameworks own configuration mechanism or using the
    application specific mechanism.
    
    FrontEndConnectionAttribute was added. This attribute should mark
    all FrontEndConnection implementations to be able to make the used
    Connection(s)/Listener(s) configurable without providing a factory
    for each Connection type. With this attribute it would even be
    possible to load the connection types on demand from external
    assemblies.

commit f3c4047e6220489b026fa9f7ef8f927cb54b0c49
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Wed Feb 3 10:45:06 2010 +0100

    Added UnixSocketConnection as replacement for named pipes on unix systems
    
    As of mono 2.6 Named pipes are supported by the framework, but marked with a
    todo, that they are not functional on unix systems.
    The UnixSocketConnection implementation uses native unix socket, the drawback is
    that now the project is not compileable on Microsoft .net framework because
    it simply does not contain these unix socket implementations.
    The simple solution would be to capsulate operating system specific stuff
    into a external library which is loaded at runtime. The base library then only
    contains platform independent code. Thats the way to go ;-)

commit 1f18bc4594f92e2d93b980b23aad19ceb3535795
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Wed Feb 3 01:41:56 2010 +0100

    Now client pipe really connects ;)

commit bcf36fb44e1240d709c6b1e3d633c8cd3082880c
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Tue Feb 2 21:32:41 2010 +0100

    Added Basic Subsystem support and packet handling in both directions
    
    The packet transmitter has been added, it currently supports asynchronous packet
    transfers in both directions (client->server, server->client). Because of the new requirement
    that the tpm blob is built on the server and signed on the client this is REALLY needed.
    
    The subsystem infrastructure has also been added.
    There is still a problem with the NamedPipeConnection in linux (i did not recognize that on windows)
    where the NamedPipeListener is just says that a client connected but then is in an unusable state...
    ...very strange
    
    Still missing is a sophisticated log framework. My research has resulted that
    apache log4net (http://logging.apache.org/log4net/) is a very nice solution

commit 7c620ff6e35218c824af36e8e9f454daacfd3d6c
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Wed Jan 27 02:43:21 2010 +0100

    Added NamedPipeConnection and started packet handling

commit 8e7318f4e1f6bf0424491d25cd7e3b96625b5362
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Mon Jan 25 13:50:57 2010 +0100

    Added connectivity stub classes, to be extended soon :-)

commit a1a78e459fc318f5bb4092302950f2f9c9e5dae5
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sun Jan 24 14:59:15 2010 +0100

    Added CommandLineHandler

commit e9468626e9e47a42369c095930f4b94ec1003bb0
Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
Date:   Sat Jan 23 17:41:40 2010 -0500

    Initial commit

commit 3df1fd9ee8b38c4d071c22c5cee75733c8228c22
Author: Georg Neubauer <georg.neubauer@student.tugraz.at>
Date:   Wed Jan 20 00:45:33 2010 +0100

    initialise and added .gitignore
