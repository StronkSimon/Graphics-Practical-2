#version 330 core

in vec2 uv;
uniform sampler2D pixels;

out vec4 outputColor;

void main()
{
    vec3 color = texture(pixels, uv).rgb;
    float brightness = dot(color, vec3(0.2126, 0.7152, 0.0722));
    if (brightness > 1.0) // threshold for brightness
        outputColor = vec4(color, 1.0);
    else
        outputColor = vec4(0.0);
}