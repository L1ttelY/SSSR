using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CGTest.Components {
	class MeshRenderer:Renderer {

		public MeshData meshData;

		protected override void DoRender() {
			base.DoRender();
			Matrix4 mvp = transform.LocalToWorldMatrix*Game.instance.ViewProjectionMatrix;

			GL.BufferData(BufferTarget.ArrayBuffer,meshData.vertices.Length*sizeof(float),meshData.vertices,BufferUsageHint.DynamicDraw);

			foreach(var i in meshData.elements) {
				i.material.Use();
				GL.BufferData(BufferTarget.ElementArrayBuffer,i.indices.Length*sizeof(uint),i.indices,BufferUsageHint.DynamicDraw);
				GL.DrawElements(PrimitiveType.Triangles,i.indices.Length,DrawElementsType.UnsignedInt,0);
			}

		}

	}
}
