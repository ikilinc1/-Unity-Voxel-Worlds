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

![Screenshot (77)](https://user-images.githubusercontent.com/81098623/163708037-00b6f182-1c79-47a2-ad3a-79cb6eaeef66.png)
![Screenshot (78)](https://user-images.githubusercontent.com/81098623/163708039-3418d660-4b70-418e-b0cd-e84fede19440.png)


Used Packages (Unity Registry)  
 -Burst(1.4.11)    
 -Input System(1.2.1)  
 -2D Sprite(1.0.0)  
 -Post Proccessing(3.2.1)  

Unity version-->2020.3.26f1  
Current proggress --> Finished.
