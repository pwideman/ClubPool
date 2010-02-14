// this code is based on the ValidatableExtensions class in Who Can Help Me (http://whocanhelpme.codeplex.com)

using System.Collections.Generic;
using System.Linq;

using SharpArch.Core.CommonValidator;
using SharpArch.Core.NHibernateValidator.CommonValidatorAdapter;

using xVal.ServerSide;

namespace ClubPool.Framework.Extensions
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

  }
}