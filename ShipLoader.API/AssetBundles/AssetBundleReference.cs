﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShipLoader.API.AssetBundles
{
	public class AssetBundleReference
	{
		AssetBundle AssetBundle => _request.assetBundle;

		private AssetBundleCreateRequest _request;

		public AssetBundleReference(string filePath)
		{
			_request = AssetBundle.LoadFromFileAsync(filePath);
		}
	}
}
