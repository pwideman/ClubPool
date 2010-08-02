using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClubPool.Framework.Validation;

namespace ClubPool.Core
{
  public class EntityDto : ValidatableObject
  {
    public int Id { get; set; }

    public override int GetHashCode() {
      return Id;
    }

    public override bool Equals(object obj) {
      var dto = obj as EntityDto;
      if (null == dto) {
        return false;
      }
      else {
        return Id == dto.Id;
      }
    }
  
  }
}
