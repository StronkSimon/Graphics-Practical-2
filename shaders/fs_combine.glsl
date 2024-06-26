#version 330 core

in vec2 uv;
uniform sampler2D originalImage;
uniform sampler2D blurredImage;

out vec4 outputColor;

void main()
{
    vec3 originalColor = texture(originalImage, uv).rgb;
    vec3 blurredColor = texture(blurredImage, uv).rgb;

    // Combine the original image with the blurred image
    vec3 result = originalColor + blurredColor * 0.5; // Adjust blend factor as needed

    outputColor = vec4(result, 1.0);
}