using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Core
{
  public class RoleDto
  {
    public int Id { get; set; }

    [DisplayName("Name:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Name { get; set; }

    public RoleDto(Role role) {
      if (null != role) {
        Id = role.Id;
        Name = role.Name;
      }
    }
  }
}
