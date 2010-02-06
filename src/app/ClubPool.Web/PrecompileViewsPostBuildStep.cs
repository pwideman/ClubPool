using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

using ClubPool.Web.Controllers;
using ClubPool.Web.Controllers.Home;
using ClubPool.Web.Code;

using Spark;

namespace ClubPool.Web
{
  [RunInstaller(true)]
  public partial class PrecompileViewsPostBuildStep : Installer
  {
    public PrecompileViewsPostBuildStep() {
      InitializeComponent();
      precompileInstaller1.SettingsInstantiator = () => SparkInitializer.GetSettings();
    }

    // we must manually add the controllers to the batch since they are in a different assembly
    private void precompileInstaller1_DescribeBatch(object sender, Spark.Web.Mvc.Install.DescribeBatchEventArgs e) {
      e.Batch.FromAssembly(typeof(BaseController).Assembly);
    }
  }
}
