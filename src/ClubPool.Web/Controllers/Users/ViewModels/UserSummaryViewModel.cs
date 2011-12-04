using System.Linq;

using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Users.ViewModels
{
  public class UserSummaryViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLocked { get; set; }
    public string[] Roles { get; set; }

    public UserSummaryViewModel(User user) {// : this() {
      Id = user.Id;
      Name = user.FullName;
      Username = user.Username;
      Email = user.Email;
      IsApproved = user.IsApproved;
      IsLocked = user.IsLocked;
      if (user.Roles.Any()) {
        Roles = user.Roles.Select(r => r.Name).ToArray();
      }
      else {
        Roles = new string[0];
      }
    }
  }
}
