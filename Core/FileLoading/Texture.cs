﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace CGTest {
	public class Texture {

		public int Handle { get; private set; }

		public Texture(string path,TextureUnit unit=TextureUnit.Texture0){
			Handle=GL.GenTexture();
			StbImage.stbi_set_flip_vertically_on_load(1);
			ImageResult image = ImageResult.FromStream(File.OpenRead(path),ColorComponents.RedGreenBlueAlpha);
			Use(unit);
			GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,image.Width,image.Height,0,PixelFormat.Rgba,PixelType.UnsignedByte,image.Data);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

		}

		public void Use(TextureUnit unit = TextureUnit.Texture0) {
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D,Handle);
		}

	}
}
