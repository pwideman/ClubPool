using System;

namespace ClubPool.Web.Infrastructure
{
  public class CreateScheduleException : Exception
  {
    public CreateScheduleException() : base() {
    }

    public CreateScheduleException(string message)
      : base(message) {
    }

  }
}
