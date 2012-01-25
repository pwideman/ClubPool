using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Controllers.Shared.ViewModels;

namespace ClubPool.Web.Controllers.UpdateMatch
{
  public class UpdateMatchViewModel
  {
    [Min(1, ErrorMessage = "Id required")]
    public int Id { get; set; }

    public string Date { get; set; }
    public string Time { get; set; }
    public bool IsForfeit { get; set; }

    [Min(1, ErrorMessage = "You must select a winner")]
    public int Winner { get; set; }

    [Min(1, ErrorMessage = "Player 1 Id required")]
    public int Player1Id { get; set; }

    [Min(0, ErrorMessage = "Player 1 innings must be >= 0")]
    public int Player1Innings { get; set; }

    [Min(0, ErrorMessage = "Player 1 defensive shots must be >= 0")]
    public int Player1DefensiveShots { get; set; }

    [Range(0, 9, ErrorMessage = "Player 1 wins must be between 0 and 9")]
    public int Player1Wins { get; set; }

    [Min(1, ErrorMessage = "Player 2 Id required")]
    public int Player2Id { get; set; }

    [Min(0, ErrorMessage = "Player 2 innings must be >= 0")]
    public int Player2Innings { get; set; }

    [Min(0, ErrorMessage = "Player 2 defensive shots must be >= 0")]
    public int Player2DefensiveShots { get; set; }

    [Range(0, 9, ErrorMessage = "Player 2 wins must be between 0 and 9")]
    public int Player2Wins { get; set; }
  }

  public class UpdateMatchResponseViewModel : AjaxUpdateResponseViewModel
  {
    public IEnumerable<ValidationResultViewModel> ValidationResults { get; set; }

    public UpdateMatchResponseViewModel(bool success) {
      ValidationResults = new List<ValidationResultViewModel>();
      Success = success;
    }

    public UpdateMatchResponseViewModel(bool success, string message)
      : this(success) {
      Message = message;
    }

    public UpdateMatchResponseViewModel(bool success, string message, ModelStateDictionary modelState)
      : this(success, message) {
      var results = new List<ValidationResultViewModel>();
      foreach (var result in modelState.Where(kvp => kvp.Value.Errors.Count > 0)) {
        results.Add(new ValidationResultViewModel(result));
      }
      ValidationResults = results;
    }
  }

  public class ValidationResultViewModel
  {
    public object AttemptedValue { get; set; }
    public string Message { get; set; }
    public string PropertyName { get; set; }

    public ValidationResultViewModel(KeyValuePair<string, ModelState> result) {
      if (null != result.Value.Value) {
        AttemptedValue = result.Value.Value.AttemptedValue;
      }
      Message = result.Value.Errors.First().ErrorMessage;
      PropertyName = result.Key;
    }
  }

}
