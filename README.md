🎮 Pixel Flow Clone – Unity Prototype

A lightweight technical clone of the mobile puzzle game Pixel Flow.
Built with Unity 6000.0.58f2, Zenject, DOTween, Odin Inspector, Dreamteck Splines, and a modular grid architecture.

📌 Overview

Tap a Shooter → it jumps onto the conveyor belt

Shooter moves along a spline and auto-fires at the nearest matching-color cube

Goal: clear all cubes and complete the picture

Built as a case study + technical architecture showcase

🔧 Core Features

Texture-based ColorCube Grid generation

Modular Shooter System (raycast detection, color matching, async firing)

Spline-based conveyor (Dreamteck Splines)

Three Grid Systems: Shooter, ColorCube, ReservedSlot

Full Object Pooling for all gameplay objects

Zenject DI throughout the project

DOTween-based VFX, recoil, movement

Last-Shooter time-scale effect

MaterialPropertyBlock usage for optimized coloring

🧩 Gameplay Flow

Level loads from LevelData

Shooters & color cubes spawn

Player selects a shooter

Shooter moves onto conveyor → auto-fires

Cubes destroyed → picture clears

No slots left → fail

All cubes cleared → win

🏗️ Architecture

Major systems:

Managers: Game, Level, Shooter, FX, Input, Settings

Grid Systems: ShooterGrid, ColorCubeGrid, ReservedSlotGrid

Gameplay Objects: Shooter, ColorCube, Conveyor, Bullet

ScriptableObjects: GameSettings, ColorSettings, LevelData

Installers: Scene installer + SO installer (Zenject)

📁 Project Structure (Simplified)
Scripts/
 ├── Audio
 ├── Data
 ├── Game
 ├── Installers
 ├── Level
 ├── Managers
 ├── Pools
 ├── SaveSystem
 ├── Signals
 ├── State Machine
 ├── UI
 └── Utils

🛠️ Tech Stack

Unity 6000.0.58f2 • Zenject • DOTween • Odin Inspector • Dreamteck Splines • UniTask • TextMeshPro
