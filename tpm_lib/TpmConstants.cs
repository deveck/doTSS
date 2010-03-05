// Author: Andreas Reiter <andreas.reiter@student.tugraz.at>
// Author: Georg Neubauer <georg.neubauer@student.tugraz.at>

using System;

namespace Iaik.Tc.Tpm.library
{
    /// <summary>
    /// Command tags for the TPM
    /// </summary>
    public static class TpmCmdTags
    {
        public const UInt16 TPM_TAG_RQU_COMMAND = 0x00c1;
        public const UInt16 TPM_TAG_RQU_AUTH1_COMMAND = 0x00c2;
        public const UInt16 TPM_TAG_RQU_AUTH2_COMMAND = 0x00c3;
        public const UInt16 TPM_TAG_RSP_COMMAND = 0x00c4;
        public const UInt16 TPM_TAG_RSP_AUTH1_COMMAND = 0x00c5;
        public const UInt16 TPM_TAG_RSP_AUTH2_COMMAND = 0x00c6;
    }

    /// <summary>
    /// Ordinals for the TPM
    /// </summary>
    public static class TpmOrdinals
    {
        public const UInt32 TPM_ORD_OIAP = 0x0000000A;
        public const UInt32 TPM_ORD_OSAP = 0x0000000B;
        public const UInt32 TPM_ORD_ChangeAuth = 0x0000000C;
        public const UInt32 TPM_ORD_TakeOwnership = 0x0000000D;
        public const UInt32 TPM_ORD_ChangeAuthAsymStart = 0x0000000E;
        public const UInt32 TPM_ORD_ChangeAuthAsymFinish = 0x0000000F;
        public const UInt32 TPM_ORD_ChangeAuthOwner = 0x00000010;
        public const UInt32 TPM_ORD_DSAP = 0x00000011;
        public const UInt32 TPM_ORD_CMK_CreateTicket = 0x00000012;
        public const UInt32 TPM_ORD_CMK_CreateKey = 0x00000013;
        public const UInt32 TPM_ORD_Extend = 0x00000014;
        public const UInt32 TPM_ORD_PcrRead = 0x00000015;
        public const UInt32 TPM_ORD_Quote = 0x00000016;
        public const UInt32 TPM_ORD_Seal = 0x00000017;
        public const UInt32 TPM_ORD_Unseal = 0x00000018;
        public const UInt32 TPM_ORD_DirWriteAuth = 0x00000019;
        public const UInt32 TPM_ORD_DirRead = 0x0000001A;
        public const UInt32 TPM_ORD_CMK_CreateBlob = 0x0000001B;
        public const UInt32 TPM_ORD_CMK_SetRestrictions = 0x0000001C;
        public const UInt32 TPM_ORD_CMK_ApproveMA = 0x0000001D;
        public const UInt32 TPM_ORD_UnBind = 0x0000001E;
        public const UInt32 TPM_ORD_CreateWrapKey = 0x0000001F;
        public const UInt32 TPM_ORD_LoadKey = 0x00000020;
        public const UInt32 TPM_ORD_GetPubKey = 0x00000021;
        public const UInt32 TPM_ORD_EvictKey = 0x00000022;
        public const UInt32 TPM_ORD_KeyControlOwner = 0x00000023;
        public const UInt32 TPM_ORD_CMK_ConvertMigration = 0x00000024;
        public const UInt32 TPM_ORD_MigrateKey = 0x00000025;
        public const UInt32 TPM_ORD_CreateMigrationBlob = 0x00000028;
        public const UInt32 TPM_ORD_DAA_Join = 0x00000029;
        public const UInt32 TPM_ORD_ConvertMigrationBlob = 0x0000002A;
        public const UInt32 TPM_ORD_AuthorizeMigrationKey = 0x0000002B;
        public const UInt32 TPM_ORD_CreateMaintenanceArchive = 0x0000002C;
        public const UInt32 TPM_ORD_LoadMaintenanceArchive = 0x0000002D;
        public const UInt32 TPM_ORD_KillMaintenanceFeature = 0x0000002E;
        public const UInt32 TPM_ORD_LoadManuMaintPub = 0x0000002F;
        public const UInt32 TPM_ORD_ReadManuMaintPub = 0x00000030;
        public const UInt32 TPM_ORD_DAA_Sign = 0x00000031;
        public const UInt32 TPM_ORD_CertifyKey = 0x00000032;
        public const UInt32 TPM_ORD_CertifyKey2 = 0x00000033;
        public const UInt32 TPM_ORD_Sign = 0x0000003C;
        public const UInt32 TPM_ORD_Sealx = 0x0000003D;
        public const UInt32 TPM_ORD_Quote2 = 0x0000003E;
        public const UInt32 TPM_ORD_SetCapability = 0x0000003F;
        public const UInt32 TPM_ORD_LoadKey2 = 0x00000041;
        public const UInt32 TPM_ORD_GetRandom = 0x00000046;
        public const UInt32 TPM_ORD_StirRandom = 0x00000047;
        public const UInt32 TPM_ORD_SelfTestFull = 0x00000050;
        public const UInt32 TPM_ORD_CertifySelfTest = 0x00000052;
        public const UInt32 TPM_ORD_ContinueSelfTest = 0x00000053;
        public const UInt32 TPM_ORD_GetTestResult = 0x00000054;
        public const UInt32 TPM_ORD_Reset = 0x0000005A;
        public const UInt32 TPM_ORD_OwnerClear = 0x0000005B;
        public const UInt32 TPM_ORD_DisableOwnerClear = 0x0000005C;
        public const UInt32 TPM_ORD_ForceClear = 0x0000005D;
        public const UInt32 TPM_ORD_DisableForceClear = 0x0000005E;
        public const UInt32 TPM_ORD_GetCapabilitySigned = 0x00000064;
        public const UInt32 TPM_ORD_GetCapability = 0x00000065;
        public const UInt32 TPM_ORD_GetCapabilityOwner = 0x00000066;
        public const UInt32 TPM_ORD_OwnerSetDisable = 0x0000006E;
        public const UInt32 TPM_ORD_PhysicalEnable = 0x0000006F;
        public const UInt32 TPM_ORD_PhysicalDisable = 0x00000070;
        public const UInt32 TPM_ORD_SetOwnerInstall = 0x00000071;
        public const UInt32 TPM_ORD_PhysicalSetDeactivated = 0x00000072;
        public const UInt32 TPM_ORD_SetTempDeactivated = 0x00000073;
        public const UInt32 TPM_ORD_SetOperatorAuth = 0x00000074;
        public const UInt32 TPM_ORD_SetOwnerPointer = 0x00000075;
        public const UInt32 TPM_ORD_CreateEndorsementKeyPair = 0x00000078;
        public const UInt32 TPM_ORD_MakeIdentity = 0x00000079;
        public const UInt32 TPM_ORD_ActivateIdentity = 0x0000007A;
        public const UInt32 TPM_ORD_ReadPubek = 0x0000007C;
        public const UInt32 TPM_ORD_OwnerReadPubek = 0x0000007D;
        public const UInt32 TPM_ORD_DisablePubekRead = 0x0000007E;
        public const UInt32 TPM_ORD_CreateRevocableEK = 0x0000007F;
        public const UInt32 TPM_ORD_RevokeTrust = 0x00000080;
        public const UInt32 TPM_ORD_OwnerReadInternalPub = 0x00000081;
        public const UInt32 TPM_ORD_GetAuditEvent = 0x00000082;
        public const UInt32 TPM_ORD_GetAuditEventSigned = 0x00000083;
        public const UInt32 TPM_ORD_GetAuditDigest = 0x00000085;
        public const UInt32 TPM_ORD_GetAuditDigestSigned = 0x00000086;
        public const UInt32 TPM_ORD_GetOrdinalAuditStatus = 0x0000008C;
        public const UInt32 TPM_ORD_SetOrdinalAuditStatus = 0x0000008D;
        public const UInt32 TPM_ORD_Terminate_Handle = 0x00000096;
        public const UInt32 TPM_ORD_Init = 0x00000097;
        public const UInt32 TPM_ORD_SaveState = 0x00000098;
        public const UInt32 TPM_ORD_Startup = 0x00000099;
        public const UInt32 TPM_ORD_SetRedirection = 0x0000009A;
        public const UInt32 TPM_ORD_SHA1Start = 0x000000A0;
        public const UInt32 TPM_ORD_SHA1Update = 0x000000A1;
        public const UInt32 TPM_ORD_SHA1Complete = 0x000000A2;
        public const UInt32 TPM_ORD_SHA1CompleteExtend = 0x000000A3;
        public const UInt32 TPM_ORD_FieldUpgrade = 0x000000AA;
        public const UInt32 TPM_ORD_SaveKeyContext = 0x000000B4;
        public const UInt32 TPM_ORD_LoadKeyContext = 0x000000B5;
        public const UInt32 TPM_ORD_SaveAuthContext = 0x000000B6;
        public const UInt32 TPM_ORD_LoadAuthContext = 0x000000B7;
        public const UInt32 TPM_ORD_SaveContext = 0x000000B8;
        public const UInt32 TPM_ORD_LoadContext = 0x000000B9;
        public const UInt32 TPM_ORD_FlushSpecific = 0x000000BA;
        public const UInt32 TPM_ORD_PCR_Reset = 0x000000C8;
        public const UInt32 TPM_ORD_NV_DefineSpace = 0x000000CC;
        public const UInt32 TPM_ORD_NV_WriteValue = 0x000000CD;
        public const UInt32 TPM_ORD_NV_WriteValueAuth = 0x000000CE;
        public const UInt32 TPM_ORD_NV_ReadValue = 0x000000CF;
        public const UInt32 TPM_ORD_NV_ReadValueAuth = 0x000000D0;
        public const UInt32 TPM_ORD_Delegate_UpdateVerification = 0x000000D1;
        public const UInt32 TPM_ORD_Delegate_Manage = 0x000000D2;
        public const UInt32 TPM_ORD_Delegate_CreateKeyDelegation = 0x000000D4;
        public const UInt32 TPM_ORD_Delegate_CreateOwnerDelegation = 0x000000D5;
        public const UInt32 TPM_ORD_Delegate_VerifyDelegation = 0x000000D6;
        public const UInt32 TPM_ORD_Delegate_LoadOwnerDelegation = 0x000000D8;
        public const UInt32 TPM_ORD_Delegate_ReadTable = 0x000000DB;
        public const UInt32 TPM_ORD_CreateCounter = 0x000000DC;
        public const UInt32 TPM_ORD_IncrementCounter = 0x000000DD;
        public const UInt32 TPM_ORD_ReadCounter = 0x000000DE;
        public const UInt32 TPM_ORD_ReleaseCounter = 0x000000DF;
        public const UInt32 TPM_ORD_ReleaseCounterOwner = 0x000000E0;
        public const UInt32 TPM_ORD_EstablishTransport = 0x000000E6;
        public const UInt32 TPM_ORD_ExecuteTransport = 0x000000E7;
        public const UInt32 TPM_ORD_ReleaseTransportSigned = 0x000000E8;
        public const UInt32 TPM_ORD_GetTicks = 0x000000F1;
        public const UInt32 TPM_ORD_TickStampBlob = 0x000000F2;
        public const UInt32 TSC_ORD_PhysicalPresence = 0x4000000A;
        public const UInt32 TSC_ORD_ResetEstablishmentBit = 0x4000000B;
    }
}