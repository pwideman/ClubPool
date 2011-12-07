using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using ClubPool.Web.Models;
using ClubPool.Web.Services.Membership;
using ClubPool.Web.Infrastructure;
using ClubPool.Testing;

namespace ClubPool.Tests.Services.Membership
{
  public abstract class ClubPoolMembershipServiceSpecificationContext : SpecificationContext
  {
    protected ClubPoolMembershipService service;
    protected Mock<IRepository> repository;
    protected List<User> users;

    public override void EstablishContext() {
      repository = new Mock<IRepository>();
      users = new List<User>();
      repository.Setup(r => r.All<User>()).Returns(users.AsQueryable());
      service = new ClubPoolMembershipService(repository.Object);
    }
  }
}
