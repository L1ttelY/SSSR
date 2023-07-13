using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;

namespace CGTest {
	public class Camera:Component {

		protected override int executionOrder => -1000;

		public Matrix4 ViewProjectionMatrix { get; protected set; }

		public float fov = 70;

		public override void Update(float deltaTime) {
			base.Update(deltaTime);

			Vector3 to= (new Vector4(1,0,0,1)*transform.LocalToWorldMatrix).Xyz;
			Vector3 up = (new Vector4(0,1,0,0)*transform.LocalToWorldMatrix).Xyz;
			ViewProjectionMatrix=Matrix4.LookAt(transform.WorldPosition,to,up);
			ViewProjectionMatrix*=Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov),1,0.01f,10000f);
			
		}

	}
}
