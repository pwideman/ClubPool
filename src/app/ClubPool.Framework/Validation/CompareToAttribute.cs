using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Mappings;
using xVal.Rules;

namespace ClubPool.Framework.Validation
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
  [ValidatorClass(typeof(CompareValidator))]
  public class CompareAttribute : Attribute, IValidateMultipleProperties
  {
    public CompareAttribute() {
      Message = "Must equal other property";
      PrimaryPropertyName = "";
      PropertyToCompare = "";
    }
    public string Message { get; set; }
    public string PrimaryPropertyName { get; set; }
    public string PropertyToCompare { get; set; }
    public ComparisonRule.Operator Operator { get; set; }
  }

  public class CompareValidator : IValidator
  {
    public bool IsValid(object value, IConstraintValidatorContext constraintValidatorContext) {
      var classMapping = new ReflectionClassMapping(value.GetType());
      var compareAttributes = classMapping.GetClassAttributes().OfType<CompareAttribute>()
        .Where(a => a.Message.Equals(constraintValidatorContext.DefaultErrorMessage));
      if (compareAttributes.Count() != 1) {
        throw new CompareAttributeException(
          string.Format("CompareTo class attributes must have unique messages: {0}", 
          constraintValidatorContext.DefaultErrorMessage));
      }
      var compareAttribute = compareAttributes.Single();
      var propertyValue = GetPropertyValue(value, compareAttribute.PrimaryPropertyName);
      var comparisonValue = GetPropertyValue(value, compareAttribute.PropertyToCompare);
      var valid = propertyValue.Equals(comparisonValue);
      if (compareAttribute.Operator == ComparisonRule.Operator.DoesNotEqual) {
        valid = !valid;
      }
      return valid;
    }

    private object GetPropertyValue(object obj, string propertyName) {
      var pi = obj.GetType().GetProperty(propertyName);
      return pi.GetValue(obj, null);
    }
  }

  public class CompareAttributeException : Exception
  {
    public CompareAttributeException(string message) : base(message) {}
  }


}
