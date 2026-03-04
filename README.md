# MadboxAssignment

## Recap :

As discussed during the interviews my strength are in 3C, juiciness and gameplay so i'm focusing on reflecting those as much as possible. I spent 3 days on this assignment. Gameplay video : https://youtube.com/shorts/cD97-QyQbtU

## What's been done :
### Architecture
- Separation of Logic and Data as much as possible. ex : EntityModule define behaviour while the datas are in EntityData. This separation allow for easy implementation of new enemies for example (one with more health, another one who runs faster, etc...)
- Unit system : following unity's own architecture I implemented a component based unit system. The Entity class is the basis on which behaviour are created by adding EntityModules to it. For performance and easy access, the entity gets and stores all its modules on awake. Every unit also automatically register to the EntityManager which allow for easy access of every Entity in the game.
- Boostrapper : Single entry point for the project. The project will always start by loading the InitScene before going to the mainScene (or menuScene if applicable). This gives me better control over code execution order. All singletons are for sure initialized first.
- Event-Driven Design : OnDeath, OnHealthChanged, OnStartedMoving etc. keep modules decoupled. The health bar, drop module, and unregistration all react to the same events without knowing each other.
- The LevelManager handles the waves of enemies which are defined in LevelData (scriptable objects) and the overall loop.
- Save system : eventhough it is not extensively used due to the small nature of this project, everything that gets put in GameData is automatically savec using EasySave. For now, only the current experience gained and current level are saved

### Juiciness
- Floating text : floating damage numbers and critical strike integration (using TMP sprite asset)
- Health bar : shake & flash upon taking damage. Delta chunk showing how much the value changed
- Unit damage feedback : shake and shine upon taking damage
- Player's animation speed scales with attack speed
- Overall VFXs

### Performance
- Created a 3D environment that I turned into 2D sprite (like in Archero)
- Pools : VFXs, Units, projectiles, Some ui elements (like healthbars) are all pooled. My pool system is built on top of LeanPool.
- Update() usage is kept to a minimum (like catching inputs for example) and most of the code is following the Observer pattern and events as a whole.
- GPU instanced materials + optimised shader (ToonyColors) + mobile mode activated

## To go further :
- Stats System : for now stats are represented by float values in EntityData but the goal should be to implement a StatSystem Following the EntityModule architecture and create a EntityStatModule that would handle base stats as well as stat modifications at run time.
- New playable characters : mage, knight, etc... (could be tied to a gatcha deck building system)
- Real implementation of the choice popup : pick 1 of 3 bonuses (tied to the stat system. Fake for now)
- Real UI system
- UI : ui element animations, Wave UI, etc...
- AI different behaviours : extend on the existing EntityModule architecture to define new behaviours (distance enemy, charge enemy, etc...)
- Sounds & music
- Health packs drop from time to time
- Overall game progression : more worlds when the first one is finished
- Daily rewards
- overall monetisation

## What I would do in a real use case :
- Not import whole asset packs into the project
- Look into and optimise asset import settings
- Some Singletons are a bit more rigid than they could be.
- Use of Addressables

## External packages used :
- Ultimate Joystick : https://assetstore.unity.com/packages/p/ultimate-joystick-27695
- Mybox (utility extensions methods) : https://github.com/Deadcows/MyBox
- GUI Pro - Super casual : https://assetstore.unity.com/packages/2d/gui/gui-pro-super-casual-278534
- Kaykit - Adventurers pack : https://assetstore.unity.com/packages/3d/characters/humanoids/humans/kaykit-adventurers-character-pack-for-unity-290679
- Kaykit - Skeletons packs : https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/kaykit-skeletons-character-pack-for-unity-290680
- Kenney - Mini dungeon : https://kenney.nl/assets/mini-dungeon
- Lean pool : https://assetstore.unity.com/packages/tools/utilities/lean-pool-35666
- Toony colors : https://assetstore.unity.com/packages/vfx/shaders/toony-colors-pro-2-8105?srsltid=AfmBOopBofCiK24P9bP5rmajTiLKZ7SZBnfP5oSJQJKDpYoQJn6oNDLF
- Multi Screenshot : https://assetstore.unity.com/packages/tools/utilities/multi-screenshot-249566
- EasySave : https://assetstore.unity.com/packages/tools/utilities/easy-save-the-complete-save-game-data-serializer-system-768
- OdinInspector  : https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041
- Dotween - https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676
- Epic Toon FX
