using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using xVal.Rules;

namespace ClubPool.Framework.Validation
{
  public class CompareToAttribute : Attribute, IValidateMultipleProperties
  {
    public CompareToAttribute() {
      Message = "Must equal other property";
      PrimaryPropertyName = "";
      PropertyToCompare = "";
    }
    public string Message { get; set; }
    public string PrimaryPropertyName { get; set; }
    public string PropertyToCompare { get; set; }
    public ComparisonRule.Operator Operator { get; set; }
  }
}
