## UnityECSTestProject - Terrain Generator

This project draws inspiration from procedural generation systems used in massive terrains, much like those seen in video games such as No Man's Sky. It leverages an Octree structure to load distinct terrain sections with varying resolutions, effectively simulating Level of Detail (LOD) principles. Concurrently, it employs the Dual Contouring algorithm to generate meshes within these sections.

## Dual Contouring

The primary objective of this project is to create an editable massive terrain. I have chosen this approach over hight maps and other 2D simulators to grant me the ability to generate procedural caves and manipulate the terrain in all dimensions. To achieve this, this project utilizes a density function that defines the terrain's topography. Currently, I are using a 3D Perlin noise function for testing, which takes the position of a point in the world as input.

To transform this mathematical representation into a mesh, I employ a grid of points as inputs for the density function, generating values for these points. The algorithm then categorizes these values based on whether they exceed a defined threshold, distinguishing whether a point is in the air or on the ground.

[![Image from Gyazo](https://i.gyazo.com/a9878a47638fb7065ead657abdf48887.gif)](https://gyazo.com/a9878a47638fb7065ead657abdf48887)

These points can also be treated as cells formed by groups of 8 points. If all the points within a cell share the same classification (AIR or GROUND), they are discarded, as they do not contribute meaningful information. The resultant cells intersect with the final mesh in various ways, prompting the use of a QefSolver to approach a vertex as closely as possible to the plane that separates the air and the ground.

[![Image from Gyazo](https://i.gyazo.com/a9d0fb6ab7f5bfa36ec3085105ca24f1.gif)](https://gyazo.com/a9d0fb6ab7f5bfa36ec3085105ca24f1)

Finally, the last step involves triangulating all these vertices to form the mesh. I utilize the cell edges that intersect the air-ground boundary to connect neighboring cells and create quads.

[![Image from Gyazo](https://i.gyazo.com/0110a47e3ec2a27efa6d3f2cc037560d.gif)](https://gyazo.com/0110a47e3ec2a27efa6d3f2cc037560d)

## Octree

This data structure dynamically splits its branches based on the camera's distance. Larger leaf nodes or chunks are created farther away from the player, while smaller ones are generated closer to the player's view. Each level of this octree features a different resolution, similar to how LODs function, determining the grid dimensions for the input points used in Dual Contouring.

[![Image from Gyazo](https://i.gyazo.com/2e2da5ee7e7e2473fbc306866382d540.gif)](https://gyazo.com/2e2da5ee7e7e2473fbc306866382d540)

## Upcoming lines

[![Image from Gyazo](https://i.gyazo.com/5aa1898036f7be2b922908cd427500f7.png)](https://gyazo.com/5aa1898036f7be2b922908cd427500f7)

- Performance: While I can generate meshes for all the chunks within the octree, the current performance leaves room for improvement. At the moment, there are aspects of Unity DOTS that I'm still working on mastering, and I'm actively seeking to enhance my knowledge of this technology to address these performance issues effectively.

- Stitching Chunks: Each chunk can currently generate its mesh independently, without considering the neighboring chunks.

- Bevy: I'm interested in rewriting this code for Bevy to see what happens.