using JetBrains.Annotations;

namespace SafeAuthenticator.Models
{
    public class ContainerPermissionsModel
    {
        private string _containerName;

        [PublicAPI]
        public string ContainerName
        {
            get => _containerName.StartsWith("apps/") ? "App Container" : _containerName;
            set => _containerName = value;
        }

        [PublicAPI]
        public PermissionSetModel Access { get; set; }
    }
}
