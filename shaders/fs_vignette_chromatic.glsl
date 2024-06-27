#version 330 core

in vec2 uv;
uniform sampler2D pixels;
uniform vec2 screenSize;

out vec4 outputColor;

void main()
{
    vec2 centeredUV = uv - 0.5;
    float dist = length(centeredUV);
    
    // Vignetting effect
    float vignette = smoothstep(0.7, 0.9, dist);

    // Chromatic aberration
    float chromaOffset = 0.01 * dist;
    vec3 color;
    color.r = texture(pixels, uv + vec2(chromaOffset, 0.0)).r;
    color.g = texture(pixels, uv).g;
    color.b = texture(pixels, uv - vec2(chromaOffset, 0.0)).b;

    // Apply vignetting
    color *= (1.0 - vignette);

    outputColor = vec4(color, 1.0);
}