using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipLoader.API.AssetBundles
{
	public class AssetBundleReference
	{

		public AssetBundle assetBundle => _request.assetBundle;

		private AssetBundleCreateRequest _request;

		public AssetBundleReference(string filePath)
		{
			_request = AssetBundle.LoadFromFileAsync(filePath);
		}
	}
}
