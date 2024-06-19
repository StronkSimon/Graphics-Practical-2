#version 330
 
// shader inputs
in vec4 positionWorld;              // fragment position in World Space
in vec4 normalWorld;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates

uniform sampler2D diffuseTexture;	// texture sampler

uniform vec3 ambientLightColor;
uniform vec3 cameraPosition;          

uniform vec3 lightPosition;
uniform vec3 lightColor;

// shader output
out vec4 outputColor;

void main()
{
	vec3 norm = normalize(normalWorld.xyz);
	
	vec3 camDirection = normalize(cameraPosition - positionWorld.xyz);

	vec3 lightDirection = normalize(lightPosition - positionWorld.xyz);

	float diff = max(0,dot(norm, lightDirection));
	vec3 diffuse = diff * lightColor;

	vec3 reflectDirection = reflect(-lightDirection, norm);
	float spec = pow(max(dot(camDirection,reflectDirection),0.0),32);
	vec3 specular = 0.5*spec*lightColor;

	vec3 result = diffuse + specular + ambientLightColor;

	outputColor = vec4(result * (texture(diffuseTexture,uv).rgb), 1.0);
}
