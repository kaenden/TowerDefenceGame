# Mobile 2D Tower Defense Game for Unity

A complete mobile-optimized 2D tower defense game built for Unity with touch controls, multiple tower types, enemy varieties, and wave-based gameplay.

## 🎮 Game Features

### Core Gameplay
- **Wave-based enemy spawning** with increasing difficulty
- **Multiple tower types** with unique abilities and upgrade paths
- **Touch-optimized controls** for mobile devices
- **Strategic tower placement** with grid-based system
- **Economy system** with money management
- **Score tracking** and progression

### Tower Types
1. **Basic Tower** - Standard damage, balanced stats
2. **Sniper Tower** - High damage, long range, slow fire rate
3. **Splash Tower** - Area damage, explosive projectiles
4. **Slow Tower** - Slows enemies, crowd control
5. **Freeze Tower** - Freezes enemies temporarily

### Enemy Types
1. **Basic Enemy** - Standard health and speed
2. **Fast Enemy** - Low health, high speed
3. **Tank Enemy** - High health, slow speed, armor
4. **Flying Enemy** - Immune to some tower effects
5. **Regenerating Enemy** - Heals over time

### Mobile Features
- **Touch controls** with tap, double-tap, and long-press gestures
- **Pinch-to-zoom** camera control
- **Responsive UI** that scales across different screen sizes
- **Performance optimized** for mobile devices
- **Object pooling** for smooth performance

## 📁 Project Structure

```
TowerDefense2D/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs          # Main game state management
│   │   ├── Enemy.cs                # Base enemy class
│   │   ├── Pathfinding.cs          # Enemy movement system
│   │   ├── PathManager.cs          # Path visualization and management
│   │   └── TowerPlacement.cs       # Tower placement system
│   ├── Towers/
│   │   └── Tower.cs                # Base tower class with targeting
│   ├── Enemies/
│   │   └── BasicEnemy.cs           # Enemy type implementations
│   ├── Projectiles/
│   │   └── Projectile.cs           # Projectile system with effects
│   ├── UI/
│   │   ├── UIManager.cs            # Main UI controller
│   │   ├── MobileInputHandler.cs   # Touch input system
│   │   └── HealthBar.cs            # Enemy health display
│   ├── Managers/
│   │   └── WaveManager.cs          # Wave spawning system
│   └── Utils/
│       ├── ObjectPool.cs           # Performance optimization
│       └── AudioManager.cs         # Sound management
├── Prefabs/
│   ├── Towers/                     # Tower prefabs
│   ├── Enemies/                    # Enemy prefabs
│   ├── Projectiles/                # Projectile prefabs
│   └── UI/                         # UI prefabs
├── Scenes/                         # Game scenes
├── Materials/                      # Materials and shaders
├── Sprites/                        # 2D artwork
└── Audio/                          # Sound effects and music
```

## 🚀 Setup Instructions

### 1. Unity Setup
1. Create a new Unity 2D project
2. Copy all scripts to your project's Assets folder
3. Set up the scene hierarchy as described below

### 2. Scene Setup

#### Main Scene Hierarchy:
```
Main Camera
├── MobileInputHandler (script)

GameManager (empty GameObject)
├── GameManager (script)
├── WaveManager (script)
├── UIManager (script)

PathManager (empty GameObject)
├── PathManager (script)
├── LineRenderer
└── Waypoints (child GameObjects)
    ├── Waypoint1
    ├── Waypoint2
    └── ...

TowerPlacement (empty GameObject)
└── TowerPlacement (script)

Canvas (Screen Space - Overlay)
├── HUD Panel
│   ├── Lives Text
│   ├── Money Text
│   ├── Wave Text
│   ├── Score Text
│   ├── Pause Button
│   └── Speed Button
├── Tower Selection Panel
│   └── Tower Buttons (1-5)
├── Tower Info Panel
│   ├── Tower Name Text
│   ├── Tower Stats Text
│   ├── Upgrade Button
│   └── Sell Button
├── Game Over Panel
└── Pause Panel

AudioManager (empty GameObject)
├── AudioManager (script)
├── Music Source
└── SFX Source

ObjectPool (empty GameObject)
└── ObjectPool (script)
```

### 3. Prefab Creation

#### Tower Prefab Setup:
1. Create a GameObject with SpriteRenderer
2. Add CircleCollider2D (set as trigger)
3. Add Tower script
4. Create child GameObject for fire point
5. Add range indicator (circle sprite, initially disabled)

#### Enemy Prefab Setup:
1. Create a GameObject with SpriteRenderer
2. Add CircleCollider2D for collision
3. Add Rigidbody2D (freeze rotation)
4. Add Enemy script and Pathfinding script
5. Add HealthBar prefab as child

#### Projectile Prefab Setup:
1. Create a GameObject with SpriteRenderer
2. Add CircleCollider2D (set as trigger)
3. Add Rigidbody2D
4. Add Projectile script

### 4. Layer Setup
Create these layers in Unity:
- **Default** (0) - General objects
- **Enemy** (8) - Enemy units
- **Tower** (9) - Tower units
- **Projectile** (10) - Projectiles
- **PlacementArea** (11) - Valid tower placement areas
- **Obstacle** (12) - Invalid placement areas

### 5. UI Setup
1. Create Canvas with Screen Space - Overlay
2. Set Canvas Scaler to "Scale With Screen Size"
3. Set Reference Resolution to 1920x1080
4. Set Screen Match Mode to "Match Width Or Height" (0.5)

## 🎯 Game Configuration

### Wave Configuration
Edit the WaveManager component to set up waves:
```csharp
// Example wave setup
Wave wave1 = new Wave
{
    waveName = "Wave 1",
    enemies = new List<EnemySpawnInfo>
    {
        new EnemySpawnInfo { enemyPrefab = basicEnemyPrefab, count = 10, spawnDelay = 0f }
    },
    timeBetweenSpawns = 1f,
    timeBeforeNextWave = 5f
};
```

### Tower Balancing
Adjust tower stats in the Tower script:
- **Damage**: Base damage per shot
- **Range**: Attack range in Unity units
- **Fire Rate**: Shots per second
- **Cost**: Purchase price

### Enemy Balancing
Modify enemy stats in enemy scripts:
- **Health**: Total hit points
- **Speed**: Movement speed
- **Rewards**: Money and score given when defeated

## 📱 Mobile Optimization

### Performance Tips
1. **Object Pooling**: Use ObjectPool for frequently spawned objects
2. **Sprite Atlasing**: Combine sprites into atlases
3. **Audio Compression**: Use compressed audio formats
4. **Texture Compression**: Use appropriate texture compression
5. **Draw Call Batching**: Use sprite batching

### Touch Controls
- **Single Tap**: Select towers or place towers
- **Double Tap**: Quick upgrade selected tower
- **Long Press**: Show tower context menu
- **Pinch**: Zoom camera
- **Drag**: Pan camera (when not placing towers)

### Screen Adaptation
- UI automatically scales to different screen sizes
- Touch areas are sized appropriately for fingers
- Important UI elements are positioned in safe areas

## 🔧 Customization

### Adding New Tower Types
1. Create new TowerType enum value
2. Implement tower behavior in Tower.cs
3. Create tower prefab
4. Add to TowerPlacement tower list

### Adding New Enemy Types
1. Create new enemy script inheriting from Enemy
2. Override necessary methods for special abilities
3. Create enemy prefab
4. Add to WaveManager enemy lists

### Adding Special Effects
1. Use Projectile script's special properties
2. Implement new effects in Enemy script
3. Create particle effects for visual feedback

## 🎵 Audio Integration

### Sound Effects
- Tower firing sounds
- Enemy death sounds
- UI interaction sounds
- Wave start/complete sounds

### Music
- Background music with fade in/out
- Dynamic music based on game state
- Volume controls for music and SFX

## 🐛 Troubleshooting

### Common Issues
1. **Towers not firing**: Check layer masks and enemy detection
2. **Enemies not moving**: Verify waypoint setup and pathfinding
3. **UI not responsive**: Check Canvas settings and event system
4. **Performance issues**: Enable object pooling and optimize sprites

### Debug Features
- Gizmos for tower range and enemy paths
- Console logging for game events
- Visual indicators for placement validity

## 📈 Future Enhancements

### Potential Features
- **Boss enemies** with special mechanics
- **Tower combinations** for unique effects
- **Power-ups** and temporary abilities
- **Multiple levels** with different paths
- **Achievement system**
- **Leaderboards** and social features
- **In-app purchases** for tower skins
- **Daily challenges**

### Technical Improvements
- **Save system** for progress persistence
- **Analytics integration** for player behavior
- **Cloud saves** for cross-device play
- **Localization** for multiple languages

## 📄 License

This project is provided as-is for educational and commercial use. Feel free to modify and extend the codebase for your own projects.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.