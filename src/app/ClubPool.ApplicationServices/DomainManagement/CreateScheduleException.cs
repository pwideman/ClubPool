using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.ApplicationServices.DomainManagement
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
