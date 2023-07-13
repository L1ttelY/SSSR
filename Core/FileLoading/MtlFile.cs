using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Mathematics;

namespace CGTest {
	static class MtlFile {

		static Vector3 GenVector3(string[] source,int offset) =>
			new Vector3(
				float.Parse(source[offset+0]),
				float.Parse(source[offset+1]),
				float.Parse(source[offset+2])
			);

		static Dictionary<string,Texture> textures = new Dictionary<string,Texture>();

		public static Dictionary<string,Material> LoadFile(string path) {

			textures.Clear();

			Dictionary<string,Material> result = new Dictionary<string,Material>();
			string[] data = File.ReadAllLines(path);

			string folderPath = path;
			int lastIndex = folderPath.LastIndexOf('/');
			if(lastIndex==-1) lastIndex=folderPath.LastIndexOf('\\');
			folderPath=folderPath.Remove(lastIndex)+'/';

			Materials.MaterialDefault currentMaterial = null;
			string currentMaterialName = "";

			foreach(string line in data) {
				if(line.Length>0&&line[0]=='#') continue;
				string[] elements = line.Split(' ');
				string command = elements[0];

				if(command=="newmtl") {
					if(currentMaterialName.Length>0) result[currentMaterialName]=currentMaterial;
					currentMaterial=new Materials.MaterialDefault();
					currentMaterialName=elements[1];
				} else if(command=="map_Kd") {
					string texturePath = folderPath+elements[1];
					if(!textures.ContainsKey(texturePath))textures[folderPath+elements[1]]=new Texture(texturePath);
					currentMaterial.diffuseMap=textures[texturePath];
				} else if(command=="map_Ka") {
					string texturePath = folderPath+elements[1];
					if(!textures.ContainsKey(texturePath)) textures[folderPath+elements[1]]=new Texture(texturePath);
					currentMaterial.ambientMap=textures[texturePath];
				} else if(command=="map_Ks") {
					string texturePath = folderPath+elements[1];
					if(!textures.ContainsKey(texturePath)) textures[folderPath+elements[1]]=new Texture(texturePath);
					currentMaterial.specularMap=textures[texturePath];
				} else if(command=="bump") {
					string texturePath = folderPath+elements[1];
					if(!textures.ContainsKey(texturePath)) textures[folderPath+elements[1]]=new Texture(texturePath);
					currentMaterial.normalMap=textures[texturePath];
				} else if(command=="Ka") {
					currentMaterial.colorAmbient=GenVector3(elements,1);
				} else if(command=="Kd") {
					currentMaterial.colorDiffuse=GenVector3(elements,1);
				} else if(command=="Ks") {
					currentMaterial.colorSpecular=GenVector3(elements,1);
				}
			}
			if(currentMaterialName.Length>0) result[currentMaterialName]=currentMaterial;

			return result;
		}


	}
}
