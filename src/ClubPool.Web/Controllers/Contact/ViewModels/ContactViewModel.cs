using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DataAnnotationsExtensions;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Contact.ViewModels
{
  public class ContactViewModel
  {
    [Min(1)]
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
  }
}
