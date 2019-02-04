using System.Linq;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;

namespace SafeAuthenticator.ViewModels
{
    public class RequestDetailViewModel : ObservableObject
    {
        public AppExchangeInfo AppInfo { get; set; }

        public string AppName => AppInfo.Name;

        public string AppVendor => AppInfo.Vendor;

        public string AppId => AppInfo.Id;

        public string PageTitle { get; set; }

        public bool IsContainerRequest { get; }

        public bool IsMDataRequest { get; }

        public bool IsUnregisteredRequest { get; }

        public string SecondaryTitle { get; set; }

        public ObservableRangeCollection<ContainerPermissionsModel> Containers { get; set; }

        public ObservableRangeCollection<MDataModel> MData { get; set; }

        private readonly AuthIpcReq _authReq;
        private readonly ShareMDataIpcReq _shareMdReq;
        private readonly ContainersIpcReq _containerReq;

        public RequestDetailViewModel(IpcReq req)
        {
            Containers = new ObservableRangeCollection<ContainerPermissionsModel>();
            MData = new ObservableRangeCollection<MDataModel>();
            var requestType = req.GetType();

            if (requestType == typeof(UnregisteredIpcReq))
            {
                IsUnregisteredRequest = true;
            }
            else if (requestType == typeof(AuthIpcReq))
            {
                _authReq = req as AuthIpcReq;
                ProcessAuthRequestData();
                SecondaryTitle = Containers.Count > 0 ? "\nis requesting access to" : "\nis requesting access";
            }
            else if (requestType == typeof(ContainersIpcReq))
            {
                IsContainerRequest = true;
                _containerReq = req as ContainersIpcReq;
                ProcessContainerRequestData();
                SecondaryTitle = Containers.Count > 0 ? "\nis requesting access to" : "\nis requesting access";
            }
            else if (requestType == typeof(ShareMDataIpcReq))
            {
                _shareMdReq = req as ShareMDataIpcReq;
                IsMDataRequest = true;
                ProcessMDataRequestData();
                SecondaryTitle = MData.Count > 0 ? "\nis requesting access to" : "\nis requesting access";
            }
        }

        private void ProcessAuthRequestData()
        {
            AppInfo = _authReq.AuthReq.App;
            Containers = _authReq.AuthReq.Containers.Select(
                x => new ContainerPermissionsModel
                {
                    Access = new PermissionSetModel
                    {
                        Read = x.Access.Read,
                        Insert = x.Access.Insert,
                        Update = x.Access.Update,
                        Delete = x.Access.Delete,
                        ManagePermissions = x.Access.ManagePermissions
                    },
                    ContainerName = Utilities.FormatContainerName(x.ContName)
                }).ToObservableRangeCollection();

            if (_authReq.AuthReq.AppContainer)
            {
                Containers.Add(new ContainerPermissionsModel()
                {
                    ContainerName = "App's own Container",
                    Access = new PermissionSetModel
                    {
                        Read = true,
                        Insert = true,
                        Update = true,
                        Delete = true,
                        ManagePermissions = true
                    }
                });
            }

            Containers = Containers.OrderBy(c => c.ContainerName).ToObservableRangeCollection();
        }

        private void ProcessContainerRequestData()
        {
            AppInfo = _containerReq.ContainersReq.App;
            Containers = _containerReq.ContainersReq.Containers.Select(
                x => new ContainerPermissionsModel
                {
                    Access = new PermissionSetModel
                    {
                        Read = x.Access.Read,
                        Insert = x.Access.Insert,
                        Update = x.Access.Update,
                        Delete = x.Access.Delete,
                        ManagePermissions = x.Access.ManagePermissions
                    },
                    ContainerName = Utilities.FormatContainerName(x.ContName)
                }).ToObservableRangeCollection();

            Containers = Containers.OrderBy(c => c.ContainerName).ToObservableRangeCollection();
        }

        private void ProcessMDataRequestData()
        {
            AppInfo = _shareMdReq.ShareMDataReq.App;
            MData = _shareMdReq.ShareMDataReq.MData.Select(
              x => new MDataModel
              {
                  Access = new PermissionSetModel
                  {
                      Read = x.Perms.Read,
                      Insert = x.Perms.Insert,
                      Update = x.Perms.Update,
                      Delete = x.Perms.Delete,
                      ManagePermissions = x.Perms.ManagePermissions
                  },
                  Name = x.Name,
                  TypeTag = x.TypeTag
              }).ToObservableRangeCollection();

            for (var i = 0; i < MData.Count; i++)
            {
                MData[i].MetaName = _shareMdReq.MetadataResponse[i].Name;
                MData[i].MetaDescription = _shareMdReq.MetadataResponse[i].Description;
            }
        }
    }
}
