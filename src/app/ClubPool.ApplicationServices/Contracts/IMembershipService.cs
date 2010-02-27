﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace ClubPool.ApplicationServices.Contracts
{
  public interface IMembershipService
  {
    bool ValidateUser(string username, string password);
    void CreateUser(string username, string password, string email, bool isApproved);
  }
}
