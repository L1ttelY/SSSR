using System;
using System.IO;
using OpenTK.Mathematics;

namespace CGTest {

	class Program {
		static void Main(string[] args) {

			Game game = new Game(800,600,"CGTest");
			game.Run();

		}
	}

}