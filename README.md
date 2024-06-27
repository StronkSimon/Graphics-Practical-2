# Graphics-Practical-2

### Team members:  
- Simon de Jong 8562024  
- Yoeri Hoebe 4769341 

#### Formalities:   
[X]  This readme.txt   
[ \]  Cleaned (no obj/bin folders)   
[ \]  Demonstration scene(s) with all implemented features  
[ \] (Optional)   
  
Screenshots: make it clear which feature is demonstrated in which screenshot  
  
#### Minimum requirements implemented:  
[X] Camera: position and orientation controls Controls: WASD for forward, left, backward and right. Spacebar for up and Left Ctrl for down. Scrollwheel can be used to zoom   
[X] Model matrix: for each mesh, stored aspart of the scene graph   
[X] Scene graph data structure: tree hierarchy, no limitation on breadth or depth or size  
[X] Rendering: recursive scene graph traversal, correct model matrix concatenation   
[X] Shading in fragment shader: diffuse, glossy, uniform variable for ambient light color  
[X] Point light: at least 1, position/color may be hardcoded  

#### Bonus features implemented:  
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

##### Notes:  
