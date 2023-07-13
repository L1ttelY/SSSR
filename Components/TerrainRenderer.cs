using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CGTest.Components {
	public class TerrainRenderer:Renderer {

		int GetIndex(int x,int y) => (resoulution+1)*y+x;
		Vector3 GetVertex(int x,int y) => new Vector3(vertices[GetIndex(x,y)*14],vertices[GetIndex(x,y)*14+1],vertices[GetIndex(x,y)*14+2]);

		Vector3 GenerateFaceNormal(Vector3 p1,Vector3 p2,Vector3 p3){
			Vector3 e1 = p2-p1;
			Vector3 e2 = p3-p1;
			Vector3 normal = Vector3.Cross(e1,e2);
			if(normal.Y<0) normal=-normal;
			normal.Normalize();
			return normal;
		}

		Vector3[] adjacentBuffer = new Vector3[8];

		public TerrainRenderer(int resoulution,float textureTileTimes) {
			this.resoulution=resoulution;
			vertices=new float[(resoulution+1)*(resoulution+1)*14];
			indices=new uint[resoulution*resoulution*6];

			if(textureTileTimes<1) textureTileTimes=1;

			for(int x = 0;x<=resoulution;x++)
				for(int y = 0;y<=resoulution;y++) {
					int index = GetIndex(x,y)*14;

					float xf = (x-resoulution/2)/(float)resoulution;
					float yf = (y-resoulution/2)/(float)resoulution;
					float u = (float)x/(float)resoulution;
					float v = (float)y/(float)resoulution;

					vertices[index+0]=xf;
					vertices[index+1]=0;
					vertices[index+2]=yf;
					vertices[index+3]=u*textureTileTimes;
					vertices[index+4]=v*textureTileTimes;
					vertices[index+5]=0;
					vertices[index+6]=1;
					vertices[index+7]=0;
					vertices[index+8]=1;
					vertices[index+9]=0;
					vertices[index+10]=0;
					vertices[index+11]=0;
					vertices[index+12]=0;
					vertices[index+13]=1;

					//右上方正方形
					if(x<resoulution&&y<resoulution) {
						int indexIndex = (resoulution*y+x)*6;
						indices[indexIndex+0]=(uint)GetIndex(x+0,y+0);
						indices[indexIndex+1]=(uint)GetIndex(x+0,y+1);
						indices[indexIndex+2]=(uint)GetIndex(x+1,y+0);
						indices[indexIndex+3]=(uint)GetIndex(x+1,y+0);
						indices[indexIndex+4]=(uint)GetIndex(x+0,y+1);
						indices[indexIndex+5]=(uint)GetIndex(x+1,y+1);
					}

				}

		}

		public void RiseTerrain(float amount,Vector2 center,float radiusIn,float radiusOut) {

			center+=new Vector2(resoulution/2,resoulution/2);

			for(int x = 0;x<resoulution+1;x++)
				for(int y = 0;y<resoulution+1;y++) {
					float distanceToCenter = Vector2.Distance(center,new Vector2(x,y));
					if(distanceToCenter>radiusOut) continue;

					float amountHere = amount;
					if(distanceToCenter>radiusIn)
						amountHere=MathHelper.Lerp(amount,0,(distanceToCenter-radiusIn)/(radiusOut-radiusIn));

					vertices[GetIndex(x,y)*14+1]+=amountHere;
				}

			//更新法线切线
			for(int x = 0;x<resoulution+1;x++)
				for(int y = 0;y<resoulution+1;y++) {
					Vector3 thisPotition = GetVertex(x,y);
					int m = resoulution;
					float d = 1/(float)resoulution;
					//567
					//3 4
					//012
					adjacentBuffer[0]=(x==0||y==0) ? thisPotition+new Vector3(-d,0,-d) : GetVertex(x-1,y-1);
					adjacentBuffer[1]=(      y==0) ? thisPotition+new Vector3( 0,0,-d) : GetVertex(x  ,y-1);
					adjacentBuffer[2]=(x==m||y==0) ? thisPotition+new Vector3( d,0,-d) : GetVertex(x+1,y-1);
					adjacentBuffer[3]=(x==0      ) ? thisPotition+new Vector3(-d,0, 0) : GetVertex(x-1,y  );
					adjacentBuffer[4]=(x==m      ) ? thisPotition+new Vector3( d,0, 0) : GetVertex(x+1,y  );
					adjacentBuffer[5]=(x==0||y==m) ? thisPotition+new Vector3(-d,0, d) : GetVertex(x-1,y+1);
					adjacentBuffer[6]=(      y==m) ? thisPotition+new Vector3( 0,0, d) : GetVertex(x  ,y+1);
					adjacentBuffer[7]=(x==m||y==m) ? thisPotition+new Vector3( d,0, d) : GetVertex(x+1,y+1);

					Vector3 normal = Vector3.Zero;
					normal+=GenerateFaceNormal(thisPotition,adjacentBuffer[0],adjacentBuffer[1]);
					normal+=GenerateFaceNormal(thisPotition,adjacentBuffer[1],adjacentBuffer[4]);
					normal+=GenerateFaceNormal(thisPotition,adjacentBuffer[4],adjacentBuffer[7]);
					normal+=GenerateFaceNormal(thisPotition,adjacentBuffer[7],adjacentBuffer[6]);
					normal+=GenerateFaceNormal(thisPotition,adjacentBuffer[6],adjacentBuffer[3]);
					normal+=GenerateFaceNormal(thisPotition,adjacentBuffer[3],adjacentBuffer[0]);
					normal.Normalize();
					Vector3 tangent = new Vector3(normal.Y,-normal.X,0);
					tangent.Normalize();
					Vector3 bitangent = new Vector3(0,-normal.Z,normal.Y);
					bitangent.Normalize();


					int index = GetIndex(x,y)*14;
					vertices[index+5]=normal.X;
					vertices[index+6]=normal.Y;
					vertices[index+7]=normal.Z;
					vertices[index+8]=tangent.X;
					vertices[index+9]=tangent.Y;
					vertices[index+10]=tangent.Z;
					vertices[index+11]=bitangent.X;
					vertices[index+12]=bitangent.Y;
					vertices[index+13]=bitangent.Z;
				}

		}


		float[] vertices;
		uint[] indices;

		readonly int resoulution = 100;

		protected override void DoRender() {
			base.DoRender();
			GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length*sizeof(float),vertices,BufferUsageHint.DynamicDraw);
			GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length*sizeof(uint),indices,BufferUsageHint.DynamicDraw);
			GL.DrawElements(PrimitiveType.Triangles,indices.Length,DrawElementsType.UnsignedInt,0);
		}


	}
}
