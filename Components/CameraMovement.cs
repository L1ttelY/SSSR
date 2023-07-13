using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CGTest.Components {
	class CameraMovement:Component {

		float originalSpeed = 5;
		float rollSpeed = 4;
		float mouseSensitivity = 0.002f;

		public override void Update(float deltaTime) {

			KeyboardState keyboardState = Input.KeyboardState;

			Vector3 forward = (Vector4.UnitX*transform.LocalToParentMatrix).Xyz.Normalized();
			Vector3 up = (Vector4.UnitY*transform.LocalToParentMatrix).Xyz.Normalized();
			Vector3 right = (Vector4.UnitZ*transform.LocalToParentMatrix).Xyz.Normalized();

			float speed = originalSpeed;

			if(keyboardState.IsKeyDown(Keys.LeftControl)) speed*=3;

			if(keyboardState.IsKeyDown(Keys.W)) transform.LocalPosition+=forward*speed*deltaTime;
			if(keyboardState.IsKeyDown(Keys.S)) transform.LocalPosition-=forward*speed*deltaTime;
			if(keyboardState.IsKeyDown(Keys.D)) transform.LocalPosition+=right*speed*deltaTime;
			if(keyboardState.IsKeyDown(Keys.A)) transform.LocalPosition-=right*speed*deltaTime;
			if(keyboardState.IsKeyDown(Keys.Space)) transform.LocalPosition+=up*speed*deltaTime;
			if(keyboardState.IsKeyDown(Keys.LeftShift)) transform.LocalPosition-=up*speed*deltaTime;

			Vector4 newRotationEuler = Vector4.Zero;
			if(keyboardState.IsKeyDown(Keys.Q)) newRotationEuler[0]-=rollSpeed*deltaTime;
			if(keyboardState.IsKeyDown(Keys.E)) newRotationEuler[0]+=rollSpeed*deltaTime;

			MouseState mouseState = Input.MouseState;
			if(mouseState.IsButtonDown(0)) {
				newRotationEuler[1]-=mouseSensitivity*mouseState.Delta.X;
				newRotationEuler[2]-=mouseSensitivity*mouseState.Delta.Y;
			}

			//newRotationEuler=transform.LocalToWorldMatrix*newRotationEuler;

			transform.LocalRotation*=new Quaternion(newRotationEuler.Xyz);

		}

	}
}
