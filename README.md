# Graphics-Practical-2

## Team members:  
- Simon de Jong 8562024  
- Yoeri Hoebe 4769341 

### Formalities:   
[X]  This readme.txt   
[ \]  Cleaned (no obj/bin folders)   
[ \]  Demonstration scene(s) with all implemented features     
[ \] Screenshots: make it clear which feature is demonstrated in which screenshot   
  
### Minimum requirements implemented:  
[X] Camera: position and orientation controls Controls: WASD for forward, left, backward and right. Spacebar for up and Left Ctrl for down. Scrollwheel can be used to zoom   
[X] Model matrix: for each mesh, stored aspart of the scene graph   
[X] Scene graph data structure: tree hierarchy, no limitation on breadth or depth or size  
[X] Rendering: recursive scene graph traversal, correct model matrix concatenation   
[X] Shading in fragment shader: diffuse, glossy, uniform variable for ambient light color  
[X] Point light: at least 1, position/color may be hardcoded  

### Bonus features implemented:  
[ \] Multiple point lights: at least 4, uniform variables to change position and color at runtime   
[ \] Spot lights: position, center direction, opening angle, color   
[ \] Environment mapping: cube or sphere mapping, used in background and/or reflections   
[ \] Frustum culling: in C# code, using scene graph node bounds, may be conservative   
[ \] Bump or normal mapping   
[ \] Shadow mapping: render depth map to texture, only hard shadows required, some artifacts allowed   
[X] Vignetting and chromatic aberrations: darker corners, color channels separated more near corners   
[X] Color grading: color cube lookup table   
[X] Blur: separate horizontal and vertical blur passes, variable blur size   
[X] HDR glow: HDR render target, blur in HDR, tone-mapping   
[ \] Depth of field: blur size based on distance from camera, some artifacts allowed   
[ \] Ambient occlusion: darker in tight corners, implemented as screen-space post process   
[ \] \...  

## Notes:
###Added Features:  
####Vignetting and Chromatic Aberration  
Description: This feature adds a vignette effect, making the corners of the screen darker, and a chromatic aberration effect, which separates the color channels near the corners.  
  
Implementation:  
Shader: Created a new fragment shader fs_vignette_chromatic.glsl.  
Shader Code:  
Applied chromatic aberration by shifting the UV coordinates for red, green, and blue channels.  
Implemented a vignette effect using a smoothstep function based on distance from the center.  
Integration: Updated the rendering pipeline in MyApplication.cs to use this shader during the post-processing stage.  
  
####Color Grading  
Description: This feature applies color grading to the rendered image using a 3D color look-up table (LUT) for advanced color manipulation.  
  
Implementation:  
LUT Generation: Created a method GenerateIdentityLUT to generate an identity LUT, which maps each color to itself.  
LUT Loading: Created a method LoadLUTTexture to load the generated LUT into a 3D texture.  
Shader: Created a new fragment shader fs_color_grading.glsl to apply the LUT.  
Shader Code:  
Sampled the LUT using the RGB values of each pixel to adjust the color.  
Integration: Updated the rendering pipeline in MyApplication.cs to use this shader during the post-processing stage.  
  
####Blur  
Description: This feature applies a blur effect to the rendered image using separate horizontal and vertical blur passes, allowing for variable blur sizes.  
  
Implementation:  
Shaders: Created fragment shaders fs_blur_horizontal.glsl and fs_blur_vertical.glsl for horizontal and vertical blur passes.  
Shader Code:  
Horizontal blur shader samples and averages the neighboring pixels horizontally.  
Vertical blur shader samples and averages the neighboring pixels vertically.  
Integration: Updated the rendering pipeline in MyApplication.cs to use these shaders for sequential blur passes.  
  
####HDR Glow  
Description: This feature adds a glow effect by using an HDR render target, applying blur in HDR, and performing tone-mapping to convert HDR to LDR.  
  
Implementation:  
HDR Render Target: Created an HDR render target to capture the scene in high dynamic range.  
Blur in HDR: Applied the horizontal and vertical blur shaders to the HDR render target.  
Tone-Mapping Shader: Created a tone-mapping shader to convert the HDR image to LDR.  
Integration: Updated the rendering pipeline in MyApplication.cs to use these shaders and render targets to achieve the HDR glow effect.  
