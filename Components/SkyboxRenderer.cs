using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CGTest.Components {
	class SkyboxRenderer:Renderer {

		public static float[] vertices ={
		//-x
		-1000,-1000,-1000,  1f/4f,1f/3f, -1,0,0, 0,0,1, 0,1,0,
		-1000, 1000,-1000,  1f/4f,2f/3f, -1,0,0, 0,0,1, 0,1,0,
		-1000,-1000, 1000,  2f/4f,1f/3f, -1,0,0, 0,0,1, 0,1,0,
		-1000, 1000, 1000,  2f/4f,2f/3f, -1,0,0, 0,0,1, 0,1,0,
		//+x								
		 1000,-1000,-1000,  4f/4f,1f/3f, 1,0,0, 0,0,1,  0,1,0,
		 1000, 1000,-1000,  4f/4f,2f/3f, 1,0,0, 0,0,1,  0,1,0,
		 1000,-1000, 1000,  3f/4f,1f/3f, 1,0,0, 0,0,1,  0,1,0,
		 1000, 1000, 1000,  3f/4f,2f/3f, 1,0,0, 0,0,1,  0,1,0,
		//-y								
		-1000,-1000,-1000,  1f/4f,1f/3f, 0,-1,0, 1,0,0, 0,0,1,
		 1000,-1000,-1000,  1f/4f,0f/3f, 0,-1,0, 1,0,0, 0,0,1,
		-1000,-1000, 1000,  2f/4f,1f/3f, 0,-1,0, 1,0,0, 0,0,1,
		 1000,-1000, 1000,  2f/4f,0f/3f, 0,-1,0, 1,0,0, 0,0,1,
		//+y								
		-1000, 1000,-1000,  1f/4f,2f/3f, 0,1,0, 1,0,0, 0,0,1,
		 1000, 1000,-1000,  1f/4f,3f/3f, 0,1,0, 1,0,0, 0,0,1,
		-1000, 1000, 1000,  2f/4f,2f/3f, 0,1,0, 1,0,0, 0,0,1,
		 1000, 1000, 1000,  2f/4f,3f/3f, 0,1,0, 1,0,0, 0,0,1,
		//-z								
		-1000,-1000,-1000,  1f/4f,1f/3f, 0,0,-1, 1,0,0, 0,1,0,
		 1000,-1000,-1000,  0f/4f,1f/3f, 0,0,-1, 1,0,0, 0,1,0,
		-1000, 1000,-1000,  1f/4f,2f/3f, 0,0,-1, 1,0,0, 0,1,0,
		 1000, 1000,-1000,  0f/4f,2f/3f, 0,0,-1, 1,0,0, 0,1,0,
		//+z
		-1000,-1000, 1000,  2f/4f,1f/3f, 0,0,1, 1,0,0, 0,1,0,
		 1000,-1000, 1000,  3f/4f,1f/3f, 1,0,0, 0,1,0, 0,1,0,
		-1000, 1000, 1000,  2f/4f,2f/3f, 1,0,0, 0,1,0, 0,1,0,
		 1000, 1000, 1000,  3f/4f,2f/3f, 1,0,0, 0,1,0, 0,1,0,

		};

		public static uint[] indices = {
			0, 1, 2,  1, 3, 2,
			4, 6, 5,  5, 6, 7,
			8, 10, 9,  9,10,11,
			12,13,14,  13,15,14,
			16,17,18,  17,19,18,
			20,22,21,  21,22,23,
		};

		protected override void DoRender() {

			Matrix4 model = Matrix4.CreateTranslation(Game.instance.currentCamera.transform.LocalToWorldMatrix.ExtractTranslation());
			Matrix4 mvp = model*Game.instance.ViewProjectionMatrix;

			Shader.SetMvp(mvp,model,Game.instance.ViewProjectionMatrix);

			if(!(lastRendered is CubeRenderer)) {
				GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length*sizeof(float),vertices,BufferUsageHint.DynamicDraw);
				GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length*sizeof(uint),indices,BufferUsageHint.DynamicDraw);
			}
			GL.DrawElements(PrimitiveType.Triangles,indices.Length,DrawElementsType.UnsignedInt,0);
		}

	}
}
