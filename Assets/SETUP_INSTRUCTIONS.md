# Instructions de Configuration - Jeu de Tir VR

## Scripts créés

Les scripts suivants ont été créés dans `Assets/Scripts/` :

1. **WeaponController.cs** - Contrôle l'arme, le tir et les munitions
2. **Target.cs** - Gère les cibles destructibles et le scoring
3. **BulletProjectile.cs** - Gère les projectiles (si vous choisissez cette méthode)
4. **GameManager.cs** - Gère le score, le timer et la boucle de jeu
5. **WeaponHUD.cs** - Affiche les munitions dans l'UI
6. **FloatingText.cs** - Affiche les points gagnés en 3D

## Configuration de la scène Unity

### Étape 1 : Créer la scène VR de base

1. Ouvrez votre scène dans Unity (`Assets/Scenes/SampleScene.unity`)
2. Supprimez la `Main Camera` par défaut
3. Depuis le dossier `Assets/Samples/XR Interaction Toolkit/2.6.5/Starter Assets/Prefabs/`, glissez le prefab **"XR Origin (XR Rig)"** dans la scène
4. Glissez également le prefab **"XR Interaction Setup"** dans la scène

### Étape 2 : Créer l'arme grabbable

#### A. Créer le modèle de l'arme

1. Créez un GameObject vide : `GameObject > Create Empty` et nommez-le "Weapon"
2. Ajoutez un modèle 3D simple pour représenter l'arme (par exemple, créez un pistolet simple avec des cubes) :
   - Créez un `Cube` comme enfant, redimensionnez-le (ex: scale 0.2, 0.1, 0.4)
   - Créez un autre `Cube` pour la poignée
   - Positionnez-les pour qu'ils ressemblent à un pistolet

#### B. Ajouter les composants nécessaires

Sur le GameObject "Weapon", ajoutez les composants suivants :

1. **Rigidbody**
   - Décochez "Use Gravity" si vous voulez que l'arme flotte
   - Mass: 0.5

2. **XR Grab Interactable** (depuis le menu Add Component)
   - Movement Type: `Instantaneous`
   - Track Position: activé
   - Track Rotation: activé

3. **WeaponController** (votre script custom)
   - Max Ammo: 10
   - Fire Rate: 0.5
   - Reload Time: 2
   - Use Raycast: activé (ou désactivé si vous voulez des projectiles)
   - Raycast Max Distance: 100
   - Target Layer: "Target" (vous devrez créer ce layer)

4. **Audio Source** (pour les sons de tir)

#### C. Créer le FirePoint

1. Créez un GameObject vide comme enfant de "Weapon"
2. Nommez-le "FirePoint"
3. Positionnez-le à l'avant du canon
4. Assignez ce FirePoint dans le champ "Fire Point" du WeaponController

#### D. Configurer l'Input Action

Dans le WeaponController, cliquez sur le champ "Shoot Action" :
- Sélectionnez `XRI RightHand Interaction/Activate`

#### E. Ajouter un laser sight (optionnel)

1. Ajoutez un composant **Line Renderer** au GameObject "Weapon"
2. Configurez :
   - Width: 0.01
   - Positions: 2 éléments (0,0,0) et (0,0,1)
   - Material: choisissez un matériau émissif rouge
3. Assignez ce Line Renderer dans le champ "Laser Sight" du WeaponController

### Étape 3 : Créer les cibles

#### A. Créer une cible basique

1. Créez un GameObject avec une forme 3D (ex: `Sphere` ou `Cube`)
2. Nommez-le "Target"
3. Ajoutez un **Collider** (normalement déjà présent)
4. Ajoutez un **Rigidbody** si la cible doit tomber quand touchée (optionnel)
5. Ajoutez le script **Target.cs**
   - Point Value: 10
   - Max Health: 1
   - Is Moving: activez pour une cible mobile
   - Move Speed: 2
   - Move Range: 5

#### B. Créer un Layer pour les cibles

1. Sélectionnez votre cible
2. En haut de l'Inspector, cliquez sur "Layer" > "Add Layer"
3. Créez un layer "Target"
4. Assignez ce layer à votre cible
5. Retournez au WeaponController et assignez ce layer dans "Target Layer"

#### C. Créer des points de spawn

1. Créez plusieurs GameObjects vides dans votre scène
2. Nommez-les "SpawnPoint_1", "SpawnPoint_2", etc.
3. Positionnez-les à différents endroits

#### D. Créer un Prefab de la cible

1. Glissez votre "Target" depuis la Hierarchy vers le dossier `Assets/Prefabs/`
2. Supprimez l'instance de la scène

### Étape 4 : Configurer le GameManager

1. Créez un GameObject vide dans la scène
2. Nommez-le "GameManager"
3. Ajoutez le script **GameManager.cs**
4. Configurez :
   - Game Duration: 60
   - Target Prefabs: Assignez votre/vos prefab(s) de cible
   - Spawn Points: Assignez tous vos SpawnPoints
   - Spawn Interval: 2

### Étape 5 : Créer l'UI

#### A. Menu principal

1. Créez un `Canvas` : `GameObject > UI > Canvas`
2. Configurez le Canvas :
   - Render Mode: `World Space`
   - Width: 1920, Height: 1080
   - Scale: 0.001 pour tous les axes
3. Positionnez le Canvas devant le joueur
4. Créez un panneau "MenuUI" comme enfant
5. Ajoutez un bouton "Start Game"
   - Dans l'événement OnClick(), assignez `GameManager > StartGame()`

#### B. HUD de jeu

1. Créez un autre panneau "GameUI" comme enfant du Canvas
2. Ajoutez des TextMeshProUGUI pour :
   - Score (nommez-le "ScoreText")
   - Timer (nommez-le "TimerText")
3. Assignez ces textes dans le GameManager

#### C. Écran Game Over

1. Créez un panneau "GameOverUI"
2. Ajoutez :
   - Un texte "GAME OVER"
   - Un texte pour le score final (nommez-le "FinalScoreText")
   - Un bouton "Restart" avec OnClick() > `GameManager > RestartGame()`
3. Assignez dans le GameManager

### Étape 6 : Créer le texte flottant (Juice)

1. Créez un GameObject avec un composant **TextMeshPro - Text (3D)**
2. Configurez :
   - Text: "+10"
   - Font Size: 1
   - Alignment: Center
   - Color: Yellow
3. Ajoutez le script **FloatingText.cs**
4. Créez un Prefab dans `Assets/Prefabs/FloatingText`
5. Assignez ce prefab dans le champ "Floating Text Prefab" de vos cibles

### Étape 7 : Ajouter des effets de particules (Juice)

#### A. Effet de destruction de cible

1. Créez un Particle System : `GameObject > Effects > Particle System`
2. Configurez pour une explosion colorée
3. Créez un Prefab "DestructionEffect"
4. Assignez dans les cibles

#### B. Muzzle Flash (éclair de bouche)

1. Créez un Particle System comme enfant de "Weapon"
2. Configurez pour un flash court et lumineux
3. Assignez dans le WeaponController

### Étape 8 : Configuration pour Meta Quest

#### A. Project Settings

1. `Edit > Project Settings > XR Plug-in Management`
2. Activez **Oculus** pour Android
3. Activez **OpenXR** comme alternative

#### B. Build Settings

1. `File > Build Settings`
2. Switch Platform vers **Android**
3. Ajoutez votre scène dans "Scenes In Build"
4. Dans `Player Settings` :
   - Minimum API Level: Android 7.0 (API 24)
   - Install Location: Auto

#### C. Optimisations

1. `Edit > Project Settings > Quality`
   - Texture Quality: Medium
   - Shadow Resolution: Medium
2. Dans vos matériaux, utilisez le shader "Universal Render Pipeline/Lit" ou "Unlit"

## Tester le jeu

### Dans l'éditeur Unity

1. Assurez-vous que le "XR Device Simulator" est activé dans la scène
2. Appuyez sur Play
3. Utilisez la souris et le clavier pour simuler le casque VR

### Sur Meta Quest

1. Activez le mode développeur sur votre Quest
2. Connectez le Quest à votre PC via USB
3. `File > Build And Run`
4. L'APK sera installé et lancé automatiquement

## Fonctionnalités implémentées

- ✅ Arme grabbable avec XR Interaction Toolkit
- ✅ Système de munitions et rechargement
- ✅ Tir par Raycast OU Projectiles (au choix)
- ✅ Cibles destructibles statiques et mobiles
- ✅ Système de points
- ✅ Timer de jeu
- ✅ Interface UI (menu, HUD, game over)
- ✅ Effets visuels (texte flottant, particules)
- ✅ Boucle de gameplay complète

## Prochaines étapes recommandées

1. Ajoutez des assets 3D pour l'arme et les cibles
2. Ajoutez des effets sonores (tir, impact, explosion)
3. Ajoutez de la musique de fond
4. Créez différents types de cibles avec des valeurs de points variées
5. Ajoutez des power-ups
6. Créez un environnement visuel thématique
7. Optimisez les performances pour Quest

## Assets autorisés à utiliser

Comme mentionné dans le sujet, vous pouvez utiliser :
- Modèles 3D gratuits (sketchfab, Unity Asset Store)
- Musiques et SFX (freesound.org, Unity Asset Store)
- Matériaux et textures
- Code des samples XR Interaction Toolkit

N'oubliez pas de répertorier tous les assets utilisés dans votre rendu final !
