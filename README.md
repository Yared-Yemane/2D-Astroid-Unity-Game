# 2D Asteroids Unity Game

A classic-style 2D asteroids shooter built in Unity. Pilot your ship, destroy asteroids, collect pickups, and survive until your lives run out.

## Requirements

- [Unity 6000.4.3f1](https://unity.com/) (Unity 6)
- Universal Render Pipeline (2D)

## Getting started

1. Clone the repository:
   ```bash
   git clone https://github.com/Yared-Yemane/2D-Astroid-Unity-Game.git
   ```
2. Open the project folder in Unity Hub.
3. Open **Assets/Scenes/SampleScene.unity** (or press Play with the default build scene).
4. Press **Play** in the Editor.

## Controls

| Action   | Input                          |
| -------- | ------------------------------ |
| Move     | WASD / arrow keys              |
| Fire     | Attack binding (see Input System) |
| Boost    | Boost binding (see Input System)  |

Input is defined in **Assets/InputSystem_Actions.inputactions**.

## Features

- Ship movement, rotation, thrust VFX, and boost
- Asteroid spawning and splitting
- Bullets, screen wrapping, and 2D physics collisions
- Score, lives, and high score (UI Toolkit HUD)
- Collectibles and game-over flow (retry / exit)
- Sound effects and music (see **Assets/Sounds/License.txt** for attribution)

## Project structure

| Path | Description |
| ---- | ----------- |
| `Assets/Scripts/` | Gameplay and UI scripts |
| `Assets/Scenes/` | `SampleScene` (game), `MainMenu` |
| `Assets/UI Toolkit/` | HUD layout and styles |
| `Assets/Prefabs/` | Asteroids, bullets, VFX, etc. |
| `Assets/Sounds/` | Audio and mixer |

## License

Game code in this repository is provided as-is for learning and portfolio use. Third-party audio assets are listed with their licenses in **Assets/Sounds/License.txt**.
