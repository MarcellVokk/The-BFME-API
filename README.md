# The-BFME-API
This Library allows you to launch and interact with BFME games as well as create virual LAN network rooms to play together even when you and other players are not on the same network.

## Features
- Create virtual LAN networks so you and your opponents can play while on completely different networks
- Launch BFME as host or offhost and automaticaly:
  - Create a new room if host, or join the room the host created on that network if offhost
  - Set player username
  - Set player color
  - Select player army
  - Select player team
  - Select spot on map
  - Select map
  - Start the match
  - Detect who won the match

## Spot detection
With some pretty clever code, this library is able to automaticaly detect spots from any map, and assign an index to them.
![Spot detection example image.]()
