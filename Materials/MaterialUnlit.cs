using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace CGTest.Materials {
	class MaterialUnlit:Material {

		static Shader defaultShader;
		static Texture defaultTexture;

		public Texture texture;

		static MaterialUnlit() {
			defaultShader=new Shader(FileUtils.AssetPath+"Shaders/unlit.vert",FileUtils.AssetPath+"Shaders/unlit.frag");
			defaultTexture=new Texture(FileUtils.AssetPath+"Textures/white.png");
		}

		public MaterialUnlit():base(defaultShader) {
			texture=defaultTexture;
		}

		public override void Use() {
			base.Use();

			texture.Use(TextureUnit.Texture0);
			shader.SetInt("mainTexture",0);

			GL.BindVertexArray(vertexArrayObject);
		}

	}
}
