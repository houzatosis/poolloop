Assets for artists to edit:

Assets/Resources/CueBall.prefab
Assets/Resources/SolidBall.prefab
Assets/Resources/StripeBall.prefab

These assets are responsible for the visual aspects of the 3 different kinds of balls that will be used in the game. They are prefabs that should only consist of visual components like the meshcomponent or particle system components. The local position should stay all 0s and the local scale should stay all 1s.

Currently all these balls point to a prototype mesh in an FBX file. Feel free to add new FBX files and redirect these prefabs to point to your new mesh.

To make a pool table in your own scene and have the balls show up:

All you need to do to get the balls to show up is to create 2 empty objects, one to spawn the cue ball, and one to spawn the group of object balls. Attach the script "CueBallSpawner.cs" to spawn the cue ball, attach the script "ObjectBallGroupSpawner.cs" to spawn the triangle layout of object balls (striped and solid balls). Check out prototype.unity for an example of this. (Don't modify prototype.unity though please).

Gameplay assets:

Assets/Resources/Spawned/CueBall.prefab
Assets/Resources/Spawned/TriangleBallLayout.prefab

These assets are responsible for the physical and gameplay aspects of the 3 different kids of balls in the game. They include rigidbodies and collision volumes. Right now editing the size of balls involves changes to each of these assets, in the future this will happen automatically based on configuration values.