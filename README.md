# -Unity-Voxel-Worlds

Created a voxel world with following steps creating quad->block->chunk->world. Also used perlin grapher to create a layer system for block layer seperation.  
Added a cave system with 3D perlin graph and used these graph to plant trees. 
Implemented a save/load system and loading interface.  
Building with selected block and destroying block is possible (left click to destroy, right click to build with selected block)  
Tab key scrolls through blocks and save icon  
  
Used job manager in order to render chunk elements efficiently   
Used LineRenderer to represent perlingraph  
Used coroutine to be able to see chunks created  
Used parrallel jobs to calculate perlin (chunk and world building optimization)  
Save and load system uses binary system  

Used Packages (Unity Registry)  
 -Burst(1.4.11)    
 -Input System(1.2.1)  
 -2D Sprite(1.0.0)  
 -Post Proccessing(3.2.1)  

Unity version-->2020.3.26f1  
Current proggress --> Finished.
