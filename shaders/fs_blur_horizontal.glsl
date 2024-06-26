#version 330 core

in vec2 uv;
uniform sampler2D image;
uniform float texelWidthOffset;

out vec4 outputColor;

void main()
{
    vec2 tex_offset = vec2(texelWidthOffset, 0.0); 
    vec3 result = vec3(0.0);

    result += texture(image, uv - 4.0 * tex_offset).rgb * 0.05;
    result += texture(image, uv - 3.0 * tex_offset).rgb * 0.09;
    result += texture(image, uv - 2.0 * tex_offset).rgb * 0.12;
    result += texture(image, uv - tex_offset).rgb * 0.15;
    result += texture(image, uv).rgb * 0.16;
    result += texture(image, uv + tex_offset).rgb * 0.15;
    result += texture(image, uv + 2.0 * tex_offset).rgb * 0.12;
    result += texture(image, uv + 3.0 * tex_offset).rgb * 0.09;
    result += texture(image, uv + 4.0 * tex_offset).rgb * 0.05;

    outputColor = vec4(result, 1.0);
}