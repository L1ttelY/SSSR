using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace CGTest {
	public class Material {

	protected static int vertexArrayObject;

	static Material(){ 
			vertexArrayObject=GL.GenVertexArray();
			GL.BindVertexArray(vertexArrayObject);
			GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,14*sizeof(float),0);
			GL.VertexAttribPointer(1,2,VertexAttribPointerType.Float,false,14*sizeof(float),3*sizeof(float));
			GL.VertexAttribPointer(2,3,VertexAttribPointerType.Float,false,14*sizeof(float),5*sizeof(float));
			GL.VertexAttribPointer(3,3,VertexAttribPointerType.Float,false,14*sizeof(float),8*sizeof(float));
			GL.VertexAttribPointer(4,3,VertexAttribPointerType.Float,false,14*sizeof(float),11*sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.EnableVertexAttribArray(3);
			GL.EnableVertexAttribArray(4);
		}

		protected Material(Shader shader) {
			this.shader=shader;
		}

		public Shader shader { get; private set; }

		public virtual void Use() {
			shader.Use();
		}

		

	}
}
