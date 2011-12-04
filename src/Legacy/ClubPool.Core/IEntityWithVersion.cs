using System;
using SharpArch.Core.DomainModel;
using SharpArch.Core;

namespace ClubPool.Core
{
  public interface IEntityWithVersion
  {
    int Version { get; }
  }
}
