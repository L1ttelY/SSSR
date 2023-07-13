using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CGTest.Materials {
	public class MaterialDefault:Material {

		public Texture diffuseMap;
		public Texture ambientMap;
		public Texture specularMap;
		public Texture normalMap;

		public Vector3 colorDiffuse = Vector3.One;
		public Vector3 colorAmbient = Vector3.One;
		public Vector3 colorSpecular = Vector3.One;
		public float shininess;
		/*
		static MaterialDefault() {
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
		*/
		static Shader _shader;
		static Texture defaultTexture;
		static Texture defaultNormalMap;
		static Shader defaultShader {
			get {
				if(_shader!=null) return _shader;
				_shader=new Shader(FileUtils.AssetPath+"Shaders/default.vert",FileUtils.AssetPath+"Shaders/default.frag");
				defaultTexture=new Texture(FileUtils.AssetPath+"Textures/white.png");
				defaultNormalMap=new Texture(FileUtils.AssetPath+"Textures/defaultnormal.png");
				return _shader;
			}
		}

		public MaterialDefault() : base(defaultShader) {
			diffuseMap=defaultTexture;
			ambientMap=defaultTexture;
			normalMap=defaultNormalMap;
			specularMap=defaultTexture;
		}

		public override void Use() {
			base.Use();

			shader.SetVec3("material.diffuse",colorDiffuse);
			shader.SetVec3("material.ambient",colorAmbient);
			shader.SetVec3("material.specular",colorSpecular);
			shader.SetFloat("material.shininess",shininess);

			diffuseMap.Use(TextureUnit.Texture0);
			ambientMap.Use(TextureUnit.Texture1);
			specularMap.Use(TextureUnit.Texture2);
			normalMap.Use(TextureUnit.Texture3);
			shader.SetInt("diffuseMap",0);
			shader.SetInt("ambientMap",1);
			shader.SetInt("specularMap",2);
			shader.SetInt("normalMap",3);

			GL.BindVertexArray(vertexArrayObject);
		}


	}
}
