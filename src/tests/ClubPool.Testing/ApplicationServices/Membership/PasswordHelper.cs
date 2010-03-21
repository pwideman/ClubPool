using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ClubPool.Testing.ApplicationServices.Membership
{
  public static class PasswordHelper
  {
    public static string EncodePassword(string password, string salt) {
      var hash = new HMACSHA1();
      hash.Key = Convert.FromBase64String(salt);
      return Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
    }

    public static string GenerateSalt(int size) {
      byte[] buf = new byte[size];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

  }
}
