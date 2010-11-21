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
      byte[] bIn = Encoding.Unicode.GetBytes(password);
      byte[] bSalt = Convert.FromBase64String(salt);
      byte[] bAll = new byte[bSalt.Length + bIn.Length];
      byte[] bRet = null;

      Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
      Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
      HashAlgorithm s = HashAlgorithm.Create("SHA1");
      bRet = s.ComputeHash(bAll);
      return Convert.ToBase64String(bRet);
    }

    public static string GenerateSalt(int size) {
      byte[] buf = new byte[size];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

  }
}
