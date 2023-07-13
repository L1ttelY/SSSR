using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace CGTest.Components {
	public class Renderer:Component {

		protected virtual int sortingOrder => 0;

		public static SortedSet<Renderer> renderList;
		static Renderer() {
			renderList=new SortedSet<Renderer>(Comparer<Renderer>.Create((x,y) => {
				int res = x.sortingOrder-y.sortingOrder;
				if(res!=0) return res;
				return x.CompareTo(y);
			}
			));
		}

		protected override void Enable() {
			base.Enable();
			renderList.Add(this);
		}
		protected override void Disable() {
			base.Disable();
			renderList.Remove(this);
		}

		protected static Renderer lastRendered { get; private set; }

		public Material material;

		public void Render() {
			Matrix4 mvp = transform.LocalToWorldMatrix*Game.instance.ViewProjectionMatrix;
			Shader.SetMvp(mvp,transform.LocalToWorldMatrix,Game.instance.ViewProjectionMatrix);
			if(material!=null){
				material.Use();
				//if(lastRendered!=null&&lastRendered.material!=material) material.Use();
			}
			DoRender();
			lastRendered=this;
		}

		public override void Update(float deltaTime) {
			base.Update(deltaTime);
			lastRendered=null;
		}

		protected virtual void DoRender() { }

	}
}
