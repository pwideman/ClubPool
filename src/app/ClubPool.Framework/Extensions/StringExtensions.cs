using System;

namespace ClubPool.Framework.Extensions
{
  public static class StringExtensions
  {
    public static bool IsNullOrWhitespace(this string value) {
      return (string.IsNullOrEmpty(value) || value.Trim().Equals(string.Empty));
    }

    //public static bool IsNonEmptyWithLengthLessThan(this string value, int length) {
    //  return (!value.IsNullOrEmptyOrBlank() && value.Length < length);
    //}
  }
}
