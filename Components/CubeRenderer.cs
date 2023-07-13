using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
namespace CGTest.Components {
	class CubeRenderer:Renderer {
		
		public static float[] vertices ={
		//-x
		-0.5f,-0.5f,-0.5f,  0,0, -1,0,0, 0,0,1, 0,1,0,
		-0.5f, 0.5f,-0.5f,  0,1, -1,0,0, 0,0,1, 0,1,0,
		-0.5f,-0.5f, 0.5f,  1,0, -1,0,0, 0,0,1, 0,1,0,
		-0.5f, 0.5f, 0.5f,  1,1, -1,0,0, 0,0,1, 0,1,0,
		//+x
		 0.5f,-0.5f,-0.5f,  0,0, 1,0,0, 0,0,1,  0,1,0,
		 0.5f, 0.5f,-0.5f,  0,1, 1,0,0, 0,0,1,  0,1,0,
		 0.5f,-0.5f, 0.5f,  1,0, 1,0,0, 0,0,1,  0,1,0,
		 0.5f, 0.5f, 0.5f,  1,1, 1,0,0, 0,0,1,  0,1,0,
		//-y
		-0.5f,-0.5f,-0.5f,  0,0, 0,-1,0, 1,0,0, 0,0,1,
		 0.5f,-0.5f,-0.5f,  1,0, 0,-1,0, 1,0,0, 0,0,1,
		-0.5f,-0.5f, 0.5f,  0,1, 0,-1,0, 1,0,0, 0,0,1,
		 0.5f,-0.5f, 0.5f,  1,1, 0,-1,0, 1,0,0, 0,0,1,
		//+y
		-0.5f, 0.5f,-0.5f,  0,0, 0,1,0, 1,0,0, 0,0,1,
		 0.5f, 0.5f,-0.5f,  1,0, 0,1,0, 1,0,0, 0,0,1,
		-0.5f, 0.5f, 0.5f,  0,1, 0,1,0, 1,0,0, 0,0,1,
		 0.5f, 0.5f, 0.5f,  1,1, 0,1,0, 1,0,0, 0,0,1,
		//-z
		-0.5f,-0.5f,-0.5f,  0,0, 0,0,-1, 1,0,0, 0,1,0,
		 0.5f,-0.5f,-0.5f,  1,0, 0,0,-1, 1,0,0, 0,1,0,
		-0.5f, 0.5f,-0.5f,  0,1, 0,0,-1, 1,0,0, 0,1,0,
		 0.5f, 0.5f,-0.5f,  1,1, 0,0,-1, 1,0,0, 0,1,0,
		//+z
		-0.5f,-0.5f, 0.5f,  0,0, 0,0,1, 1,0,0, 0,1,0,
		 0.5f,-0.5f, 0.5f,  1,0, 0,0,1, 1,0,0, 0,1,0,
		-0.5f, 0.5f, 0.5f,  0,1, 0,0,1, 1,0,0, 0,1,0,
		 0.5f, 0.5f, 0.5f,  1,1, 0,0,1, 1,0,0, 0,1,0,

		};

		public static uint[] indices = {
			0, 2, 1,  1, 2, 3,
			4, 5, 6,  5, 7, 6,
			8, 9,10,  9,11,10,
			12,14,13,  13,14,15,
			16,18,17,  17,18,19,
			20,21,22,  21,23,22,
		};

		protected override void DoRender() {
			if(!(lastRendered is CubeRenderer)) {
				GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length*sizeof(float),vertices,BufferUsageHint.DynamicDraw);
				GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length*sizeof(uint),indices,BufferUsageHint.DynamicDraw);
			}
			GL.DrawElements(PrimitiveType.Triangles,indices.Length,DrawElementsType.UnsignedInt,0);
		}

	}
}
