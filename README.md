/pop command for oxide rust servers, it shows how many players are online and how many players are connecting to the server.

How to use:

Put the file Pop.cs in your oxide/plugins folder

Add the following permission to the players or groups that are going to be able to use the command:

pop.use

Write in game chat: /pop (you can change the command via the config file)

It will return to you (not broadcasted to the server, only to the player that issued the command):

Players Online: 79/100 Players Joining: 3

Localization:Copy the oxide/lang/en folder and rename it to a new language (like de for german), edit the file Pop.json inside that folder overwriting with text in the new language.

Hope you enjoy!
