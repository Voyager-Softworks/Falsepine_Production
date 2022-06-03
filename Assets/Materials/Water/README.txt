Thank you for buying my asset!
If you have any questions or problems please send me an email at piotrtplaystore@gmail.com

How to use:
-Grab or create material using Water Shader
-Apply the material to plane which should be water in your scene
-Make sure your water plane/mesh have Cast Shadows option set to Off



How to enable Screen Space Reflection (SSR) for your project and my water material:

To make SSR work with transparent materials we have to enable a couple of options in the project:
1. First go to project settings -> HDRP Default Settings -> Under Frame Settings select Lighting -> Enable Screen Space Reflections and suboption called Transparents
2. Go to your HDRP quality settings files (if you created the project with Unity wizard go to folder Settings) -> in each file HDRPHighQuality, HDRPLowQuality, etc under Lighting -> Reflections select 
Screen Space Reflections and suboption called Transparent
3. In scene hierarchy select your scene Volume (object where you set all parameters for postprocessing in your scene, you can have a couple of such objects in the scene) -> Click Add Override -> write Screen Space Reflections
set its settings to meet your needs (remember Minimum Smoothness have to be lower or equal of water material Smoothness value (in case of using prebuild water material use value of 0.9))

And finally we have to enable this option in the materials we want to use:
4. Finally select my water materials in folder Water Shader HDRP under section Surface Options and select option Receive SSR Transparent

