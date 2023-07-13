using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CGTest {
	static class Input {

		public static GameWindow boundWindow { get; private set; }

		public static KeyboardState KeyboardState => boundWindow.KeyboardState;
		public static MouseState MouseState => boundWindow.MouseState;

		public static void Init(GameWindow window) {
			boundWindow=window;

		}
		public static void Update() { }

		public static bool GetKey(Keys key) {
			return boundWindow.IsFocused&&KeyboardState.IsKeyDown(key);
		}


	}
}
