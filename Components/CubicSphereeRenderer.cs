using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
namespace CGTest.Components {
	class CubicSphereRenderer:Renderer {

		public static float[] vertices ={
			-0.5f,-0.5f,-0.5f, 0,0, -0.57735026f,-0.57735026f,-0.57735026f, 0,0,0,
			-0.5f, 0.5f,-0.5f, 0,1, -0.57735026f, 0.57735026f,-0.57735026f, 0,0,0,
			-0.5f,-0.5f, 0.5f, 1,0, -0.57735026f,-0.57735026f, 0.57735026f, 0,0,0,
			-0.5f, 0.5f, 0.5f, 1,1, -0.57735026f, 0.57735026f, 0.57735026f, 0,0,0,
			 0.5f,-0.5f,-0.5f, 1,0,  0.57735026f,-0.57735026f,-0.57735026f, 0,0,0,
			 0.5f, 0.5f,-0.5f, 1,1,  0.57735026f, 0.57735026f,-0.57735026f, 0,0,0,
			 0.5f,-0.5f, 0.5f, 0,0,  0.57735026f,-0.57735026f, 0.57735026f, 0,0,0,
			 0.5f, 0.5f, 0.5f, 0,1,  0.57735026f, 0.57735026f, 0.57735026f, 0,0,0,
		};

		public static uint[] indices = {
			0,2,1,  1,2,3,
			4,5,6,  5,7,6,
			1,3,5,  3,7,5,
			0,4,2,  2,4,6,
			0,1,4,  1,5,4,
			2,6,3,  3,6,7,
		};

		protected override void DoRender() {
			if(!(lastRendered is CubeRenderer)){
				GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length*sizeof(float),vertices,BufferUsageHint.DynamicDraw);
				GL.BufferData(BufferTarget.ElementArrayBuffer,indices.Length*sizeof(uint),indices,BufferUsageHint.DynamicDraw);
			}
			GL.DrawElements(PrimitiveType.Triangles,indices.Length,DrawElementsType.UnsignedInt,0);
		}

	}
}
