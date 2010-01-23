﻿using System.Reflection;
using System;

namespace Tests.ClubPool.SharpArchProviders.NHibernateMaps.Conventions
{
  public class ForeignKeyConvention : FluentNHibernate.Conventions.ForeignKeyConvention
  {
    protected override string GetKeyName(FluentNHibernate.Member property, Type type) {
      if (null == property) {
        return type.Name + "Id";
      }
      else {
        return property.Name + "Id";
      }
    }
  }
}
