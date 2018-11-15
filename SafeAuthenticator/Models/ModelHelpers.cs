using JetBrains.Annotations;

namespace SafeAuthenticator.Models {
  [PublicAPI]
  public class PermissionSetModel {
    public bool Delete { get; set; }
    public bool Insert { get; set; }
    public bool ManagePermissions { get; set; }
    public bool Read { get; set; }
    public bool Update { get; set; }
  }

  public class ContainerPermissionsModel {
    private string _containerName;
    [PublicAPI]
    public string ContainerName {
      get => _containerName.StartsWith("apps/") ? "App Container" : _containerName;
      set => _containerName = value;
    }
    [PublicAPI]
    public PermissionSetModel Access { get; set; }
  }

    public class MDataModel
    {
        public ulong TypeTag { get; set; }
        public byte[] Name { get; set; }

        [PublicAPI]
        public PermissionSetModel Access { get; set; }
    }
}
