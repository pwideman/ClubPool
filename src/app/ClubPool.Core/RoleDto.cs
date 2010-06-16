using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NHibernate.Validator.Constraints;

namespace ClubPool.Core
{
  public class RoleDto : EntityDto
  {
    public RoleDto() {
    }

    public RoleDto(Role role) {
      if (null != role) {
        Id = role.Id;
        Name = role.Name;
      }
    }

    [DisplayName("Name:")]
    [NotNullNotEmpty(Message = "Required")]
    public string Name { get; set; }
  }
}
