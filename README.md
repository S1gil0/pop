<img src="https://github.com/S1gil0/pop/blob/main/pop.png">

/pop command for oxide rust server, it shows how many players are online, connecting and queued to enter the server.

How to use:

Put the file Pop.cs in your oxide/plugins folder

Add the following permission to the players or groups that are going to be able to use the command:

pop.use

Write in game chat: /pop (you can change the command via the config file)

It will return the message to you (not broadcasted to the server, only to the player that issued the command).

Example:

Players Online: 189/200 | Joining: 11 | Queued: 24

Localization: Copy the oxide/lang/en folder and rename it to a new language (like de for german), edit the file Pop.json inside that folder overwriting with text in the new language.

Hope you enjoy!
