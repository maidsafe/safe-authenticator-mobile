using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafeAuthenticator.Native {
  public partial interface IAuthBindings {
    bool IsMockBuild();
    Task AuthReconnectAsync(IntPtr auth);
    Task<AccountInfo> AuthAccountInfoAsync(IntPtr auth);
    Task<string> AuthExeFileStemAsync();
    Task AuthSetAdditionalSearchPathAsync(string newPath);
    void AuthFree(IntPtr auth);
    Task AuthRmRevokedAppAsync(IntPtr auth, string appId);
    Task<List<AppExchangeInfo>> AuthRevokedAppsAsync(IntPtr auth);
    Task<List<RegisteredApp>> AuthRegisteredAppsAsync(IntPtr auth);
    Task<List<AppAccess>> AuthAppsAccessingMutableDataAsync(IntPtr auth, byte[] mdName, ulong mdTypeTag);
    Task<string> EncodeShareMDataRespAsync(IntPtr auth, ref ShareMDataReq req, uint reqId, bool isGranted);
    Task<string> AuthRevokeAppAsync(IntPtr auth, string appId);
    Task AuthFlushAppRevocationQueueAsync(IntPtr auth);
    Task<string> EncodeUnregisteredRespAsync(uint reqId, bool isGranted);
    Task<string> EncodeAuthRespAsync(IntPtr auth, ref AuthReq req, uint reqId, bool isGranted);
    Task<string> EncodeContainersRespAsync(IntPtr auth, ref ContainersReq req, uint reqId, bool isGranted);
    Task AuthInitLoggingAsync(string outputFileNameOverride);
    Task<string> AuthOutputLogPathAsync(string outputFileName);
  }
}
