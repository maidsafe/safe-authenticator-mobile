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

        public bool AppContainerRequest { get; set; }

        public string PageTitle { get; set; }

        public bool IsMDataRequest { get; }

        public ObservableRangeCollection<ContainerPermissionsModel> Containers { get; set; }

        public ObservableRangeCollection<MDataModel> MData { get; set; }

        readonly AuthIpcReq _authReq;
        readonly ShareMDataIpcReq _shareMdReq;
        readonly ContainersIpcReq _containerReq;

        public RequestDetailViewModel(IpcReq req)
        {
            var requestType = req.GetType();
            if (requestType == typeof(AuthIpcReq))
            {
                _authReq = req as AuthIpcReq;
                PageTitle = "Authentication Request";
                ProcessAuthRequestData();
            }
            else if (requestType == typeof(ContainersIpcReq))
            {
                _containerReq = req as ContainersIpcReq;
                PageTitle = "Container Request";
                ProcessContainerRequestData();
            }
            else if (requestType == typeof(ShareMDataIpcReq))
            {
                _shareMdReq = req as ShareMDataIpcReq;
                PageTitle = "Share MData Request";
                IsMDataRequest = true;
                ProcessMDataRequestData();
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
                    ContainerName = x.ContName
                }).ToObservableRangeCollection();
            AppContainerRequest = _authReq.AuthReq.AppContainer;
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
                    ContainerName = x.ContName
                }).ToObservableRangeCollection();
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
        }
    }
}
