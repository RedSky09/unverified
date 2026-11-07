\# UNVERIFIED



A first-person psychological horror game about room inspection and anomaly detection.

Built with Unity 6 (HDRP). Solo project.



\*\*Play for free (Windows):\*\* \[itch.io link]



> \*Inspection. Anomaly. Trust No One.\*



\## Premise

You are hired by Mr. Voss to inspect rooms in a Victorian house.

For each room, you decide: is everything normal, or is something wrong?

If something is wrong, you must also report what kind of anomaly it is.

Mr. Voss briefs you before every inspection, but his briefings are not always accurate.



\## Features

\- 9 rooms with randomized anomalies (normal / anomaly roughly 40:60)

\- Anomaly types: missing object, extra object, position, rotation, and scale changes

\- Two-step inspection report system

\- Flashlight with battery drain, flicker, and adjustable beam angle

\- Randomly spawned battery pickups (hidden in drawers and around the room)

\- Unreliable narrator: briefings can be misleading

\- Three endings based on number of mistakes



\## Controls

| Key | Action |

|---|---|

| WASD | Move |

| Mouse | Look around |

| F | Toggle flashlight |

| Mouse wheel | Adjust beam angle |

| E | Interact |

| Tab | Open inspection report |



\## About this repository

This repository contains \*\*only the C# gameplay scripts\*\*.

Third-party assets (3D models, UI, audio, fonts) are not included due to licensing,

so the project will not run as-is from this repo alone.



\### Script overview

| Script | Purpose |

|---|---|

| `LevelManager` | Decides whether a room is normal or anomalous and activates anomalies |

| `AnomalyObject` | A single object that can change (position, rotation, scale, visibility) |

| `AnomalyGroup` | Groups several anomaly objects into one anomaly |

| `ReportSystem` | Inspection report UI and answer validation |

| `GameManager` | Room progression, mistake tracking, ending selection |

| `VossDialog` | Briefings, hints, and the unreliable narrator logic |

| `Flashlight` | Battery drain, flicker, and beam control |

| `InteractionSystem` | Raycast-based interaction (drawers, batteries) |

| `Drawer`, `BatteryPickup` | Interactable objects |

| `BatterySpawnManager` | Randomly places batteries (max 2 per room) |

| `FirstPersonController` | Movement, mouse look, headbob |

| `AudioManager` | Footsteps, interaction sounds, ambience |

| `EndingScreen` | Ending presentation and restart |

| `MainMenu` | Menu, settings (sensitivity, volume, fullscreen), credits |



\## Tech

\- Unity 6 (6000.4.x), High Definition Render Pipeline

\- Unity Input System package



\## Credits

Developer: nv3rmind

Assets: ZNS3D (Victorian interior), SlimUI (UI), Rubik Glitch and Courier Prime (fonts)

Audio: Freesound.org and Pixabay contributors



\## License

Code in this repository is © nv3rmind. All rights reserved unless stated otherwise.

Third-party assets belong to their respective owners.

