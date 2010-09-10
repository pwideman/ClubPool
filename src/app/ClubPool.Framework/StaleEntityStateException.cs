using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClubPool.Framework
{
  public class StaleEntityStateException : Exception
  {
    public StaleEntityStateException()
      : base() {
    }

    public StaleEntityStateException(string entityName, object identifier)
      : base("The object was updated or deleted by another transaction") {
      EntityName = entityName;
      Identifier = identifier;
    }

    public StaleEntityStateException(string entityName, object identifier, Exception innerException)
      : this(entityName, identifier) {
      InnerException = innerException;
    }

    public string EntityName { get; protected set; }
    public object Identifier { get; protected set; }
    public Exception InnerException { get; protected set; }
  }
}
