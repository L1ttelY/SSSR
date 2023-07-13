using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CGTest {
	class FileUtils {

		static string _assetPath = "";
		public static string AssetPath {
			get {
				if(_assetPath.Length==0) {
					for(int i = 0;i<100;i++) {
						if(Directory.Exists(_assetPath+"Assets")) continue;
						_assetPath+="../";
						if(i==99) {
							_assetPath="";
							Console.WriteLine("ASSET PATH NOT FOUND");
						}
					}
					_assetPath+="Assets/";
				}
				return _assetPath;
			}
		}

	}
}
