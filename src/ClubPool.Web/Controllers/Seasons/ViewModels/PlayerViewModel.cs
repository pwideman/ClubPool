
using ClubPool.Web.Models;

namespace ClubPool.Web.Controllers.Seasons.ViewModels
{
  public class PlayerViewModel
  {
    public PlayerViewModel() {
    }

    public PlayerViewModel(User player) {
      Id = player.Id;
      Name = player.FullName;
    }

    public int Id { get; set; }
    public string Name { get; set; }
  }
}
