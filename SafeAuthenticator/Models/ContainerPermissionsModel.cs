using JetBrains.Annotations;
using SafeAuthenticator.Helpers;

namespace SafeAuthenticator.Models
{
    public class ContainerPermissionsModel
    {
        public string ContainerImage => Utilities.FormatContainerNameToImage(ContainerName);

        [PublicAPI]
        public string ContainerName { get; set; }

        [PublicAPI]
        public PermissionSetModel Access { get; set; }
    }
}
