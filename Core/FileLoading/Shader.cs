using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CGTest {
	public class Shader:IDisposable {

		public int Handle { get; private set; }
		int vertexShaderPosition;
		int fragmentShaderPosition;



		public Shader(string vertexPath,string fragmentPath) {

			string vertexShaderSource = File.ReadAllText(vertexPath);
			string fragmentShaderSource = File.ReadAllText(fragmentPath);
			vertexShaderPosition=GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShaderPosition,vertexShaderSource);
			fragmentShaderPosition=GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShaderPosition,fragmentShaderSource);

			GL.CompileShader(vertexShaderPosition);
			int success;
			GL.GetShader(vertexShaderPosition,ShaderParameter.CompileStatus,out success);
			if(success==0) {
				string infoLog = GL.GetShaderInfoLog(vertexShaderPosition);
				Console.WriteLine(infoLog);
			}
			GL.CompileShader(fragmentShaderPosition);
			GL.GetShader(fragmentShaderPosition,ShaderParameter.CompileStatus,out success);
			if(success==0) {
				string infoLog = GL.GetShaderInfoLog(fragmentShaderPosition);
				Console.WriteLine(infoLog);
			}

			Handle=GL.CreateProgram();
			GL.AttachShader(Handle,vertexShaderPosition);
			GL.AttachShader(Handle,fragmentShaderPosition);
			GL.LinkProgram(Handle);
			GL.GetProgram(Handle,GetProgramParameterName.LinkStatus,out success);
			if(success==0) {
				string infoLog = GL.GetProgramInfoLog(Handle);
				Console.WriteLine(infoLog);
			}

			GL.DetachShader(Handle,vertexShaderPosition);
			GL.DetachShader(Handle,fragmentShaderPosition);
			GL.DeleteShader(vertexShaderPosition);
			GL.DeleteShader(fragmentShaderPosition);

		}

		public void Use() {
			shaders.Add(this);
			GL.UseProgram(Handle);
		}

		private bool disposedValue = false;

		public static readonly HashSet<Shader> shaders = new HashSet<Shader>();

		protected virtual void Dispose(bool disposing) {
			if(shaders.Contains(this)) shaders.Remove(this);
			if(!disposedValue) {
				GL.DeleteProgram(Handle);
				disposedValue=true;
			}
		}
		~Shader() {
			if(disposedValue==false) {
				Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SetMatrix4(string name,Matrix4 value) {
			GL.UniformMatrix4(GL.GetUniformLocation(Handle,name),false,ref value);
		}
		public void SetInt(string name,int value) {
			GL.Uniform1(GL.GetUniformLocation(Handle,name),value);
		}
		public void SetFloat(string name,float value) {
			GL.Uniform1(GL.GetUniformLocation(Handle,name),value);
		}
		public void SetVec4(string name,Vector4 value) {
			GL.Uniform4(GL.GetUniformLocation(Handle,name),ref value);
		}
		public void SetVec3(string name,Vector3 value) {
			GL.Uniform3(GL.GetUniformLocation(Handle,name),ref value);
		}
		public void SetVec2(string name,Vector2 value) {
			GL.Uniform2(GL.GetUniformLocation(Handle,name),ref value);
		}

		public static void SetMvp(Matrix4 mvp,Matrix4 model,Matrix4 vp) {
			Matrix4 model_normal = Matrix4.Transpose(Matrix4.Invert(model));
			foreach(var shader in shaders){
				GL.ProgramUniformMatrix4(shader.Handle,GL.GetUniformLocation(shader.Handle,"mvp"),false,ref mvp);
				GL.ProgramUniformMatrix4(shader.Handle,GL.GetUniformLocation(shader.Handle,"vp"),false,ref vp);
				GL.ProgramUniformMatrix4(shader.Handle,GL.GetUniformLocation(shader.Handle,"model"),false,ref model);
				GL.ProgramUniformMatrix4(shader.Handle,GL.GetUniformLocation(shader.Handle,"model_normal"),false,ref model_normal);
			}
		}
	}
}
