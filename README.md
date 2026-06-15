# Robotic Arm Color Sorting Simulator

<a id="readme-top"></a>

[![Unity][Unity-shield]][Unity-url]
[![CSharp][CSharp-shield]][CSharp-url]


<br />
<div align="center">
  <a href="https://github.com/YOUR_USERNAME/robotic-arm-color-sort">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">UR5 Robotic Arm Color Sorting Simulator</h3>

  <p align="center">
    A Unity-based robotics simulation featuring a UR5 robot arm that uses a visual sensor to identify, pick up, and sort colored blocks using inverse kinematics and articulation body physics.
    <br />
  </p>
</div>

---

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#key-systems">Key Systems</a></li>
      </ul>
    </li>
    <li><a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#project-structure">Project Structure</a></li>
    <li><a href="#script-architecture">Script Architecture</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#robotics-research-context">Robotics Research Context</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

---

## About The Project

[![Product Screenshot][product-screenshot]](https://github.com/YOUR_USERNAME/robotic-arm-color-sort)

This project is a robotics manipulation simulator built in Unity 6 using a real **UR5 robot model** (imported via URDF). The simulation demonstrates a complete perception-to-action pipeline:

- **4 colored blocks** (red, green, blue, yellow) spawn at random positions within the arm's reach every round
- A **visual sensor** (camera-based detection system) scans the scene and identifies the target block by color
- The **UR5 arm** moves using Articulation Body joint control — the same physics model used in professional robotics simulation tools
- A **gripper** picks up the identified block and places it in a designated drop box
- The user can **reset the environment** at any time, respawning blocks at new random positions

This project was built to demonstrate robotics simulation competency for a research lab application, showcasing skills in manipulation, perception, task planning, and physics-based simulation.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

### Built With

* [![Unity][Unity-shield]][Unity-url]
* [![CSharp][CSharp-shield]][CSharp-url]
* [![ROS][ROS-shield]][ROS-url]

**Unity Packages Used:**
* [URDF Importer](https://github.com/Unity-Technologies/URDF-Importer) — imports real UR5 robot model with joint limits and physics
* [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) — UI text rendering

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

### Key Systems

| System | Description |
|---|---|
| **Visual Sensor** | Camera-based object detection using tag lookup and distance filtering — simulates real robot perception |
| **Articulation Body Control** | Each UR5 joint is driven using Unity's PhysX 4.0 ArticulationBody — physically accurate joint simulation |
| **Pick-and-Place Pipeline** | Full manipulation sequence: scan → approach → grasp → lift → transport → release |
| **Task State Machine** | Prevents concurrent task execution, handles errors, supports mid-task abort |
| **Random Block Spawner** | Spawns 4 non-overlapping colored blocks within the arm's workspace every round |
| **Environment Reset** | Fully resets scene state — arm returns home, gripper releases, blocks respawn |

---

## Getting Started

### Prerequisites

* **Unity Hub** — [Download here](https://unity.com/download)
* **Unity 6 LTS** — Install via Unity Hub
* **Git** — [Download here](https://git-scm.com)
* **Git LFS** — [Download here](https://git-lfs.github.com) (required for binary assets)
* A code editor — [Visual Studio Community](https://visualstudio.microsoft.com) (recommended) or [VS Code](https://code.visualstudio.com)

### Installation

1. **Clone the repository**
   ```sh
   git clone https://github.com/YOUR_USERNAME/robotic-arm-color-sort.git
   ```

2. **Pull LFS assets** (3D models, textures)
   ```sh
   cd robotic-arm-color-sort
   git lfs pull
   ```

3. **Open in Unity Hub**
   - Open Unity Hub
   - Click **Add → Add project from disk**
   - Select the cloned `robotic-arm-color-sort` folder
   - Make sure the editor version is set to **Unity 6 LTS**
   - Click **Open**

4. **Wait for Unity to import assets**
   - First-time import takes 2–5 minutes
   - Watch the progress bar in the bottom right of Unity

5. **Open the main scene**
   - In the Project window navigate to `Assets/Scenes/`
   - Double-click `MainScene.unity`

6. **Press Play**
   - The simulation starts automatically
   - 4 colored blocks spawn in front of the arm
   - Use the UI buttons to select a color and start the task

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Usage

### Basic Operation

| Action | How |
|---|---|
| **Select a target color** | Click one of the 4 colored buttons (Red, Green, Blue, Yellow) |
| **Watch the task execute** | The arm scans, moves, grasps the block, and places it in the box automatically |
| **Reset the environment** | Click the **Reset** button — arm returns home and blocks respawn at new positions |

### Task Pipeline (what happens when you click a color)

```
1. Visual sensor scans the scene
          ↓
2. Target block located by color tag
          ↓
3. Arm moves to pick pose
          ↓
4. Gripper closes — block grasped
          ↓
5. Arm lifts block
          ↓
6. Arm moves to drop box
          ↓
7. Gripper opens — block placed
          ↓
8. Arm returns to home position
```

### Status Messages

The status panel at the top of the screen shows what the arm is doing at each step:

| Message | Meaning |
|---|---|
| `Select a color to begin` | Idle — waiting for input |
| `Scanning for [color] block...` | Visual sensor is searching |
| `Moving to [color] block...` | Arm is approaching the block |
| `Gripping block...` | Gripper is closing |
| `Placing block in box...` | Arm is moving to the drop box |
| `Returning home...` | Task complete, arm resetting |
| `[color] block placed!` | Full task completed successfully |
| `Arm is busy — please wait` | Task already in progress |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Project Structure

```
RoboticArmColorSort/
├── Assets/
│   ├── Materials/
│   │   ├── ArmMetal.mat
│   │   ├── Block_Red.mat
│   │   ├── Block_Green.mat
│   │   ├── Block_Blue.mat
│   │   ├── Block_Yellow.mat
│   │   ├── BoxMat.mat
│   │   └── GroundMat.mat
│   ├── Models/
│   │   └── ur5/
│   │       ├── ur5.urdf              ← UR5 robot description
│   │       └── meshes/
│   │           ├── visual/           ← visual geometry (.dae)
│   │           └── collision/        ← collision geometry (.stl)
│   ├── Prefabs/
│   │   ├── Block_Red.prefab
│   │   ├── Block_Green.prefab
│   │   ├── Block_Blue.prefab
│   │   └── Block_Yellow.prefab
│   ├── Scenes/
│   │   └── MainScene.unity
│   └── Scripts/
│       ├── ArmController.cs          ← joint control via ArticulationBody
│       ├── BlockSpawner.cs           ← random block placement
│       ├── GripperController.cs      ← finger animation and grasping
│       ├── ResetManager.cs           ← environment reset
│       ├── TaskManager.cs            ← pick-and-place state machine
│       ├── UIManager.cs              ← button handling and status text
│       └── VisualSensor.cs           ← color-based block detection
├── Packages/
│   └── manifest.json
├── ProjectSettings/
├── .gitignore
├── .gitattributes                    ← Git LFS tracking rules
└── README.md
```

---

## Script Architecture

```
UIManager
    │  onClick events
    ↓
TaskManager  ←──────────────────────────────────┐
    │                                            │
    ├──→ VisualSensor.DetectBlock()              │
    │         Returns: target block position     │
    │                                            │
    ├──→ ArmController.GoToPose()               │
    │         Drives: ArticulationBody joints    │
    │                                            │
    ├──→ GripperController.CloseGripper()       │
    │         Grasps: block via OverlapSphere    │
    │                                            │
    └──→ ResetManager.ResetEnvironment() ───────┘
              Calls: BlockSpawner.SpawnAllBlocks()
```

### Manager Responsibilities

| Script | Attached To | Role |
|---|---|---|
| `BlockSpawner.cs` | `Managers` | Spawns and tracks 4 colored blocks |
| `VisualSensor.cs` | `SensorCamera` | Detects blocks and drop box by position |
| `ArmController.cs` | `ur5` | Controls all 6 UR5 joints via ArticulationBody |
| `GripperController.cs` | `Gripper` | Animates fingers, detects and holds blocks |
| `TaskManager.cs` | `Managers` | Orchestrates full pick-and-place sequence |
| `UIManager.cs` | `Canvas` | Handles button input and status display |
| `ResetManager.cs` | `Managers` | Resets arm, gripper, and block positions |

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Robotics Research Context

This project was built to develop practical skills relevant to robotics research through hands-on simulation work.

Skills developed:

- Working with URDF robot, the standard format used across ROS-based tools
- Physics-accurate joint control using Articulation Bodies and drive configuration
- Designing a perception-action pipeline: sensor detects target, arm plans and executes task
- Implementing a task state machine for structured, repeatable manipulation sequences
- Setting up a repeatable simulation environment with randomised initial conditions
- Importing and configuring a real industry robot model (UR5) used in research labs worldwide
- Understanding pick-and-place manipulation: approach, grasp, transport, release

The UR5 robot model used in this project is the same model used in real university robotics labs worldwide. The URDF file contains the actual joint limits, link masses, and inertia tensors of the physical robot.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Contact

Wayne Chiu - yw.chiu@mail.utoronto.ca or wayne.chiu0823@gmail.com

---

## Acknowledgments

* [Unity Robotics Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub) — URDF Importer and robotics simulation tools
* [ROS Industrial](https://github.com/ros-industrial/universal_robot) — UR5 URDF robot description
* [Universal Robots](https://www.universal-robots.com) — UR5 robot hardware specifications
* [Unity Technologies](https://unity.com) — Unity 6 game engine and ArticulationBody physics

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

<!-- MARKDOWN LINKS & IMAGES -->
[license-shield]: https://img.shields.io/github/license/YOUR_USERNAME/robotic-arm-color-sort.svg?style=for-the-badge
[license-url]: https://github.com/YOUR_USERNAME/robotic-arm-color-sort/blob/main/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/YOUR_USERNAME
[product-screenshot]: images/screenshot.png
[Unity-shield]: https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white
[Unity-url]: https://unity.com
[CSharp-shield]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white
[CSharp-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
[ROS-shield]: https://img.shields.io/badge/ROS-22314E?style=for-the-badge&logo=ros&logoColor=white
[ROS-url]: https://www.ros.org
