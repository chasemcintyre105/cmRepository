Assignment 5: AI

COMP 437: Computer Game Development Spring 2018

Part 1 (20 points): Get an animated character model from the asset store (or make your own).

Part 2 (25 points): Implement a behavior script which dictates the characters movement.

Part 3 (25 points): Integrate a character controller into this behavior setup; ensure that the character transitions smoothly between animations and that animations match actions (for instance, a walk or run animation should be active if the character is moving).

Part 4 (30 points): Add at least three features to this basic setup. For example:

• create a trigger setup to let the character hit your player (hint: add a trigger collider to a component of the character model).
• attach dialogue to the character; start this dialogue when the player approaches
• allow the player to hit the character
• regulate spawning, death and respawning

This is by no means an exhaustive list. Larger features can count for more than one of these three, and the goal should not be strictly to add three features so much as to create a complete interactive NPC (a basic combat-based enemy, an ally, a quest hub. . . ). If you implement waypoint-based A* pathfinding, this qualifies as all three features.
