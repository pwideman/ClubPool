using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Core
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
