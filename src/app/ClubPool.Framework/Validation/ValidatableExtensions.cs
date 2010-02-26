// this code is based on the ValidatableExtensions class in Who Can Help Me (http://whocanhelpme.codeplex.com)

using System.Collections.Generic;
using System.Linq;

using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

using xVal.ServerSide;

namespace ClubPool.Framework.Validation
{
  /// <summary>
  /// Extension methods for Entities
  /// </summary>
  public static class ValidatableExtensions
  {
    /// <summary>
    /// Validates an object that implements IValidatable and throws a rules exception if there are validation errors
    /// </summary>
    /// <param name="validatable">
    /// The object to validate.
    /// </param>
    /// <exception cref="RulesException">
    /// </exception>
    public static void Validate(this IValidatable validatable) {
      if (!validatable.IsValid()) {
        var errors = new List<ErrorInfo>();
        var results = validatable.ValidationResults();
        foreach (var result in results) {
          errors.Add(result.GetErrorInfo());
        }
        throw new RulesException(errors);
      }
    }

    /// <summary>
    /// Gets the ErrorInfo from the validation result and the parent entity type
    /// </summary>
    /// <param name="result">
    /// The validation result.
    /// </param>
    /// <returns>
    /// The ErrorInfo
    /// </returns>
    private static ErrorInfo GetErrorInfo(this IValidationResult result) {
      if (string.IsNullOrEmpty(result.PropertyName)) {
        return GetClassLevelErrorInfo(result);
      }
      return GetPropertyLevelErrorInfo(result);
    }

    /// <summary>
    /// Returns an ErrorInfo for a property level validation result
    /// </summary>
    /// <param name="result">
    /// The validation result.
    /// </param>
    /// <returns>
    /// The ErrorInfo
    /// </returns>
    private static ErrorInfo GetPropertyLevelErrorInfo(IValidationResult result) {
      return new ErrorInfo(((ValidationResult)result).InvalidValue.PropertyPath, result.Message);
    }

    /// <summary>
    /// Returns an ErrorInfo for a class level validation result
    /// </summary>
    /// <param name="result">
    /// The validation result.
    /// </param>
    /// <returns>
    /// The ErrorInfo
    /// </returns>
    private static ErrorInfo GetClassLevelErrorInfo(IValidationResult result) {
      var errorInfo = new ErrorInfo(string.Empty, result.Message);

      // Get the validation attributes on the entity type
      var validatorProperties = result.ClassContext.GetCustomAttributes(false)
          .Where(x => typeof(IValidateMultipleProperties).IsAssignableFrom(x.GetType()));

      // If the validation message matches one of the attributes messages,
      // then set the correct property path, based on the primary property name
      foreach(var x in validatorProperties) {
        if (result.Message == ((IValidateMultipleProperties)x).Message) {
          errorInfo = new ErrorInfo(((ValidationResult)result).InvalidValue.PropertyPath + ((IValidateMultipleProperties)x).PrimaryPropertyName, result.Message);
        }
      }

      return errorInfo;
    }
  }
}