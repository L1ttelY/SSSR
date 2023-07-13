#version 330 core

out vec4 FragColor;

in vec2 uv;
in vec3 normal;
in vec3 position;
in vec3 tangent;
in vec3 bitangent;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;
uniform sampler2D ambientMap;
uniform sampler2D specularMap;

struct Material{
	vec3 diffuse;
	vec3 ambient;
	vec3 specular;
	float shininess;
};

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
uniform Material material;

void main(){

	vec3 normal=normalize(normal);

	vec3 colorAmbient=ambientLight;
	vec3 colorDiffusion=vec3(0,0,0);
	vec3 colorSpecular=vec3(0,0,0);

	vec3 idealLightDirection=normalize(reflect(cameraPosition-position,normal));

	vec3 bitangent=normalize(bitangent);
	vec3 tangent=normalize(tangent);
	mat3 tbn=mat3(tangent,bitangent,normal);

	vec3 normalColor=texture(normalMap,uv).xyz;
	normalColor*=2;
	normalColor-=vec3(1,1,1);
	normal=tbn*normalColor;
	
	for(int i=0;i<10;i++){
		Light light=lights[i];
		//light=lights[0];

		float lightDistance=distance(position,light.position);
		vec3 lightDirection=normalize(position-light.position);

		float lightIntensityRaw=1/
		max(1,
			light.falloffPolynomial[0]+
			light.falloffPolynomial[1]*lightDistance+
			light.falloffPolynomial[2]*lightDistance*lightDistance
		);
		
		lightIntensityRaw=dot(light.direction,lightDirection)>(1-light.cutoff)?lightIntensityRaw:0;
		
		colorDiffusion+=max(0,dot(-lightDirection,normal))*lightIntensityRaw*light.color;
		
		float specularPower=clamp(dot(idealLightDirection,lightDirection),0,1);
		colorSpecular+=pow(specularPower,max(material.shininess,2))*lightIntensityRaw*light.color;

	}

	FragColor=vec4(0,0,0,1);
	FragColor+=vec4(colorAmbient*material.ambient*texture(ambientMap,uv).xyz,0);
	FragColor+=vec4(colorDiffusion*material.diffuse*texture(diffuseMap,uv).xyz,0);
	FragColor+=vec4(colorSpecular*material.specular*texture(specularMap,uv).xyz,0);
	//FragColor=vec4((normal+vec3(1,1,1))/2,0);
}
