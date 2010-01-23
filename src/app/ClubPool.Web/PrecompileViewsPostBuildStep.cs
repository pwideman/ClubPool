using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using ClubPool.Web.Controllers;

namespace ClubPool.Web
{
  [RunInstaller(true)]
  public partial class PrecompileViewsPostBuildStep : Installer
  {
    public PrecompileViewsPostBuildStep() {
      InitializeComponent();
    }

    // we must manually add the controllers to the batch since they are in a different assembly
    private void precompileInstaller1_DescribeBatch(object sender, Spark.Web.Mvc.Install.DescribeBatchEventArgs e) {
      e.Batch.FromAssembly(typeof(HomeController).Assembly);
    }
  }
}
