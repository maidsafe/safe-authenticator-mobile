using Hexasoft.Zxcvbn;
using SafeAuthenticator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeAuthenticator.Helpers {
  internal static class Utilities {
    static ZxcvbnEstimator estimator;

    internal static ObservableRangeCollection<T> ToObservableRangeCollection<T>(this IEnumerable<T> source) {
      var result = new ObservableRangeCollection<T>();
      foreach (var item in source) {
        result.Add(item);
      }
      return result;
    }

    internal static (double, double, string) StrengthChecker(string data)
    {
      if (estimator == null) estimator = new ZxcvbnEstimator();
      if (string.IsNullOrEmpty(data)) return (0, 0, "");
      string Strength = null;
      var result = estimator.EstimateStrength(data);
      var calc = Math.Log(result.Guesses) / Math.Log(10);
      if (calc < AppConstants.AccStrengthVeryWeak) Strength = "VERY_WEAK";
      else if (calc < AppConstants.AccStrengthWeak) Strength = "WEAK";
      else if (calc < AppConstants.AccStrengthSomeWhatSecure) Strength = "SOMEWHAT_SECURE";
      else if (calc >= AppConstants.AccStrengthSomeWhatSecure) Strength = "SECURE";
      double percentage = Math.Round(Math.Min((calc / 16) * 100, 100));
      return (calc, percentage, Strength);
    }
        #region Encoding Extensions

    public static string ToUtfString(this List<byte> input) {
      var ba = input.ToArray();
      return Encoding.UTF8.GetString(ba, 0, ba.Length);
    }

    public static List<byte> ToUtfBytes(this string input) {
      var byteArray = Encoding.UTF8.GetBytes(input);
      return byteArray.ToList();
    }

    public static string ToHexString(this List<byte> byteList) {
      var ba = byteList.ToArray();
      var hex = BitConverter.ToString(ba);
      return hex.Replace("-", "").ToLower();
    }

    public static List<byte> ToHexBytes(this string hex) {
      var numberChars = hex.Length;
      var bytes = new byte[numberChars / 2];
      for (var i = 0; i < numberChars; i += 2) {
        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
      }
      return bytes.ToList();
    }

    public static string PrintByteArray(List<byte> bytes) {
      var sb = new StringBuilder("new byte[] { ");
      foreach (var b in bytes) {
        sb.Append(b + ", ");
      }
      sb.Append("}");
      return sb.ToString();
    }

    #endregion
  }
}
