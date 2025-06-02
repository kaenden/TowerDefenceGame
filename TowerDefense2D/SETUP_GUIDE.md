# Unity 2D Tower Defense - Quick Setup Guide

This guide will help you quickly set up the tower defense game in Unity.

## 📋 Prerequisites

- Unity 2021.3 LTS or newer
- Basic understanding of Unity Editor
- Mobile device for testing (optional)

## 🚀 Quick Setup (15 minutes)

### Step 1: Create New Unity Project
1. Open Unity Hub
2. Create new 2D project
3. Name it "TowerDefense2D"

### Step 2: Import Scripts
1. Copy all scripts from the `Scripts/` folder to `Assets/Scripts/`
2. Wait for Unity to compile

### Step 3: Create Basic Scene Structure

#### A. Create Game Manager
1. Create empty GameObject, name it "GameManager"
2. Add these scripts:
   - GameManager
   - WaveManager
   - UIManager

#### B. Create Path System
1. Create empty GameObject, name it "PathManager"
2. Add PathManager script
3. Add LineRenderer component
4. Create child GameObjects for waypoints:
   - Right-click PathManager → Create Empty
   - Name them "Waypoint1", "Waypoint2", etc.
   - Position them to create a path (e.g., (-8,0), (-4,2), (0,0), (4,-2), (8,0))

#### C. Create Tower Placement System
1. Create empty GameObject, name it "TowerPlacement"
2. Add TowerPlacement script

#### D. Setup Camera
1. Select Main Camera
2. Add MobileInputHandler script
3. Set camera to Orthographic
4. Set Size to 5

### Step 4: Create UI Canvas
1. Right-click in Hierarchy → UI → Canvas
2. Set Canvas Scaler:
   - UI Scale Mode: "Scale With Screen Size"
   - Reference Resolution: 1920x1080
   - Screen Match Mode: "Match Width Or Height" (0.5)

#### Create UI Elements:
1. **HUD Panel** (top of screen):
   - Lives Text: "Lives: 20"
   - Money Text: "$100"
   - Wave Text: "Wave: 1"
   - Score Text: "Score: 0"

2. **Tower Selection Panel** (bottom):
   - 5 buttons for different towers
   - Set button text to tower names and costs

3. **Tower Info Panel** (side, initially disabled):
   - Tower name text
   - Tower stats text
   - Upgrade button
   - Sell button

### Step 5: Create Basic Prefabs

#### Tower Prefab:
1. Create empty GameObject, name it "BasicTower"
2. Add SpriteRenderer (use Unity's default sprite or create a simple colored square)
3. Add CircleCollider2D, set as Trigger, radius 3
4. Add Tower script
5. Create child GameObject "FirePoint" at tower's edge
6. Create child GameObject "RangeIndicator" with circle sprite (scale to match range)
7. Save as prefab in `Assets/Prefabs/Towers/`

#### Enemy Prefab:
1. Create empty GameObject, name it "BasicEnemy"
2. Add SpriteRenderer (different color from tower)
3. Add CircleCollider2D for collision
4. Add Rigidbody2D, freeze Z rotation
5. Add Enemy script and Pathfinding script
6. Save as prefab in `Assets/Prefabs/Enemies/`

#### Projectile Prefab:
1. Create empty GameObject, name it "BasicProjectile"
2. Add SpriteRenderer (small circle or bullet shape)
3. Add CircleCollider2D, set as Trigger
4. Add Rigidbody2D
5. Add Projectile script
6. Save as prefab in `Assets/Prefabs/Projectiles/`

### Step 6: Configure Components

#### GameManager Configuration:
- Starting Lives: 20
- Starting Money: 100
- Assign WaveManager and UIManager references

#### WaveManager Configuration:
- Create a simple wave:
  - Wave 1: 10 Basic Enemies, 1 second between spawns
- Set Spawn Point to first waypoint position
- Enable Auto Start Waves

#### Tower Configuration:
- Basic Tower: Damage 25, Range 3, Fire Rate 1, Cost 50
- Assign projectile prefab
- Set fire point reference

#### UIManager Configuration:
- Assign all UI text references
- Assign tower buttons and prefabs
- Connect button events

### Step 7: Setup Layers
1. Go to Edit → Project Settings → Tags and Layers
2. Create these layers:
   - Layer 8: "Enemy"
   - Layer 9: "Tower"
   - Layer 10: "Projectile"
   - Layer 11: "PlacementArea"
   - Layer 12: "Obstacle"

3. Assign layers to prefabs:
   - Enemy prefab → Enemy layer
   - Tower prefab → Tower layer
   - Projectile prefab → Projectile layer

### Step 8: Test the Game
1. Press Play
2. Click tower buttons to select towers
3. Click on empty areas to place towers
4. Enemies should spawn and follow the path
5. Towers should target and shoot enemies

## 🎮 Testing Checklist

- [ ] Enemies spawn and follow waypoints
- [ ] Towers can be placed and removed
- [ ] Towers target and shoot enemies
- [ ] UI updates correctly (money, lives, score)
- [ ] Touch controls work (if testing on mobile)
- [ ] Game over triggers when lives reach 0
- [ ] Waves progress automatically

## 🔧 Common Issues & Solutions

### Enemies Not Moving
- Check waypoint positions in PathManager
- Ensure Pathfinding script is attached
- Verify waypoints are assigned to PathManager

### Towers Not Shooting
- Check layer masks in Tower script
- Ensure projectile prefab is assigned
- Verify fire point is set correctly

### UI Not Updating
- Check script references in UIManager
- Ensure event subscriptions are working
- Verify UI elements are assigned

### Touch Controls Not Working
- Check if EventSystem exists in scene
- Verify MobileInputHandler is attached to camera
- Test with mouse in editor first

## 📱 Mobile Build Settings

### For Android:
1. File → Build Settings → Android
2. Player Settings:
   - Orientation: Landscape
   - Minimum API Level: 21
   - Target API Level: Latest
3. XR Settings: None

### For iOS:
1. File → Build Settings → iOS
2. Player Settings:
   - Orientation: Landscape
   - Target minimum iOS Version: 11.0
3. Camera Usage Description: "For AR features" (if using camera)

## 🎨 Art Assets (Optional)

If you want to add custom graphics:

1. **Sprites** (2D textures):
   - Tower sprites: 128x128 pixels
   - Enemy sprites: 64x64 pixels
   - Projectile sprites: 32x32 pixels
   - UI elements: Various sizes

2. **Import Settings**:
   - Texture Type: Sprite (2D and UI)
   - Pixels Per Unit: 100
   - Filter Mode: Point (for pixel art) or Bilinear

3. **Sprite Atlas** (for performance):
   - Window → 2D → Sprite Atlas
   - Add all game sprites to atlas

## 🔊 Audio Setup (Optional)

1. Create AudioManager GameObject
2. Add AudioManager script
3. Add audio clips to the sounds array
4. Configure volume settings

## 📈 Performance Optimization

1. **Object Pooling**:
   - Create ObjectPool GameObject
   - Add ObjectPool script
   - Configure pools for enemies and projectiles

2. **Mobile Optimization**:
   - Use sprite atlases
   - Limit particle effects
   - Optimize texture sizes
   - Use object pooling

## 🎯 Next Steps

Once basic setup is complete:

1. **Add More Content**:
   - Create different tower types
   - Add more enemy varieties
   - Design multiple levels

2. **Polish Features**:
   - Add particle effects
   - Implement sound effects
   - Create animated sprites

3. **Advanced Features**:
   - Save/load system
   - Achievement system
   - In-app purchases

## 📞 Support

If you encounter issues:
1. Check Unity Console for errors
2. Verify all script references are assigned
3. Test in editor before building to mobile
4. Check the main README.md for detailed documentation

Happy game development! 🎮