#version 330 core

in vec2 uv;
uniform sampler2D pixels;
uniform sampler3D colorLUT;
uniform float lutSize;

out vec4 outputColor;

vec3 applyLUT(vec3 color, sampler3D lut, float size) {
    color = clamp(color, 0.0, 1.0);
    float sliceSize = 1.0 / size;
    float slicePixelSize = sliceSize / size;
    float halfPixelOffset = slicePixelSize / 2.0;

    color = color * (size - 1.0) / size + halfPixelOffset;

    vec3 slicePos = color * size;
    vec3 sliceIndex = floor(slicePos);
    vec3 sliceWeight = fract(slicePos);

    vec3 slice0 = sliceIndex / size;
    vec3 slice1 = (sliceIndex + vec3(1.0)) / size;

    vec3 result = mix(
        mix(
            mix(texture(lut, vec3(slice0.x, slice0.y, slice0.z)).rgb, texture(lut, vec3(slice1.x, slice0.y, slice0.z)).rgb, sliceWeight.x),
            mix(texture(lut, vec3(slice0.x, slice1.y, slice0.z)).rgb, texture(lut, vec3(slice1.x, slice1.y, slice0.z)).rgb, sliceWeight.x),
            sliceWeight.y),
        mix(
            mix(texture(lut, vec3(slice0.x, slice0.y, slice1.z)).rgb, texture(lut, vec3(slice1.x, slice0.y, slice1.z)).rgb, sliceWeight.x),
            mix(texture(lut, vec3(slice0.x, slice1.y, slice1.z)).rgb, texture(lut, vec3(slice1.x, slice1.y, slice1.z)).rgb, sliceWeight.x),
            sliceWeight.y),
        sliceWeight.z);

    return result;
}

void main()
{
    vec3 color = texture(pixels, uv).rgb;
    color = applyLUT(color, colorLUT, lutSize);
    outputColor = vec4(color, 1.0);
}