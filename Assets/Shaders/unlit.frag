#version 330 core

out vec4 FragColor;

in vec2 uv;
in vec3 normal;
in vec3 position;
in vec3 tangent;
in vec3 bitangent;

uniform sampler2D mainTexture;

struct Light{
	vec3 position;
	vec3 direction;
	vec3 color;

	//corrospond to dot value,  0 mean no light, 2 mean all direction light
	float cutoff;
	vec3 falloffPolynomial;
};

uniform vec3 ambientLight;
uniform vec3 cameraPosition;
uniform Light[10]lights;

void main(){
	FragColor=vec4(texture(mainTexture,uv).xyz,1);
}
