
namespace ClubPool.Web.Controllers.Shared.ViewModels
{
  public class AjaxUpdateResponseViewModel
  {
    public string Message { get; set; }
    public bool Success { get; set; }

    public AjaxUpdateResponseViewModel() {
      Success = true;
    }

    public AjaxUpdateResponseViewModel(bool success) {
      Success = success;
    }

    public AjaxUpdateResponseViewModel(bool success, string message): this(success) {
      Message = message;
    }
  }
}
