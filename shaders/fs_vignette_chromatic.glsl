#version 330 core

in vec2 uv;
uniform sampler2D screenTexture;
uniform vec2 screenSize;

out vec4 outputColor;

void main()
{
    // Chromatic aberration effect
    float strength = 0.0005; // Adjust this value to reduce the effect
    vec2 uvRed = uv + vec2(strength, 0.0);
    vec2 uvGreen = uv;
    vec2 uvBlue = uv - vec2(strength, 0.0);

    vec3 color;
    color.r = texture(screenTexture, uvRed).r;
    color.g = texture(screenTexture, uvGreen).g;
    color.b = texture(screenTexture, uvBlue).b;

    // Vignette effect
    float radius = 0.75; // Adjust this value for vignette radius
    float softness = 0.45; // Adjust this value for vignette softness
    vec2 position = uv * 2.0 - 1.0;
    float vignette = smoothstep(radius, radius - softness, dot(position, position));

    color = mix(color, color * vignette, 0.5);

    outputColor = vec4(color, 1.0);
}