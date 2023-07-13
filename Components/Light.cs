using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace CGTest.Components {

	public class Light:Component {

		public enum LightMode {
			Ambient,
			Directional,
			Point,
			Spotlight,
		}

		static HashSet<Light> lights = new HashSet<Light>();

		public static void UpdateLights() {
			foreach(var i in Shader.shaders) i.SetVec3("ambientLight",Vector3.Zero);

			int index = 0;
			foreach(var i in lights) {
				switch(i.mode) {
				case LightMode.Ambient: i.UseLightAmbient(); break;
				case LightMode.Point: i.UseLightPoint(index); break;
				case LightMode.Directional: i.UseLightDirectional(index); break;
				case LightMode.Spotlight: i.UseLightSpotlight(index); break;
				}
				if(i.mode!=LightMode.Ambient) index++;
			}

			for(int i = index;i<10;i++) {
				foreach(var shader in Shader.shaders) {
					shader.SetVec3($"lights[{i}].color",Vector3.Zero);
				}
			}

		}

		public LightMode mode;
		public Vector3 color;
		public Vector3 falloffPolynomial;
		public float cutoff;

		protected override void Enable() {
			base.Enable();
			lights.Add(this);
		}
		protected override void Disable() {
			base.Disable();
			lights.Remove(this);
		}

		static float[] buffer3 = new float[3];
		protected void UseLightAmbient() {
			foreach(Shader shader in Shader.shaders) {
				int ambientPosition = GL.GetUniformLocation(shader.Handle,"ambientLight");
				Vector3 ambientLight = Vector3.Zero;
				GL.GetUniform(shader.Handle,ambientPosition,buffer3);
				ambientLight.X=buffer3[0];
				ambientLight.Y=buffer3[1];
				ambientLight.Z=buffer3[2];
				ambientLight+=color;
				shader.SetVec3("ambientLight",ambientLight);
			}
		}

		protected void UseLightPoint(int target) {
			foreach(Shader shader in Shader.shaders) {
				shader.SetVec3($"lights[{target}].position",transform.WorldPosition);
				shader.SetVec3($"lights[{target}].direction",transform.Forward.Normalized());
				shader.SetVec3($"lights[{target}].color",color);
				shader.SetVec3($"lights[{target}].falloffPolynomial",falloffPolynomial);
				shader.SetFloat($"lights[{target}].cutoff",2);
			}
		}

		protected void UseLightDirectional(int target) {
			foreach(Shader shader in Shader.shaders) {
				shader.SetVec3($"lights[{target}].position",-transform.Forward*10000000f);
				shader.SetVec3($"lights[{target}].direction",transform.Forward.Normalized());
				shader.SetVec3($"lights[{target}].color",color);
				shader.SetVec3($"lights[{target}].falloffPolynomial",new Vector3(0,0,0));
				shader.SetFloat($"lights[{target}].cutoff",2);
			}
		}
		protected void UseLightSpotlight(int target) {
			foreach(Shader shader in Shader.shaders) {
				shader.SetVec3($"lights[{target}].position",transform.WorldPosition);
				shader.SetVec3($"lights[{target}].direction",transform.Forward.Normalized());
				shader.SetVec3($"lights[{target}].color",color);
				shader.SetVec3($"lights[{target}].falloffPolynomial",falloffPolynomial);
				shader.SetFloat($"lights[{target}].cutoff",cutoff);
			}
		}

	}
}
