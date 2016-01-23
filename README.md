# City State System
While playing the ChainGame you can be in either buildmode or playmode. These two modes share a lot of the same functionality, so I wanted to make a good system that allowed us to reuse as much code as possible.

Something I really liked about this system was how the ripple worked. All I had to do was to start a coroutine and pass a function as parameter. Whenever a tile got selected by the ripple coroutine, the function given as parameter would then decide what happened to the tile, making a very reusable system.