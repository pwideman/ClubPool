using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using ClubPool.Web.Infrastructure;

namespace ClubPool.Web.Models
{
  public class Role : Entity
  {
    [Required]
    public string Name { get; set; }
    public virtual ICollection<User> Users { get; private set; }
    
    protected Role() {
      InitMembers();
    }

    public Role(string name)
      : this() {
      Arg.NotNull(name, "name");

      Name = name;
    }

    protected virtual void InitMembers() {
      Users = new List<User>();
    }


  }
}
