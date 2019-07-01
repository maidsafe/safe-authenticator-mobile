using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hexasoft.Zxcvbn;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using Xamarin.Essentials;

namespace SafeAuthenticator.Helpers
{
    internal static class Utilities
    {
        private static ZxcvbnEstimator _estimator;
        private static Dictionary<string, string> containerNameList;

        internal static ObservableRangeCollection<T> ToObservableRangeCollection<T>(this IEnumerable<T> source)
        {
            var result = new ObservableRangeCollection<T>();
            foreach (var item in source)
            {
                result.Add(item);
            }

            return result;
        }

        internal static StrengthIndicator StrengthChecker(string data)
        {
            if (_estimator == null)
            {
                _estimator = new ZxcvbnEstimator();
            }

            var strengthIndicator = new StrengthIndicator();

            if (string.IsNullOrEmpty(data))
            {
                throw new Exception("Can't check strength for empty string.");
            }

            var result = _estimator.EstimateStrength(data);
            strengthIndicator.Guesses = Math.Log(result.Guesses) / Math.Log(10);
            if (strengthIndicator.Guesses < Constants.StrengthScoreVeryWeak)
            {
                strengthIndicator.Strength = Constants.StrengthVeryWeak;
            }
            else if (strengthIndicator.Guesses < Constants.StrengthScoreWeak)
            {
                strengthIndicator.Strength = Constants.StrengthWeak;
            }
            else if (strengthIndicator.Guesses < Constants.StrengthScoreSomeWhatSecure)
            {
                strengthIndicator.Strength = Constants.StrengthSomewhatSecure;
            }
            else if (strengthIndicator.Guesses >= Constants.StrengthScoreSomeWhatSecure)
            {
                strengthIndicator.Strength = Constants.StrengthSecure;
            }

            strengthIndicator.Percentage = Math.Round(Math.Min((strengthIndicator.Guesses / 16) * 100, 100));
            return strengthIndicator;
        }

        internal static string GetErrorMessage(FfiException error)
        {
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                return Constants.NoInternetMessage;
            }

            switch (error.ErrorCode)
            {
                case Constants.UnexpectedError:
                    return Constants.CouldNotConnect;
                case Constants.RoutingInterfaceError:
                    return Constants.UpdateIp;
                case Constants.NoSuchAccountError:
                    return Constants.AccountNotPresent;
                case Constants.SymmetricDecipherFailureError:
                    return Constants.IncorrectPassword;
                case Constants.AccountExistsError:
                    return Constants.AccountAlreadyExists;
                case Constants.InvalidInvitationError:
                    return Constants.InvalidInvitationToken;
                case Constants.InvitationAlreadyClaimedError:
                    return Constants.InvitationAlreadyClaimed;
                case Constants.SharedMDataDeniedError:
                    return Constants.SharedMDataRequestDenied;
                case Constants.LowBalanceError:
                    return Constants.InsufficientAccountBalance;
                case Constants.NoSuchContainerError:
                    var firstIndex = error.Message.IndexOf("\'") + 1;
                    var lastIndex = error.Message.LastIndexOf("'");
                    var containerNameLength = lastIndex - firstIndex;
                    return string.Format(Constants.InvalidContainer, error.Message.Substring(firstIndex, containerNameLength)
                        .Replace(Constants.AppContainer, string.Empty));
                default:
                    return error.Message;
            }
        }

        internal static string GetRandomColor(int appNameLength)
        {
            var colors = new List<string>
            {
                "#EF5350",
                "#7E57C2",
                "#29B6F6",
                "#66BB6A",
                "#FF7043",
                "#42A5F5",
                "#EC407A",
                "#AB47BC",
                "#26A69A"
            };
            return colors[appNameLength % colors.Count];
        }

        internal static string FormatContainerName(string containerName, string reqId)
        {
            if (containerName.StartsWith(Constants.AppContainer))
            {
                var appId = containerName.Replace(Constants.AppContainer, string.Empty);
                if (reqId == appId)
                {
                    return Constants.AppOwnFormattedContainer;
                }
                var appName = GetAppNameFromId(appId) ?? appId;
                return $"{appName} Container";
            }

            if (containerName == Constants.PublicNamesContainer)
            {
                return Constants.PublicNamesFormattedContainer;
            }

            var formattedText = $"{containerName.Substring(1, 1).ToUpper()}{containerName.Substring(2)}";

            switch (formattedText)
            {
                case Constants.DocumentsFormattedContainer:
                case Constants.DownloadsFormattedContainer:
                case Constants.MusicFormattedContainer:
                case Constants.PicturesFormattedContainer:
                case Constants.VideosFormattedContainer:
                case Constants.PublicFormattedContainer:
                    return formattedText;
                default:
                    throw new Exception(string.Format(Constants.InvalidContainer, containerName));
            }
        }

        internal static string FormatContainerNameToImage(string containerName)
        {
            if (containerName.EndsWith("Container"))
            {
                return Constants.AppContainerImage;
            }

            switch (containerName)
            {
                case Constants.PublicFormattedContainer:
                    return Constants.PublicContainerImage;
                case Constants.PublicNamesFormattedContainer:
                    return Constants.PublicNamesContainerImage;
                default:
                    return containerName;
            }
        }

        internal static string GetAppNameFromId(string appId)
        {
            var appContainerName = $"{Constants.AppContainer}{appId}";

            return containerNameList.ContainsKey(appContainerName) ? containerNameList[appContainerName] : null;
        }

        #region Encoding Extensions

        public static string ToUtfString(this List<byte> input)
        {
            var ba = input.ToArray();
            return Encoding.UTF8.GetString(ba, 0, ba.Length);
        }

        public static List<byte> ToUtfBytes(this string input)
        {
            var byteArray = Encoding.UTF8.GetBytes(input);
            return byteArray.ToList();
        }

        public static string ToHexString(this List<byte> byteList)
        {
            var ba = byteList.ToArray();
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", string.Empty).ToLower();
        }

        public static List<byte> ToHexBytes(this string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes.ToList();
        }

        public static string PrintByteArray(List<byte> bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }

            sb.Append("}");
            return sb.ToString();
        }

        #endregion

        public static void UpdateAppContainerNameList(string appId, string appName)
        {
            if (containerNameList == null)
                containerNameList = new Dictionary<string, string>();

            var appContainerName = $"{Constants.AppContainer}{appId}";

            if (!containerNameList.ContainsKey(appContainerName))
                containerNameList.Add(appContainerName, appName);
        }
    }
}
