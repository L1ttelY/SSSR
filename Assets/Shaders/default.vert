#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTextCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;
uniform mat4 mvp;
uniform mat4 model_normal;
uniform mat4 model;
uniform mat4 vp;

out vec2 uv;
out vec3 normal;
out vec3 position;
out vec3 tangent;
out vec3 bitangent;

void main(){
	gl_Position = mvp*vec4(aPosition,1.0);
	uv=aTextCoord;
	normal=mat3(model_normal)*aNormal;
	position=vec3(model*vec4(aPosition,1.0));
	tangent=vec3(model*vec4(aTangent,0));
	bitangent=vec3(model*vec4(aBitangent,0));
}