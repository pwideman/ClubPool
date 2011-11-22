using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class CreateSeasonViewModel
  {
    [DisplayName("Name:")]
    [Required(ErrorMessage="Required")]
    public string Name { get; set; }
  }
}
