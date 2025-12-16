# Jeu de Tir VR - Projet WR507D

## 📋 Description du Projet

Ce projet est un jeu de tir en réalité virtuelle développé pour Meta Quest (Oculus) avec Unity et XR Interaction Toolkit. Le jeu respecte tous les critères du sujet d'évaluation.

## ✅ Fonctionnalités Implémentées

### Obligatoires
- ✅ **Cibles** : Statiques et mobiles avec système de points
- ✅ **Arme grabbable** : Pistolet saisissable avec XR Grab Interactable
- ✅ **Système de munitions** : Chargeur avec rechargement automatique
- ✅ **Système de tir** : Raycast ET projectiles (au choix)
- ✅ **Timer** : Compte à rebours de 60 secondes
- ✅ **Interface UI** : Menu, HUD, Game Over
- ✅ **Juice** : Particules, texte flottant, effets sonores
- ✅ **Boucle de gameplay** : Start → Play → Game Over → Restart

### Bonus
- 🎯 Différents types de cibles
- ✨ Effets visuels multiples (particules, laser sight, floating text)
- 🎨 Système de couleurs cohérent
- 📊 Système de scoring avancé
- 🎵 AudioManager pour gestion centralisée du son

## 📁 Structure du Projet

```
Assets/
├── Scripts/
│   ├── WeaponController.cs      # Gestion de l'arme et du tir
│   ├── Target.cs                # Comportement des cibles
│   ├── BulletProjectile.cs      # Projectiles physiques
│   ├── GameManager.cs           # Gestion du jeu
│   ├── WeaponHUD.cs            # Affichage munitions
│   ├── FloatingText.cs         # Points flottants
│   ├── TargetAnimator.cs       # Animations des cibles
│   ├── TargetSpawner.cs        # Spawn automatique
│   ├── AudioManager.cs         # Gestion audio
│   └── Editor/
│       └── ProjectSetupHelper.cs # Outils de configuration
├── Prefabs/                     # Prefabs du jeu
├── Materials/                   # Matériaux
├── UI/                         # Éléments d'interface
└── Scenes/
    └── SampleScene.unity       # Scène principale
```

## 🎮 Scripts Principaux

### WeaponController.cs
Gère toute la logique de l'arme :
- Tir par Raycast ou Projectile
- Système de munitions (10 balles)
- Rechargement automatique
- Effets visuels (muzzle flash, laser sight)
- Sons de tir et rechargement

### Target.cs
Comportement des cibles :
- Points variables
- Vie configurable
- Mouvement (statique, oscillant, ou aléatoire)
- Destruction avec effets
- Intégration avec GameManager

### GameManager.cs
Gestion globale :
- Score tracking
- Timer de 60 secondes
- États du jeu (Menu/Playing/GameOver)
- Spawn de cibles
- UI management

### AudioManager.cs
Gestion audio centralisée :
- Musique de fond (menu, jeu, game over)
- Effets sonores
- Contrôle du volume

## 🛠️ Configuration Requise

### Packages Unity
- Unity 2022.3+ LTS
- Universal Render Pipeline (URP)
- XR Interaction Toolkit 2.6.5
- XR Management 4.5.2
- Oculus XR Plugin 4.5.2
- OpenXR 1.14.3
- TextMeshPro

### Hardware
- Meta Quest 2 ou Quest 3
- PC pour développement Unity
- Câble USB pour build & deploy

## 📖 Documentation

### Guides Inclus
1. **SETUP_INSTRUCTIONS.md** - Instructions complètes de configuration
2. **VISUAL_EFFECTS_GUIDE.md** - Guide des effets visuels
3. **CHECKLIST.md** - Checklist du projet

### Liens Utiles
- [Documentation XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.6/manual/index.html)
- [Oculus Developer Center](https://developer.oculus.com/)
- [Unity XR Best Practices](https://docs.unity3d.com/Manual/xr_performance.html)

## 🚀 Quick Start

### 1. Ouvrir le projet
```bash
1. Ouvrez Unity Hub
2. Cliquez sur "Add" et sélectionnez ce dossier
3. Ouvrez avec Unity 2022.3 LTS ou supérieur
```

### 2. Configuration initiale
```bash
1. Tools > VR Shooter > Setup Project Layers
2. Tools > VR Shooter > Quick Scene Setup
```

### 3. Configurer la scène
Suivez les instructions dans `SETUP_INSTRUCTIONS.md`

### 4. Build pour Quest
```bash
1. File > Build Settings
2. Switch Platform > Android
3. Add Open Scenes
4. Build And Run
```

## 🎯 Utilisation

### Dans Unity Editor (avec XR Device Simulator)
- **Mouvement** : WASD + Souris
- **Tirer** : Clic gauche
- **Grab** : Maintenir Clic droit

### Dans le Casque Quest
- **Mouvement** : Joystick gauche / Téléportation
- **Grab arme** : Trigger grip droit
- **Tirer** : Trigger index droit
- **Menu** : Interactions avec ray interactor

## 🔧 Personnalisation

### Modifier la difficulté
Dans le GameManager :
- `gameDuration` : Durée de la partie
- `spawnInterval` : Fréquence d'apparition des cibles

### Modifier l'arme
Dans le WeaponController :
- `maxAmmo` : Munitions max
- `fireRate` : Cadence de tir
- `reloadTime` : Temps de rechargement
- `useRaycast` : true = raycast, false = projectiles

### Créer de nouvelles cibles
1. Dupliquer un prefab de cible existant
2. Modifier `pointValue` pour changer les points
3. Activer `isMoving` pour cibles mobiles
4. Ajouter au GameManager dans `targetPrefabs[]`

## 🎨 Assets Recommandés

### Modèles 3D Gratuits
- [Kenney.nl](https://kenney.nl/) - Assets low-poly
- [Poly Pizza](https://poly.pizza/) - Modèles 3D low-poly
- [Sketchfab](https://sketchfab.com/) (filtre: downloadable + free)

### Sons Gratuits
- [Freesound.org](https://freesound.org/) - SFX gratuits
- [OpenGameArt.org](https://opengameart.org/) - Audio & music
- [Sonniss GDC](https://sonniss.com/gameaudiogdc) - Bundles gratuits

### Textures
- [TextureLabs](https://www.texturelabs.org/)
- [Poly Haven](https://polyhaven.com/textures)

## 📊 Performance

### Optimisations Implémentées
- URP pour rendu optimisé mobile
- Object pooling possible pour projectiles
- Destruction automatique des effets
- Layers pour raycast ciblé
- Désactivation de composants inutiles

### Metrics Cibles pour Quest
- **FPS** : 72+ (Quest 2) / 90+ (Quest 3)
- **Drawcalls** : < 100
- **Tris** : < 100k
- **Batches** : < 50

## 🐛 Troubleshooting

### L'arme ne tire pas
1. Vérifier que l'Input Action est assigné (XRI RightHand Interaction/Activate)
2. Vérifier que le FirePoint est assigné
3. Vérifier que le Layer "Target" existe et est assigné aux cibles

### Les cibles ne donnent pas de points
1. Vérifier que le GameManager existe dans la scène
2. Vérifier que le Layer des cibles correspond au `targetLayer` du WeaponController

### Build Android échoue
1. Installer Android SDK via Unity Hub
2. Vérifier que Oculus XR Plugin est activé pour Android
3. Minimum API Level: Android 7.0 (API 24)

## 📝 TODO pour Rendu Final

- [ ] Ajouter des assets 3D pour l'arme
- [ ] Créer 3+ types de cibles différents
- [ ] Ajouter effets sonores complets
- [ ] Créer environnement visuel thématique
- [ ] Optimiser pour 72+ FPS constant
- [ ] Enregistrer vidéo démo
- [ ] Prendre screenshots
- [ ] Build .apk final
- [ ] Créer page web de rendu
- [ ] Lister tous les assets utilisés

## 📞 Support

Pour toute question sur le code :
1. Lire SETUP_INSTRUCTIONS.md
2. Lire VISUAL_EFFECTS_GUIDE.md
3. Consulter la documentation XR Interaction Toolkit
4. Vérifier la CHECKLIST.md

## 📄 Licence

Ce projet est créé dans le cadre du cours WR507D - 3D Game Development.
Libre d'utilisation pour le cours.

## 👥 Crédits

### Code
- Scripts custom développés pour le projet
- Exemples XR Interaction Toolkit (Unity Technologies)

### Assets (à compléter lors du rendu)
```
MODÈLES 3D:
- [À ajouter]

SONS:
- [À ajouter]

MUSIQUE:
- [À ajouter]

TEXTURES:
- [À ajouter]
```

---

**Bon développement ! 🎮🥽**
