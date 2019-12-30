using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;

namespace SafeAuthenticator.Native
{
    [PublicAPI]
    public struct MDataInfo
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool Seq;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.XorNameLen)]
        public byte[] Name;

        public ulong TypeTag;

        [MarshalAs(UnmanagedType.U1)]
        public bool HasEncInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymKeyLen)]
        public byte[] EncKey;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymNonceLen)]
        public byte[] EncNonce;

        [MarshalAs(UnmanagedType.U1)]
        public bool HasNewEncInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymKeyLen)]
        public byte[] NewEncKey;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymNonceLen)]
        public byte[] NewEncNonce;
    }

    [PublicAPI]
    public struct PermissionSet
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool Read;
        [MarshalAs(UnmanagedType.U1)]
        public bool Insert;
        [MarshalAs(UnmanagedType.U1)]
        public bool Update;
        [MarshalAs(UnmanagedType.U1)]
        public bool Delete;
        [MarshalAs(UnmanagedType.U1)]
        public bool ManagePermissions;
    }

    [PublicAPI]
    public struct AuthReq
    {
        public AppExchangeInfo App;
        public bool AppContainer;
        public bool AppPermissionTransferCoins;
        public bool AppPermissionPerformMutations;
        public bool AppPermissionGetBalance;
        public List<ContainerPermissions> Containers;

        public AuthReq(AuthReqNative native)
        {
            App = native.App;
            AppContainer = native.AppContainer;
            AppPermissionTransferCoins = native.AppPermissionTransferCoins;
            AppPermissionPerformMutations = native.AppPermissionPerformMutations;
            AppPermissionGetBalance = native.AppPermissionGetBalance;
            Containers =
                BindingUtils.CopyToObjectList<ContainerPermissions>(native.ContainersPtr, (int)native.ContainersLen);
        }

        public AuthReqNative ToNative()
        {
            return new AuthReqNative()
            {
                App = App,
                AppContainer = AppContainer,
                AppPermissionTransferCoins = AppPermissionTransferCoins,
                AppPermissionPerformMutations = AppPermissionPerformMutations,
                AppPermissionGetBalance = AppPermissionGetBalance,
                ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
                ContainersLen = (UIntPtr)(Containers?.Count ?? 0)
            };
        }
    }

    public struct AuthReqNative
    {
        internal AppExchangeInfo App;
        [MarshalAs(UnmanagedType.U1)]
        internal bool AppContainer;
        [MarshalAs(UnmanagedType.U1)]
        public bool AppPermissionTransferCoins;
        [MarshalAs(UnmanagedType.U1)]
        public bool AppPermissionPerformMutations;
        [MarshalAs(UnmanagedType.U1)]
        public bool AppPermissionGetBalance;
        internal IntPtr ContainersPtr;
        internal UIntPtr ContainersLen;

        public void Free()
        {
            BindingUtils.FreeList(ref ContainersPtr, ref ContainersLen);
        }
    }

    [PublicAPI]
    public struct ContainersReq
    {
        internal AppExchangeInfo App;
        internal List<ContainerPermissions> Containers;

        public ContainersReq(ContainersReqNative native)
        {
            App = native.App;
            Containers =
                BindingUtils.CopyToObjectList<ContainerPermissions>(native.ContainersPtr, (int)native.ContainersLen);
        }

        public ContainersReqNative ToNative()
        {
            return new ContainersReqNative()
            {
                App = App,
                ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
                ContainersLen = (UIntPtr)(Containers?.Count ?? 0)
            };
        }
    }

    public struct ContainersReqNative
    {
        internal AppExchangeInfo App;
        internal IntPtr ContainersPtr;
        internal UIntPtr ContainersLen;

        public void Free()
        {
            BindingUtils.FreeList(ref ContainersPtr, ref ContainersLen);
        }
    }

    [PublicAPI]
    public struct AppExchangeInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Id;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Scope;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Vendor;
    }

    [PublicAPI]
    public struct ContainerPermissions
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string ContName;
        public PermissionSet Access;
    }

    [PublicAPI]
    public struct ShareMDataReq
    {
        public AppExchangeInfo App;
        public List<ShareMData> MData;

        public ShareMDataReq(ShareMDataReqNative native)
        {
            App = native.App;
            MData = BindingUtils.CopyToObjectList<ShareMData>(native.MDataPtr, (int)native.MDataLen);
        }

        public ShareMDataReqNative ToNative()
        {
            return new ShareMDataReqNative()
            {
                App = App,
                MDataPtr = BindingUtils.CopyFromObjectList(MData),
                MDataLen = (UIntPtr)(MData?.Count ?? 0)
            };
        }
    }

    public struct ShareMDataReqNative
    {
        internal AppExchangeInfo App;
        internal IntPtr MDataPtr;
        internal UIntPtr MDataLen;

        public void Free()
        {
            BindingUtils.FreeList(ref MDataPtr, ref MDataLen);
        }
    }

    [PublicAPI]
    public struct ShareMData
    {
        public ulong TypeTag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.XorNameLen)]
        public byte[] Name;

        public PermissionSet Perms;
    }

    [PublicAPI]
    public struct AuthGranted
    {
        public AppKeys AppKeys;
        public AccessContInfo AccessContainerInfo;
        public AccessContainerEntry AccessContainerEntry;
        public List<byte> BootstrapConfig;

        internal AuthGranted(AuthGrantedNative native)
        {
            AppKeys = new AppKeys(native.AppKeys);
            AccessContainerInfo = native.AccessContainerInfo;
            AccessContainerEntry = new AccessContainerEntry(native.AccessContainerEntry);
            BootstrapConfig = BindingUtils.CopyToByteList(native.BootstrapConfigPtr, (int)native.BootstrapConfigLen);
        }

        internal AuthGrantedNative ToNative()
        {
            return new AuthGrantedNative()
            {
                AppKeys = AppKeys.ToNative(),
                AccessContainerInfo = AccessContainerInfo,
                AccessContainerEntry = AccessContainerEntry.ToNative(),
                BootstrapConfigPtr = BindingUtils.CopyFromByteList(BootstrapConfig),
                BootstrapConfigLen = (UIntPtr)(BootstrapConfig?.Count ?? 0)
            };
        }
    }

    internal struct AuthGrantedNative
    {
        public AppKeysNative AppKeys;
        public AccessContInfo AccessContainerInfo;
        public AccessContainerEntryNative AccessContainerEntry;
        public IntPtr BootstrapConfigPtr;
        public UIntPtr BootstrapConfigLen;

        // ReSharper disable once UnusedMember.Global
        public void Free()
        {
            AppKeys.Free();
            AccessContainerEntry.Free();
            BindingUtils.FreeList(ref BootstrapConfigPtr, ref BootstrapConfigLen);
        }
    }

    [PublicAPI]
    public struct AppKeys
    {
        public List<byte> FullId;
        public byte[] EncKey;
        public byte[] EncPublicKey;
        public List<byte> EncSecretKey;

        internal AppKeys(AppKeysNative native)
        {
            FullId = BindingUtils.CopyToByteList(native.FullIdPtr, (int)native.FullIdLen);
            EncKey = native.EncKey;
            EncPublicKey = native.EncPublicKey;
            EncSecretKey = BindingUtils.CopyToByteList(native.EncSecretKeyPtr, (int)native.EncSecretKeyLen);
        }

        internal AppKeysNative ToNative()
        {
            return new AppKeysNative
            {
                FullIdPtr = BindingUtils.CopyFromByteList(FullId),
                FullIdLen = (UIntPtr)(FullId?.Count ?? 0),
                EncKey = EncKey,
                EncPublicKey = EncPublicKey,
                EncSecretKeyPtr = BindingUtils.CopyFromByteList(EncSecretKey),
                EncSecretKeyLen = (UIntPtr)(EncSecretKey?.Count ?? 0)
            };
        }
    }

    internal struct AppKeysNative
    {
        public IntPtr FullIdPtr;
        public UIntPtr FullIdLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymEncKeyLen)]
        public byte[] EncKey;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.BlsPublicKeyLen)]
        public byte[] EncPublicKey;
        public IntPtr EncSecretKeyPtr;
        public UIntPtr EncSecretKeyLen;

        internal void Free()
        {
            BindingUtils.FreeList(ref FullIdPtr, ref FullIdLen);
            BindingUtils.FreeList(ref EncSecretKeyPtr, ref EncSecretKeyLen);
        }
    }

    [PublicAPI]
    public struct AccessContInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.XorNameLen)]
        public byte[] Id;

        public ulong Tag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymNonceLen)]
        public byte[] Nonce;
    }

    [PublicAPI]
    public struct AccessContainerEntry
    {
        public List<ContainerInfo> Containers;

        public AccessContainerEntry(AccessContainerEntryNative native)
        {
            Containers = BindingUtils.CopyToObjectList<ContainerInfo>(native.ContainersPtr, (int)native.ContainersLen);
        }

        public AccessContainerEntryNative ToNative()
        {
            return new AccessContainerEntryNative()
            {
                ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
                ContainersLen = (UIntPtr)(Containers?.Count ?? 0)
            };
        }
    }

    public struct AccessContainerEntryNative
    {
        internal IntPtr ContainersPtr;
        internal UIntPtr ContainersLen;

        internal void Free()
        {
            BindingUtils.FreeList(ref ContainersPtr, ref ContainersLen);
        }
    }

    [PublicAPI]
    public struct ContainerInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
        public MDataInfo MDataInfo;
        public PermissionSet Permissions;
    }

    [PublicAPI]
    public struct AppAccess
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SignPublicKeyLen)]
        public byte[] SignKey;

        public PermissionSet Permissions;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string AppId;
    }

    [PublicAPI]
    public struct MetadataResponse
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Description;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.XorNameLen)]
        public byte[] XorName;

        public ulong TypeTag;
    }

    [PublicAPI]
    public struct MDataValue
    {
        public List<byte> Content;
        public ulong EntryVersion;

        public MDataValue(MDataValueNative native)
        {
            Content = BindingUtils.CopyToByteList(native.ContentPtr, (int)native.ContentLen);
            EntryVersion = native.EntryVersion;
        }

        public MDataValueNative ToNative()
        {
            return new MDataValueNative()
            {
                ContentPtr = BindingUtils.CopyFromByteList(Content),
                ContentLen = (UIntPtr)(Content?.Count ?? 0),
                EntryVersion = EntryVersion
            };
        }
    }

    public struct MDataValueNative
    {
        internal IntPtr ContentPtr;
        internal UIntPtr ContentLen;
        internal ulong EntryVersion;

        // ReSharper disable once UnusedMember.Global
        internal void Free()
        {
            BindingUtils.FreeList(ref ContentPtr, ref ContentLen);
        }
    }

    [PublicAPI]
    public struct MDataKey
    {
        public List<byte> Key;

        public MDataKey(MDataKeyNative native)
        {
            Key = BindingUtils.CopyToByteList(native.KeyPtr, (int)native.KeyLen);
        }

        public MDataKeyNative ToNative()
        {
            return new MDataKeyNative()
            {
                KeyPtr = BindingUtils.CopyFromByteList(Key),
                KeyLen = (UIntPtr)(Key?.Count ?? 0)
            };
        }
    }

    public struct MDataKeyNative
    {
        internal IntPtr KeyPtr;
        internal UIntPtr KeyLen;

        // ReSharper disable once UnusedMember.Global
        public void Free()
        {
            BindingUtils.FreeList(ref KeyPtr, ref KeyLen);
        }
    }

    [PublicAPI]
    public struct File
    {
        public ulong Size;
        public long CreatedSec;
        public uint CreatedNsec;
        public long ModifiedSec;
        public uint ModifiedNsec;
        public List<byte> UserMetadata;
        public byte[] DataMapName;
        public bool Published;

        public File(FileNative native)
        {
            Size = native.Size;
            CreatedSec = native.CreatedSec;
            CreatedNsec = native.CreatedNsec;
            ModifiedSec = native.ModifiedSec;
            ModifiedNsec = native.ModifiedNsec;
            UserMetadata = BindingUtils.CopyToByteList(native.UserMetadataPtr, (int)native.UserMetadataLen);
            DataMapName = native.DataMapName;
            Published = native.Published;
        }

        public FileNative ToNative()
        {
            return new FileNative()
            {
                Size = Size,
                CreatedSec = CreatedSec,
                CreatedNsec = CreatedNsec,
                ModifiedSec = ModifiedSec,
                ModifiedNsec = ModifiedNsec,
                UserMetadataPtr = BindingUtils.CopyFromByteList(UserMetadata),
                UserMetadataLen = (UIntPtr)(UserMetadata?.Count ?? 0),
                UserMetadataCap = UIntPtr.Zero,
                DataMapName = DataMapName,
                Published = Published
            };
        }
    }

    public struct FileNative
    {
        internal ulong Size;
        internal long CreatedSec;
        internal uint CreatedNsec;
        internal long ModifiedSec;
        internal uint ModifiedNsec;
        internal IntPtr UserMetadataPtr;
        internal UIntPtr UserMetadataLen;

        // ReSharper disable once NotAccessedField.Compiler
        internal UIntPtr UserMetadataCap;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.XorNameLen)]
        internal byte[] DataMapName;

        [MarshalAs(UnmanagedType.U1)]
        public bool Published;

        // ReSharper disable once UnusedMember.Global
        public void Free()
        {
            BindingUtils.FreeList(ref UserMetadataPtr, ref UserMetadataLen);
        }
    }

    [PublicAPI]
    public struct UserPermissionSet
    {
        public ulong UserH;
        public PermissionSet PermSet;
    }

    [PublicAPI]
    public struct RegisteredApp
    {
        public AppExchangeInfo AppInfo;
        public List<ContainerPermissions> Containers;

        public RegisteredApp(RegisteredAppNative native)
        {
            AppInfo = native.AppInfo;
            Containers =
                BindingUtils.CopyToObjectList<ContainerPermissions>(native.ContainersPtr, (int)native.ContainersLen);
        }

        public RegisteredAppNative ToNative()
        {
            return new RegisteredAppNative()
            {
                AppInfo = AppInfo,
                ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
                ContainersLen = (UIntPtr)(Containers?.Count ?? 0)
            };
        }
    }

    public struct RegisteredAppNative
    {
        internal AppExchangeInfo AppInfo;
        internal IntPtr ContainersPtr;
        internal UIntPtr ContainersLen;

        // ReSharper disable once UnusedMember.Global
        public void Free()
        {
            BindingUtils.FreeList(ref ContainersPtr, ref ContainersLen);
        }
    }
}
