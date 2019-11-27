using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels
{
    internal class RequestDetailViewModel : BaseViewModel
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

        int minPopupHeight = 210;
        int maxPopupHeight = 260;

        private bool _showTestCoinPermissions;

        public bool ShowTestCoinPermissions
        {
            get => _showTestCoinPermissions;
            set => SetProperty(ref _showTestCoinPermissions, value);
        }

        private string _testCoinPermissions;

        public string TestCoinPermissions
        {
            get => _testCoinPermissions;
            set => SetProperty(ref _testCoinPermissions, value);
        }

        private string _popupState;

        public string PopupState
        {
            get => _popupState;
            set => SetProperty(ref _popupState, value);
        }

        private int _popupLayoutHeight;

        public int PopupLayoutHeight
        {
            get => _popupLayoutHeight;
            set => SetProperty(ref _popupLayoutHeight, value);
        }

        private bool _isAppDetailsVisible;

        public bool IsAppDetailsVisible
        {
            get => _isAppDetailsVisible;
            set => SetProperty(ref _isAppDetailsVisible, value);
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private readonly string encodedRequest;
        private readonly IpcReq decodedRequest;

        private readonly AuthIpcReq _authReq;
        private readonly ShareMDataIpcReq _shareMdReq;
        private readonly ContainersIpcReq _containerReq;

        public ICommand SendResponseCommand { get; }

        public ICommand InfoButtonCommand { get; }

        public RequestDetailViewModel(string encodedUri, IpcReq req)
        {
            Containers = new ObservableRangeCollection<ContainerPermissionsModel>();
            MData = new ObservableRangeCollection<MDataModel>();
            var requestType = req.GetType();
            encodedRequest = encodedUri;
            decodedRequest = req;

            PopupState = Constants.None;
            IsAppDetailsVisible = false;

            SendResponseCommand = new Command<string>(OnSendResponse);
            InfoButtonCommand = new Command(OnInfoButtonCommand);

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
            PopupLayoutHeight = (Containers.Count == 0 && MData.Count == 0) ? minPopupHeight : maxPopupHeight;
        }

        private void OnInfoButtonCommand()
        {
            IsAppDetailsVisible = !IsAppDetailsVisible;
            if (IsAppDetailsVisible)
                PopupLayoutHeight += 105;
            else
                PopupLayoutHeight -= 105;
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
                    ContainerName = Utilities.FormatContainerName(x.ContName, AppInfo.Id)
                }).ToObservableRangeCollection();

            if (_authReq.AuthReq.AppContainer)
            {
                Containers.Add(new ContainerPermissionsModel()
                {
                    ContainerName = Constants.AppOwnFormattedContainer,
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

            if (_authReq.AuthReq.AppPermissionGetBalance)
                TestCoinPermissions += "Check balance ,";

            if (_authReq.AuthReq.AppPermissionTransferCoins)
                TestCoinPermissions += "Transfer coins";

            if (!string.IsNullOrEmpty(TestCoinPermissions))
                ShowTestCoinPermissions = true;
            else
                return;

            if (TestCoinPermissions.EndsWith(","))
                TestCoinPermissions = TestCoinPermissions.TrimEnd(',');
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
                    ContainerName = Utilities.FormatContainerName(x.ContName, AppInfo.Id)
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

        private async void OnSendResponse(string sender)
        {
            if (sender == "OK")
            {
                await PopupNavigation.Instance.PopAsync();
                return;
            }
            try
            {
                var response = sender == "ALLOW" ? true : false;
                if (IsAppDetailsVisible)
                    IsAppDetailsVisible = !IsAppDetailsVisible;

                PopupLayoutHeight = minPopupHeight;
                PopupState = Constants.Loading;

                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    throw new Exception(Constants.NoInternetMessage);
                }
                var encodedRsp = await Authenticator.GetEncodedResponseAsync(decodedRequest, response);

                if (response)
                    MessagingCenter.Send(this, MessengerConstants.RefreshHomePage, decodedRequest);

                var appId = AppId ?? UrlFormat.GetAppId(encodedRequest);
                var formattedRsp = UrlFormat.Format(appId, encodedRsp, false);
                Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");

                var appLaunched = await DependencyService.Get<IAppHandler>().LaunchApp(formattedRsp);
                if (!appLaunched)
                {
                    throw new Exception($"An app with the ID {appId} was not found");
                }
                await PopupNavigation.Instance.PopAsync();
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                PopupLayoutHeight = minPopupHeight;
                ErrorMessage = errorMessage;
                PopupState = Constants.Error;
            }
            catch (Exception ex)
            {
                PopupLayoutHeight = minPopupHeight;
                ErrorMessage = ex.Message;
                PopupState = Constants.Error;
            }
        }
    }
}
