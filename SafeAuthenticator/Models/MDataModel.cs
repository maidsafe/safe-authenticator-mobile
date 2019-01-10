using JetBrains.Annotations;

namespace SafeAuthenticator.Models
{
    public class MDataModel
    {
        public ulong TypeTag { get; set; }

        public byte[] Name { get; set; }

        public string MetaName { get; set; }

        public string MetaDescription { get; set; }

        [PublicAPI]
        public PermissionSetModel Access { get; set; }
    }
}
