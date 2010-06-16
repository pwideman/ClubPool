using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Core
{
  public class EntityDto
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
