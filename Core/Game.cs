using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using CGTest.Materials;
using CGTest.Components;

namespace CGTest {
	public class Game:GameWindow {

		public static Game instance;

		public int VertexBufferObject { get; private set; }
		public int ElementBufferObject { get; private set; }
		public Matrix4 ViewProjectionMatrix { get; private set; }

		public Game(int width,int height,string title) :
		base(
			GameWindowSettings.Default,new NativeWindowSettings() { Size=(width, height),Title=title }
		) {
			instance=this;
		}

		protected override void OnUpdateFrame(FrameEventArgs args) {
			base.OnUpdateFrame(args);
			KeyboardState input = KeyboardState;

			if(input.IsKeyDown(Keys.Escape)) {
				Close();
			}
		}

		Transform root = new Transform();
		Transform camera;

		Texture texture0;
		Texture texture1;

		public Camera currentCamera{ get; private set; }

		protected override void OnLoad() {
			base.OnLoad();

			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,(int)TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,(int)TextureMagFilter.Linear);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);

			GL.ClearColor(1,1,1,1);
			Input.Init(this);

			VertexBufferObject=GL.GenBuffer();
			ElementBufferObject=GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer,VertexBufferObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,ElementBufferObject);

			OnInitScene();

		}


		protected override void OnUnload() {
			base.OnUnload();
			List<Shader> shaders=new List<Shader>();
			foreach(Shader shader in Shader.shaders) shaders.Add(shader);
			for(int i=0;i<shaders.Count;i++)shaders[i].Dispose();
		}

		Queue<Camera> cameraBuffer = new Queue<Camera>();

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit);
			GL.ClearColor(0,0,0.3f,1);
			GL.BindBuffer(BufferTarget.ArrayBuffer,VertexBufferObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,ElementBufferObject);

			foreach(var i in Component.updateList) {
				i.Update((float)e.Time);
				if(i is Camera) cameraBuffer.Enqueue(i as Camera);
			}
			while(cameraBuffer.Count!=0) {
				currentCamera = cameraBuffer.Dequeue();
				foreach(var shader in Shader.shaders) {
					shader.SetVec3("cameraPosition",currentCamera.transform.WorldPosition);
				}
				ViewProjectionMatrix=currentCamera.ViewProjectionMatrix;
				Light.UpdateLights();
				foreach(var i in Renderer.renderList) i.Render();
			}

			SwapBuffers();

			//Console.WriteLine(camera.WorldPosition);

		}

		protected override void OnResize(ResizeEventArgs e) {
			base.OnResize(e);
			GL.Viewport(0,0,e.Width,e.Height);
		}

		void OnInitScene(){

			texture0=new Texture(FileUtils.AssetPath+"Textures/grass.jpg");
			texture1=new Texture(FileUtils.AssetPath+"Models/Cottage2/Cottage_Clean_Normal.png");
			MaterialDefault materialDefault = new MaterialDefault();
			//materialDefault.normalMap=texture1;
			materialDefault.diffuseMap=texture0;
			materialDefault.ambientMap=texture0;
			materialDefault.colorAmbient=new Vector3(1,1,1);
			materialDefault.colorDiffuse=new Vector3(1,1,1);
			materialDefault.shininess=25;
			materialDefault.colorSpecular=new Vector3(0.7f,0.7f,0.7f);

			Texture textureSkybox = new Texture(FileUtils.AssetPath+"Textures/skybox.jpg");
			MaterialUnlitSkybox materialSkybox = new MaterialUnlitSkybox();
			materialSkybox.texture=textureSkybox;
			SkyboxRenderer skybox = new SkyboxRenderer();
			skybox.material=materialSkybox;
			root.AddComponent(skybox);

			Light ambientLight = new Light();
			ambientLight.color=new Vector3(0.1f,0.1f,0.1f);
			ambientLight.mode=Light.LightMode.Ambient;
			root.AddComponent(ambientLight);

			Transform cameraAround = new Transform();
			cameraAround.parent=root;
			cameraAround.AddComponent(new CameraMovement());

			Transform camera = new Transform();
			this.camera=camera;
			camera.parent=cameraAround;
			camera.LocalPosition=new Vector3(-2.5f,0,0);
			camera.AddComponent(new Camera());
			Light spotLight = new Light();
			spotLight.mode=Light.LightMode.Point;
			spotLight.color=new Vector3(1,1,1);
			spotLight.cutoff=0.05f;
			spotLight.falloffPolynomial=new Vector3(-0.5f,0.3f,0);
			camera.AddComponent(spotLight);

			Transform cube = new Transform();
			cube.parent=root;
			CubeRenderer cubeRenderer = new CubeRenderer();
			cubeRenderer.material=materialDefault;
			cube.AddComponent(cubeRenderer);
			cube.LocalRotationEuler=new Vector3(0,0,-1);
			cube.LocalPosition=new Vector3(0,-20,0);
			Light directionalLight = new Light();
			directionalLight.mode=Light.LightMode.Directional;
			directionalLight.color=new Vector3(0.4f,0.4f,0.4f);
			cube.AddComponent(directionalLight);

			Transform mesh = new Transform();
			MeshData meshData = ObjFile.ReadFile(FileUtils.AssetPath+@"Models/Cottage2/Cottage_FREE.obj");
			mesh.parent=root;
			mesh.LocalPosition=Vector3.One*3;
			MeshRenderer meshRenderer = new MeshRenderer();
			meshRenderer.meshData=meshData;
			mesh.AddComponent(meshRenderer);
			mesh.LocalPosition=new Vector3(0,-0.125f,15);

			Transform mesh2 = new Transform();
			meshData=ObjFile.ReadFile(FileUtils.AssetPath+@"Models/Cottage/cottage_obj.obj");
			mesh2.parent=root;
			mesh2.LocalPosition=Vector3.One*3;
			MeshRenderer meshRenderer2 = new MeshRenderer();
			meshRenderer2.meshData=meshData;
			mesh2.AddComponent(meshRenderer2);
			mesh2.LocalPosition=new Vector3(50,-0.1f,0);

			Transform terrain = new Transform();
			TerrainRenderer terrainRenderer = new TerrainRenderer(100,100);
			terrainRenderer.material=materialDefault;
			terrainRenderer.RiseTerrain(10,new Vector2(-25,-25),10,20);
			terrainRenderer.RiseTerrain(20,new Vector2(-10,-25),5,30);
			terrainRenderer.RiseTerrain(30,new Vector2(10,25),10,15);
			//terrainRenderer.UpdateNormals();
			terrain.AddComponent(terrainRenderer);
			terrain.parent=root;
			terrain.LocalPosition=new Vector3(0,0.12f,0);
			terrain.LocalScale=new Vector3(200,1,200);

		}

	}
}