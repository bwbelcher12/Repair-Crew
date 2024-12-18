REPAIR CREW
Game Design Document 
Frozen Rock Studios
August 30, 2024

 
Introduction
This document is intended to serve as the development guide for the Repair Crew team. It will outline the game’s overview, gameplay, mechanics, visual and audio style, story, world, and characters.
What is Repair Crew? It is a comical task-based multiplayer horror game similar to Lethal Company and Among Us. Technicians will be tasked with repairing several substations in a derelict, procedurally generated facility. The Technicians will not be alone, however. Alien creatures, murderous robots, and eldritch horrors will stalk the Technicians to ensure the station does not become operational. Quick thinking and strategic planning will be required to ensure the facility is ready for processing by CORPORATE’s Asset Reacquisition Team.
Game Overview
Concept
In Repair Crew, the player works for CORPORATE, a faceless, industrial, space-faring mega-corporation. The player is a member of CORPORATE’s Repair Crew, a brave group of Technicians tasked with infiltrating comprised CORPORATE facilities. They must repair subsystems and create a foothold with which the Asset Reacquisition Team (A.R.T.) can enter the facility and remove any unauthorized inhabitants. 
In each round, Technicians will be tasked with reaching the facility’s control room and broadcasting the clear-to-board signal. Before transmitting anything, they must restore systems like power generators, life support, and radio communications. While doing so, they must avoid the enemies that compromised the facility. The Technicians will have a set number of lives. If those lives are depleted, the mission is unsuccessful, and the asset will be deemed too dangerous for retrieval.
Players should feel a sense of horror as they explore the dark passageways when they first enter the facility. As they repair systems, diegetic feedback will include working lights, doors, weapon systems, and cameras. This will provide the player with both a sense of progress and safety. However, if they are too slow to signal for A.R.T., the enemies aboard could damage the systems again, leading to a sense of dread as the player must now traverse what was once a previously safe route.
 
Audience
This game is intended for players of all ages, but mostly young adults. The success of Lethal Company shows that there is an audience for this style of game. While it is intended to be a party game and played with friends, it will also be playable solo. This should also gain interest from speed runners as they attempt to clear levels/systems as quickly as possible. Players will buy Repair Crew because it is an innovative but faithful take on a proven formula.

Genre
Repair Crew is a comical task-based multiplayer horror game.

Setting
Repair Crew takes place in a corporatist future on the wild frontiers of explored space.

World Structure
The player will be exploring procedurally generated maze-like facilities. Each facility will have a set of rooms that must exist (control room, generator room, etc.) connected by various paths. Navigation will be difficult at first since the lack of power means no lights or automatic doors. Technicians will need to bring tools like flashlights and crowbars to get where they need to go.

Player
The players will play as the Technicians of CORPORATE’s Repair Crew. Through gameplay, players can specialize in roles they enjoy. Some players may enjoy repairing terminals while others enjoy scouting ahead and creating paths for their team.

Core Loop
Technicians will move from room to room exploring their surroundings and repairing facility substations. They will need to avoid enemies (either through stealth or brute force) as they move. Once they have repaired enough systems, they will be able to return to the control room and broadcast the clear-to-board signal.

Look & Feel
Repair Crew will feature a low-fidelity art style with minimal art and modeling. It will aim for an analog horror aesthetic with a futuristic grunge similar to the original Star Wars movies. A high-quality soundscape will be critical in maintaining the scariness of the game. All sounds should be diegetic. 
Gameplay
Objectives
The player’s primary objective in Repair Crew is to transmit the clear-to-board signal to A.R.T. from the facility’s control room. Before the signal can be sent, Technicians must complete a set of tasks that can be viewed both in the control room and in a mission briefing they receive at the start of the round. While all the systems remain down, enemies will have free range over the entire facility. As Technicians repair those systems, enemy movement will be restricted. Occasionally, enemies will venture into operational rooms and damage those systems again. This means the Technicians will need to be quick or double back and ensure all rooms are clear. 

Repairing
Each subsystem repair will be a minigame. Some could include:
•	Opening a breaker box and flipping the correct breakers
•	Reconnecting disconnected wires
•	Realigning a transmission dish
•	Spinning valves to achieve correct system pressure
•	Completing a pipe path to allow for fluid flow

Progression
The game will be broken up into “System Contracts.” Each system contract will contain a variable number of facilities that must be repaired in sequence (repairing the first facility unlocks the second, etc.). Successful repair of a facility will award the player CORPORATE CREDITS (CCs) which can be used to purchase equipment from the company store. Each completed subtask will earn the player a small amount of CCs. Players can attempt to perform more repairs than necessary to earn more CCs. As the player progresses through a contract, the facilities will become more dangerous and require more repairs. Successful completion of a contract will grant the player experience. Leveling up will unlock new titles and outfits for the player’s character. 

Play Flow
Players are expected to attempt contracts in their entirety. Any progression (CCs, purchased equipment) will be lost when a player terminates or completes a contract. 

Difficulty
The game's difficulty will be based on two factors. Each system will have a difficulty rating. Experienced players can choose to take contracts in more difficult systems while novice players can select contracts in easier systems. Difficulty will also increase as a contract progresses. The closer the player is to completing a contract, the more difficult the facilities will be to repair (more enemies, more subsystems, bigger maps, etc.).
Mechanics
Rules
Technicians must complete all necessary repairs and transmit the clear-to-board signal before either their timer runs out or they run out of lives. They are free to complete the objectives in whatever order they see fit. However, some objectives may be reliant on others having already been repaired (the generators will almost always need to be fixed first, for instance).

Game Universe
Enemy AI will have different behaviors depending on their type. Some will chase the player relentlessly, some will patrol the facility, some will attempt to sneak up on the player, while others will try to sabotage repairs that have been made. Only some enemies will pose a direct threat to the player, but all will try to hinder their progress in some way. 

Physics
The game will feature very light physics. Player/Enemy ragdolls and weapon projectiles are the only physics features. 

Economy
The game will have two economies: CCs and EXP. CCs are earned by the player for completing tasks. They can be used to purchase items to make subsequent missions easier. Once earned, CCs can only be lost via purchases or when the contract is completed. Each completed contract will award an amount of EXP based on the contract’s difficulty in addition to a portion of unspent CCs.

Character Movement
The character controller will be very simple. It will feature walking, sprinting, crouching, and jumping. The player will need to utilize all of the movement options, deciding when to sacrifice stealth for speed and vice versa. 

Player Interaction
Players will have limited interactions with the game world. Doors, switches, terminals, and repair points will be the main interactable objects. Players will also be able to purchase and bring items with them like motion sensors, deployable cameras, and weapons. Lucky Technicians may find some items that the deceased crew of the facility left behind.
Game menus will be very minimal. The main menu will have the following:
•	Play
•	Join
•	Host
•	Settings
•	Quit
Settings will be available during gameplay, as well as an option to leave the current lobby. The game will feature an always visible toolbelt-style inventory; thus no additional screen will be needed. There will be a shop interface where Technicians can spend their CCs. Purchased items will then be instantly created by a matter reassembler in the Technician’s boarding pod.
Saving will not be possible mid-mission. Upon mission completion, the game will automatically save contract progress. If a contract is terminated, it cannot be restarted from a previous save point.

Assets
The game will need models for the player, all enemy types, all items, and all interior components. This will include rooms, their decorations, and all interactable objects. All of these models will need texturing. Lots of sounds will be needed as well. Almost all objects should produce a sound of some kind. Sound and lighting will be the main source of atmosphere for the game.

Items
Technicians will have access to a range of items that will assist them in their efforts to repair the facility. These items will include:
•	Flashlight
o	Used to illuminate the area in front of the Technician in a cone of light.
o	Will trigger illumination responses from enemies.
•	Remote Camera
o	Used to remotely view the location the camera was placed in.
•	Motion Sensor
o	Used to alert the Technician of movement near the sensor.
o	Will set off an alarm that can be heard by nearby Technicians and enemies.
o	Enemies will react to the sound.
•	Door Jam
o	Used to prevent doors from opening (or closing)
•	Stun Gun
o	Used to temporarily stun an enemy.
o	Must recharge between shots.
•	Pistol
o	Handgun with limited ammunition.
o	Can be used to neutralize enemies.
•	Radio
o	Used to communicate with other technicians.

•	Crowbar
o	Used to open doors that are stuck shut.
o	Takes time
Graphics & Audio
Visual System
The game will be highly stylized with shaders to reduce the importance of high-quality modeling/texturing. It will be a first-person 3D game.

Interface
The game will feature a minimal interface simply communicating the most important information. Held items and health/stamina are all that need to be shown.

Audio System
A high-quality audio system will be critical for instilling fear in the player. Ambiance will be critical so all sounds should be diegetic. In game, there will be no out-of-universe sounds or music. A title theme will be made for the main menu. The game will feature short-range proximity voice chat as well as radio communications. 

Lighting System
Lighting will also be very important. The player will need to interact with the environment to enable/disable lights. Enemies will respond to the light in different ways. They should be able to detect where a light is and if they are in it and respond accordingly. 

Story & Narrative
Main Plot
The story of Repair Crew will be very limited. CORPORATE is intentionally depicted as shadowy to add to the mystery. Why is the player working for them? What are the facilities for? Why do the enemies want to disable them? What does A.R.T. do when they arrive? These questions should not be explicitly answered.
Characters
Player Character
The player will play as a non-descript human in a stylized hazmat suit known as “Technicians”. Technicians will not speak.

Enemies
There will be multiple enemy types present in the game. The ones thought of so far are:
•	Circuit Muncher
o	A small, bug-like creature that does not pose a direct threat to Technicians.
o	It will remain in an idle state until it is illuminated. Once illuminated, it will target the repair node that restored power to those lights and attempt to disable it again.
o	Once it has disabled the lights, it will return to a new dark location and enter the idle state again. 
o	It will be vulnerable to attacks from Technicians and have a small health pool.
•	Robot Sentry
•	Floating Eyes
•	Stalker
•	Roller
•	Cryer
•	Screamer
•	Mimic

