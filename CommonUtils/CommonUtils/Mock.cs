using System;
using System.Linq;

namespace CommonUtils {
  public static class Mock {
    private static readonly Random Random = new Random();

    public static string RandomString(int maxLength, bool fixedLength = false) {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
      var length = fixedLength ? maxLength : Random.Next(10, Math.Max(10, maxLength));
      return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
    }
  }
}
