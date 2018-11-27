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

        internal static ObservableRangeCollection<T> ToObservableRangeCollection<T>(this IEnumerable<T> source)
        {
            var result = new ObservableRangeCollection<T>();
            foreach (var item in source)
            {
                result.Add(item);
            }

            return result;
        }

        internal static (double, double, string) StrengthChecker(string data)
        {
            if (_estimator == null)
                _estimator = new ZxcvbnEstimator();
            if (string.IsNullOrEmpty(data))
                return (0, 0, string.Empty);
            string strength = null;
            var result = _estimator.EstimateStrength(data);
            var calc = Math.Log(result.Guesses) / Math.Log(10);
            if (calc < AppConstants.AccStrengthVeryWeak)
                strength = "VERY_WEAK";
            else if (calc < AppConstants.AccStrengthWeak)
                strength = "WEAK";
            else if (calc < AppConstants.AccStrengthSomeWhatSecure)
                strength = "SOMEWHAT_SECURE";
            else if (calc >= AppConstants.AccStrengthSomeWhatSecure)
                strength = "SECURE";
            double percentage = Math.Round(Math.Min((calc / 16) * 100, 100));
            return (calc, percentage, strength);
        }

        internal static string GetErrorMessage(FfiException error)
        {
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
                return "No internet connection";
            switch (error.ErrorCode)
            {
                case -2000:
                    return "Could not connect to the SAFE Network";
                case -101:
                    return "Account does not exist";
                case -3:
                    return "Incorrect password";
                case -102:
                    return "Account already exists";
                case -116:
                    return "Invalid invitation";
                case -117:
                    return "Invitation already claimed";
                default:
                    return error.Message;
            }
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
    }
}
