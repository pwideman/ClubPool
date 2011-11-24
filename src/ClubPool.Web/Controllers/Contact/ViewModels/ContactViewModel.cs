using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Contact.ViewModels
{
  public class ContactViewModel
  {
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    public string Name { get; set; }

    [Required]
    [Email]
    [DisplayName("Reply to Address:")]
    public string ReplyToAddress { get; set; }

    [Required]
    [DisplayName("Subject:")]
    public string Subject { get; set; }

    [Required]
    [DisplayName("Body:")]
    public string Body { get; set; }

    public ContactViewModel(User player, User sender) {
      Id = player.Id;
      Name = player.FullName;
      ReplyToAddress = sender.Email;
    }

    public ContactViewModel(Team team, User sender) {
      Id = team.Id;
      Name = team.Name;
      ReplyToAddress = sender.Email;
    }

  }
}
