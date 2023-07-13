using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Mathematics;

namespace CGTest {
	class ObjFile {
		static Vector3 GenVector3(string[] source,int offset) => new Vector3(
					float.Parse(source[offset+0]),
					float.Parse(source[offset+1]),
					float.Parse(source[offset+2])
				);
		private class Vertex {
			public int positionIndex;
			public int textIndex;
			public int normalIndex;
			public Vector3 tangent;
			public Vector3 bitangent;
		}

		private class Face {
			public int vertex1;
			public int vertex2;
			public int vertex3;
			public string material;
		}

		static Dictionary<Vertex,(Vertex, int)> vertexBuffer = new Dictionary<Vertex,(Vertex, int)>();
		static List<Vertex> vertexList = new List<Vertex>();
		static List<Vector3> positionBuffer = new List<Vector3>();
		static List<Vector2> textBuffer = new List<Vector2>();
		static List<Vector3> normalBuffer = new List<Vector3>();
		static List<Face> faceBuffer = new List<Face>();
		static Dictionary<string,Material> materials;

		static int GenerateVertex(string source) {
			string[] elements = source.Split('/');
			Vertex newVertex = new Vertex();
			if(elements.Length>0&&elements[0].Length>0) newVertex.positionIndex=int.Parse(elements[0]);
			if(elements.Length>1&&elements[1].Length>0) newVertex.textIndex=int.Parse(elements[1]);
			if(elements.Length>2&&elements[2].Length>0) newVertex.normalIndex=int.Parse(elements[2]);

			if(vertexBuffer.ContainsKey(newVertex)) {
				return vertexBuffer[newVertex].Item2;
			}
			vertexBuffer.Add(newVertex,(newVertex, vertexBuffer.Count));
			vertexList.Add(newVertex);
			return vertexBuffer[newVertex].Item2;

		}

		public static MeshData ReadFile(string path) {

			positionBuffer.Clear();
			textBuffer.Clear();
			normalBuffer.Clear();
			faceBuffer.Clear();
			vertexBuffer.Clear();
			vertexList.Clear();

			positionBuffer.Add(Vector3.Zero);
			textBuffer.Add(Vector2.Zero);
			normalBuffer.Add(Vector3.Zero);

			string[] data = File.ReadAllLines(path);

			string folderPath = path;
			int lastIndex = folderPath.LastIndexOf('/');
			if(lastIndex==-1) lastIndex=folderPath.LastIndexOf('\\');
			folderPath=folderPath.Remove(lastIndex)+'/';

			string currentMaterial = "";
			foreach(string line in data) {
				if(line.Length==0||line[0]=='#') continue;

				string[] elements = line.Split(' ');

				if(elements[0]=="v") {
					positionBuffer.Add(new Vector3(GenVector3(elements,1)));
				} else if(elements[0]=="vn") {
					normalBuffer.Add(GenVector3(elements,1));
				} else if(elements[0]=="vt") {
					textBuffer.Add(new Vector2(
						float.Parse(elements[1]),
						float.Parse(elements[2])
					));
				} else if(elements[0]=="f") {
					for(int endIndex = 3;endIndex<elements.Length;endIndex++) {
						Face newFace = new Face();
						newFace.vertex1=GenerateVertex(elements[1]);
						newFace.vertex2=GenerateVertex(elements[endIndex-1]);
						newFace.vertex3=GenerateVertex(elements[endIndex]);
						newFace.material=currentMaterial;
						faceBuffer.Add(newFace);
					}
				} else if(elements[0]=="mtllib") {
					materials=MtlFile.LoadFile(folderPath+elements[1]);
				} else if(elements[0]=="usemtl") {
					currentMaterial=elements[1];
				}
			}

			MeshData result = new MeshData();
			result.elements=new List<MeshElement>();
			result.vertices=new float[vertexBuffer.Count*14];
			Dictionary<string,List<uint>> meshElements = new Dictionary<string,List<uint>>();

			foreach(var i in faceBuffer) {
				if(!meshElements.ContainsKey(i.material)) meshElements.Add(i.material,new List<uint>());
				meshElements[i.material].Add((uint)i.vertex1);
				meshElements[i.material].Add((uint)i.vertex2);
				meshElements[i.material].Add((uint)i.vertex3);

				//calculate face tangent
				Vector3 position1 = positionBuffer[vertexList[i.vertex1].positionIndex];
				Vector3 position2 = positionBuffer[vertexList[i.vertex2].positionIndex];
				Vector3 position3 = positionBuffer[vertexList[i.vertex3].positionIndex];
				Vector2 uv1 = textBuffer[vertexList[i.vertex1].textIndex];
				Vector2 uv2 = textBuffer[vertexList[i.vertex2].textIndex];
				Vector2 uv3 = textBuffer[vertexList[i.vertex3].textIndex];
				Matrix2x3 matrixT = CalculateTangent(position1,position2,position3,uv1,uv2,uv3);
				matrixT.Row0.Normalize();
				matrixT.Row1.Normalize();
				vertexList[i.vertex1].tangent+=matrixT.Row0;
				vertexList[i.vertex2].tangent+=matrixT.Row0;
				vertexList[i.vertex3].tangent+=matrixT.Row0;
				vertexList[i.vertex1].bitangent+=matrixT.Row1;
				vertexList[i.vertex2].bitangent+=matrixT.Row1;
				vertexList[i.vertex3].bitangent+=matrixT.Row1;

			}
			foreach(var i in vertexBuffer) {
				int index = i.Value.Item2;
				i.Key.tangent.Normalize();
				result.vertices[index*14+0]=positionBuffer[i.Key.positionIndex].X;
				result.vertices[index*14+1]=positionBuffer[i.Key.positionIndex].Y;
				result.vertices[index*14+2]=positionBuffer[i.Key.positionIndex].Z;
				result.vertices[index*14+3]=textBuffer[i.Key.textIndex].X;
				result.vertices[index*14+4]=textBuffer[i.Key.textIndex].Y;
				result.vertices[index*14+5]=normalBuffer[i.Key.normalIndex].X;
				result.vertices[index*14+6]=normalBuffer[i.Key.normalIndex].Y;
				result.vertices[index*14+7]=normalBuffer[i.Key.normalIndex].Z;
				result.vertices[index*14+8]=i.Key.tangent.X;
				result.vertices[index*14+9]=i.Key.tangent.Y;
				result.vertices[index*14+10]=i.Key.tangent.Z;
				result.vertices[index*14+11]=i.Key.bitangent.X;
				result.vertices[index*14+12]=i.Key.bitangent.Y;
				result.vertices[index*14+13]=i.Key.bitangent.Z;
			}
			foreach(var i in meshElements) {
				MeshElement newElement = new MeshElement();
				newElement.material=materials[i.Key];
				newElement.indices=i.Value.ToArray();
				result.elements.Add(newElement);
			}

			return result;

		}

		static Matrix2x3 CalculateTangent(Vector3 p1,Vector3 p2,Vector3 p3,Vector2 uv1,Vector2 uv2,Vector2 uv3) {

			Vector3 E1 = p2-p1;
			Vector3 E2 = p3-p1;
			Matrix2x3 matrixE = new Matrix2x3();
			matrixE.Row0=E1;
			matrixE.Row1=E2;

			Vector2 U1 = uv2-uv1;
			Vector2 U2 = uv3-uv1;
			Matrix2 matrixU = new Matrix2();
			matrixU.Row0=U1;
			matrixU.Row1=U2;
			if(matrixU.Determinant==0) return new Matrix2x3();

			matrixU.Invert();
			return matrixU*matrixE;

		}

	}
}
