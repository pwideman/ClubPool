using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using NUnit.Framework;
using FluentAssertions;

using ClubPool.Web.Controllers.Users.ViewModels;
using ClubPool.Testing;

namespace ClubPool.Tests.Controllers.Users
{
  public abstract class EditPostTest : EditTest
  {
    protected EditViewModel viewModel;
    protected bool isApproved = true;
    protected bool isLocked = false;

    public override void EstablishContext() {
      base.EstablishContext();
      viewModel = new EditViewModel() {
        FirstName = "first",
        LastName = "last",
        Username = "newusername",
        Email = "newemail@email.com",
        IsLocked = !isLocked,
        IsApproved = !isApproved,
        Id = user.Id,
        Roles = new int[] { adminRole.Id, officerRole.Id },
        Version = DomainModelHelper.ConvertIntVersionToString(1),
        Password = "newpass",
        ConfirmPassword = "newpass"
      };

      user.IsLocked = isLocked;
      user.IsApproved = isApproved;

      membershipService.Setup(s => s.EncodePassword(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((password, salt) => password);
    }
  }
}
