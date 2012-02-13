using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubPool.Web.Infrastructure
{
  public class AssetManager
  {
    private Dictionary<string, Asset> assets = new Dictionary<string, Asset>();

    public void AddAsset(string name, string localDebugPath, string localPath, string remotePath) {
      var asset = new Asset {
        LocalDebugPath = VirtualPathUtility.ToAbsolute(localDebugPath),
        LocalPath = VirtualPathUtility.ToAbsolute(localPath),
        RemotePath = remotePath
      };
      assets.Add(name, asset);
    }

    public void AddAsset(string name, string localPath, string remotePath) {
      var asset = new Asset {
        LocalPath = VirtualPathUtility.ToAbsolute(localPath),
        RemotePath = remotePath
      };
      assets.Add(name, asset);
    }

    public string GetAssetPath(string name) {
      if (!assets.ContainsKey(name)) {
        throw new ArgumentOutOfRangeException("name");
      }
      var asset = assets[name];
      if (null != HttpContext.Current && HttpContext.Current.IsDebuggingEnabled) {
        return !string.IsNullOrWhiteSpace(asset.LocalDebugPath) ? asset.LocalDebugPath : asset.LocalPath;
      }
      else if (string.IsNullOrWhiteSpace(asset.RemotePath)) {
        return asset.LocalPath;
      }
      else {
        return asset.RemotePath;
      }
    }

    public string GetLocalAssetPath(string name) {
      if (!assets.ContainsKey(name)) {
        throw new ArgumentOutOfRangeException("name");
      }
      return assets[name].LocalPath;
    }
  }

  public class Asset
  {
    public string LocalDebugPath { get; set; }
    public string LocalPath { get; set; }
    public string RemotePath { get; set; }
  }
}