using System;
using System.Collections.Generic;
using System.Text;

namespace CGTest {
	public class MeshData {
		public float[] vertices;
		public List<MeshElement> elements;
	}

	public class MeshElement{
		public uint[] indices;
		public Material material;
	}

}
