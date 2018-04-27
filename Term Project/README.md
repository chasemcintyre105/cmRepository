Project Proposal(5%):
Each project group must submit a single project proposal, which should convey the following information:
• who is in the group
• a description of your desired project
Project groups do not need to match assignment groups. The description may be detailed, but it does not need to be; “We plan to make a FPS” is a valid description. I would recommend, however, that you attempt to break the project into component parts and briefly plan how each part might be accomplished.

Who is in the group:
-	Paul Banyai
-	Binhwa Jeong
-	Chase McIntyre
-	James McLeod
-	David Swepson


A description of your desired project:

We plan on making a 3d chess game in Unity. We plan on creating/modeling/painting the chess pieces and board, animating movement of the chess pieces, and programming the game control software. These features are necessary for our minimum viable product.

We hope to allow for multiplayer, or program a simple ‘opponent’, so that the player does not have to control both sides of the board. We also hope to construct a UI homepage giving the player several capabilities (some basic settings, play game option, etc.). These are not necessary components of our minimum viable product,  and will be implemented as a secondary feature if our available resources allow for it.

Breakdown of essential (MVP) tasks:
We plan to model and paint the chess pieces in Blendr, and import them into Unity.
-	Model the basic pieces (DONE)
-	Paint them (more interesting color scheme than simply black/white?) (James)
-	Import into Unity 
We plan to animate the movement of these pieces in Unity.
-	Create a ‘lift-off’ animation, which drags the selected piece into the air high enough that it would not collide with any ‘grounded’ piece if moved. The piece is considered ‘airborne’ after this animation finishes.
-	Create a ‘set-down’ animation which lowers an ‘airborne’ piece down onto the chess board. The piece is considered ‘grounded’ after this animation finishes
-	Create a ‘move forward’ animation which will move an ‘airborne’ piece one square away from its home row
-	Create a ‘move backward’ animation which will move an ‘airborne’ piece one square towards its home row
-	Create a ‘move right’ animation which will move an ‘airborne’ piece one square to its right
-	Create a ‘move left’ animation which will move an ‘airborne’ piece one square to its left
We plan to model and paint the chess board in Blendr, and import it into Unity
-	Model the board (basic)  (DONE)
-	Customize the board (colors, environment around the board, physical aspects of the board)
-	Paint them (more interesting color scheme than simply black/white?)
-	Import into Unity
We plan to program the game control software in C#.
-	The controller must put the user in control of one set of pieces, giving them the ability to move one piece per turn
-	The user must have the ability to select a piece, and see possible movements for that piece.
-	The user must have the ability to choose a possibility, and execute the movement/animation that defines the move.
-	The controller must handle the effects of player movements appropriately
-	Execute the animation
-	Update appropriate information on movements
-	Handle taking/capturing of pieces appropriately
-	The controller must automate alternating turns
-	When a player executes a movement, control of the board/pieces must transfer to the other player
-	The controller must recognize significant events
-	A pawn reaching the opposite ‘home row’
-	A player being put into check
-	A player being checkmated
-	Current game must end, execute desired end of game functionality
-	New game must begin


