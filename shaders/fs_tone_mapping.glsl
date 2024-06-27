#version 330 core

in vec2 uv;
uniform sampler2D hdrImage;

out vec4 outputColor;

void main()
{
    vec3 hdrColor = texture(hdrImage, uv).rgb;

    // Tone-mapping using Reinhard operator
    vec3 result = hdrColor / (hdrColor + vec3(1.0));

    // Gamma correction
    result = pow(result, vec3(1.0 / 2.2));

    outputColor = vec4(result, 1.0);
}