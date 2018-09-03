using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommonUtils;
using JetBrains.Annotations;

namespace SafeAuthenticator.Native {
  [PublicAPI]
  public enum MDataAction
  {
    // ReSharper disable once InconsistentNaming
    Insert,
    // ReSharper disable once InconsistentNaming
    Update,
    // ReSharper disable once InconsistentNaming
    Delete,
    // ReSharper disable once InconsistentNaming
    ManagePermissions,
  }

  [PublicAPI]
  public struct AccountInfo
  {
    public ulong MutationsDone;
    public ulong MutationsAvailable;
  }

  [PublicAPI]
  public struct MDataInfo
  {
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
    public List<ContainerPermissions> Containers;

    public AuthReq(AuthReqNative native)
    {
      App = native.App;
      AppContainer = native.AppContainer;
      Containers = BindingUtils.CopyToObjectList<ContainerPermissions>(native.ContainersPtr, (int)native.ContainersLen);
    }

    public AuthReqNative ToNative()
    {
      return new AuthReqNative()
      {
        App = App,
        AppContainer = AppContainer,
        ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
        ContainersLen = (UIntPtr)(Containers?.Count ?? 0),
        ContainersCap = UIntPtr.Zero
      };
    }
  }

  public struct AuthReqNative
  {
    internal AppExchangeInfo App;
    [MarshalAs(UnmanagedType.U1)]
    internal bool AppContainer;
    internal IntPtr ContainersPtr;
    internal UIntPtr ContainersLen;
    // ReSharper disable once NotAccessedField.Compiler
    internal UIntPtr ContainersCap;

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
      Containers = BindingUtils.CopyToObjectList<ContainerPermissions>(native.ContainersPtr, (int)native.ContainersLen);
    }

    public ContainersReqNative ToNative()
    {
      return new ContainersReqNative()
      {
        App = App,
        ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
        ContainersLen = (UIntPtr)(Containers?.Count ?? 0),
        ContainersCap = UIntPtr.Zero
      };
    }
  }

  public struct ContainersReqNative
  {
    internal AppExchangeInfo App;
    internal IntPtr ContainersPtr;
    internal UIntPtr ContainersLen;
    // ReSharper disable once NotAccessedField.Compiler
    internal UIntPtr ContainersCap;

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
        MDataLen = (UIntPtr)(MData?.Count ?? 0),
        MDataCap = UIntPtr.Zero
      };
    }
  }

  public struct ShareMDataReqNative
  {
    internal AppExchangeInfo App;
    internal IntPtr MDataPtr;
    internal UIntPtr MDataLen;
    // ReSharper disable once NotAccessedField.Compiler
    internal UIntPtr MDataCap;

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

    public AuthGranted(AuthGrantedNative native)
    {
      AppKeys = native.AppKeys;
      AccessContainerInfo = native.AccessContainerInfo;
      AccessContainerEntry = new AccessContainerEntry(native.AccessContainerEntry);
      BootstrapConfig = BindingUtils.CopyToByteList(native.BootstrapConfigPtr, (int)native.BootstrapConfigLen);
    }

    public AuthGrantedNative ToNative()
    {
      return new AuthGrantedNative()
      {
        AppKeys = AppKeys,
        AccessContainerInfo = AccessContainerInfo,
        AccessContainerEntry = AccessContainerEntry.ToNative(),
        BootstrapConfigPtr = BindingUtils.CopyFromByteList(BootstrapConfig),
        BootstrapConfigLen = (UIntPtr)(BootstrapConfig?.Count ?? 0),
        BootstrapConfigCap = UIntPtr.Zero
      };
    }
  }

  public struct AuthGrantedNative
  {
    internal AppKeys AppKeys;
    internal AccessContInfo AccessContainerInfo;
    internal AccessContainerEntryNative AccessContainerEntry;
    internal IntPtr BootstrapConfigPtr;
    internal UIntPtr BootstrapConfigLen;
    // ReSharper disable once NotAccessedField.Compiler
    internal UIntPtr BootstrapConfigCap;

    // ReSharper disable once UnusedMember.Global
    public void Free()
    {
      AccessContainerEntry.Free();
      BindingUtils.FreeList(ref BootstrapConfigPtr, ref BootstrapConfigLen);
    }
  }

  [PublicAPI]
  public struct AppKeys
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SignPublicKeyLen)]
    public byte[] OwnerKey;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SymKeyLen)]
    public byte[] EncKey;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SignPublicKeyLen)]
    public byte[] SignPk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.SignSecretKeyLen)]
    public byte[] SignSk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.AsymPublicKeyLen)]
    public byte[] EncPk;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AppConstants.AsymSecretKeyLen)]
    public byte[] EncSk;
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
        ContainersLen = (UIntPtr)(Containers?.Count ?? 0),
        ContainersCap = UIntPtr.Zero
      };
    }
  }

  public struct AccessContainerEntryNative
  {
    internal IntPtr ContainersPtr;
    internal UIntPtr ContainersLen;
    // ReSharper disable once NotAccessedField.Compiler
    internal UIntPtr ContainersCap;

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
    public List<byte> Val;

    public MDataKey(MDataKeyNative native)
    {
      Val = BindingUtils.CopyToByteList(native.ValPtr, (int)native.ValLen);
    }

    public MDataKeyNative ToNative()
    {
      return new MDataKeyNative()
      {
        ValPtr = BindingUtils.CopyFromByteList(Val),
        ValLen = (UIntPtr)(Val?.Count ?? 0)
      };
    }
  }

  public struct MDataKeyNative
  {
    internal IntPtr ValPtr;
    internal UIntPtr ValLen;

    // ReSharper disable once UnusedMember.Global
    public void Free()
    {
      BindingUtils.FreeList(ref ValPtr, ref ValLen);
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

    public File(FileNative native)
    {
      Size = native.Size;
      CreatedSec = native.CreatedSec;
      CreatedNsec = native.CreatedNsec;
      ModifiedSec = native.ModifiedSec;
      ModifiedNsec = native.ModifiedNsec;
      UserMetadata = BindingUtils.CopyToByteList(native.UserMetadataPtr, (int)native.UserMetadataLen);
      DataMapName = native.DataMapName;
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
        DataMapName = DataMapName
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
      Containers = BindingUtils.CopyToObjectList<ContainerPermissions>(native.ContainersPtr, (int)native.ContainersLen);
    }

    public RegisteredAppNative ToNative()
    {
      return new RegisteredAppNative()
      {
        AppInfo = AppInfo,
        ContainersPtr = BindingUtils.CopyFromObjectList(Containers),
        ContainersLen = (UIntPtr)(Containers?.Count ?? 0),
        ContainersCap = IntPtr.Zero
      };
    }
  }

  public struct RegisteredAppNative
  {
    internal AppExchangeInfo AppInfo;
    internal IntPtr ContainersPtr;
    internal UIntPtr ContainersLen;

    // ReSharper disable once NotAccessedField.Compiler
    internal IntPtr ContainersCap;

    // ReSharper disable once UnusedMember.Global
    public void Free()
    {
      BindingUtils.FreeList(ref ContainersPtr, ref ContainersLen);
    }
  }

}
