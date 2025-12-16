# Checklist du Projet VR - Jeu de Tir

## Critères du sujet à respecter ✓

### Éléments obligatoires

- [ ] **Cibles** (statiques et/ou volantes)
  - [ ] Faire gagner des points quand touchées
  - [ ] Script `Target.cs` configuré
  - [ ] Prefab(s) de cible créé(s)

- [ ] **Arme grabbable**
  - [ ] Peut être saisie avec les contrôleurs VR
  - [ ] XRGrabInteractable configuré
  - [ ] Pistolet / lance-pierres / arc modélisé

- [ ] **Système de munitions**
  - [ ] Nombre de balles dans le chargeur (affiché)
  - [ ] Système de rechargement fonctionnel
  - [ ] Script `WeaponController.cs` configuré

- [ ] **Système de tir**
  - [ ] Choix fait : Raycast OU Projectile
  - [ ] Détruit les cibles correctement
  - [ ] Script fonctionnel

- [ ] **Minuteur OU système de vies**
  - [ ] Timer affiché dans l'UI
  - [ ] OU : Système de vies si les cibles attaquent
  - [ ] Termine la partie quand temps écoulé / vies = 0

- [ ] **Interface UI**
  - [ ] Menu principal avec bouton Start
  - [ ] HUD en jeu (score, munitions, timer)
  - [ ] Écran Game Over avec score final
  - [ ] Boutons interactifs en VR

- [ ] **Juice (effets visuels/sonores)**
  - [ ] Points qui s'affichent (+10, +20, etc.)
  - [ ] Particules d'explosion/destruction
  - [ ] Effets lumineux (muzzle flash, impacts)
  - [ ] Sons (tir, impact, explosion, musique)

- [ ] **Boucle de gameplay complète**
  - [ ] Démarrer une partie (menu)
  - [ ] Jouer (spawn de cibles, tir, score)
  - [ ] Perdre/Gagner (fin du timer)
  - [ ] Recommencer (bouton restart)

### Bonus

- [ ] Variété des cibles (différents types, points, mouvements)
- [ ] Juice++ (plus d'effets, animations, feedback)
- [ ] Atmosphère (environnement thématique cohérent)

## Configuration technique

### Scène Unity

- [ ] XR Origin (XR Rig) dans la scène
- [ ] XR Interaction Setup dans la scène
- [ ] GameManager configuré
- [ ] Canvas UI en World Space
- [ ] Spawn points positionnés

### Arme

- [ ] Modèle 3D (même simple)
- [ ] Rigidbody
- [ ] XRGrabInteractable
- [ ] WeaponController script
- [ ] FirePoint positionné
- [ ] Input Action configuré (trigger)
- [ ] Audio Source ajouté
- [ ] Laser sight (optionnel mais recommandé)

### Cibles

- [ ] Collider sur les cibles
- [ ] Layer "Target" créé et assigné
- [ ] Script Target.cs configuré
- [ ] Prefabs créés
- [ ] Valeurs de points définies
- [ ] Mouvement configuré (si désiré)

### UI

- [ ] Canvas World Space
- [ ] MenuUI (avec bouton Start)
- [ ] GameUI (score, timer, munitions)
- [ ] GameOverUI (score final, restart)
- [ ] Textes TextMeshProUGUI assignés dans GameManager
- [ ] Événements OnClick configurés

### Effets (Juice)

- [ ] FloatingText prefab créé
- [ ] Particle Systems pour explosions
- [ ] Particle System pour muzzle flash
- [ ] Impact effects configurés
- [ ] Tous assignés dans les scripts

### Audio

- [ ] Sons de tir trouvés/ajoutés
- [ ] Sons d'impact trouvés/ajoutés
- [ ] Sons de destruction trouvés/ajoutés
- [ ] Son de rechargement
- [ ] Musique de fond (optionnelle)
- [ ] Audio Sources configurés

### Build Meta Quest

- [ ] Platform switched to Android
- [ ] Oculus XR Plugin activé
- [ ] Scene ajoutée dans Build Settings
- [ ] Minimum API Level: Android 7.0
- [ ] Test dans le casque réussi
- [ ] Performance fluide (60+ FPS)

## Optimisation

- [ ] Utiliser des modèles low-poly
- [ ] Texture resolution raisonnable
- [ ] URP Shaders (Lit ou Unlit)
- [ ] Pas trop de particules simultanées
- [ ] Occlusion culling configuré
- [ ] Baked lighting (si possible)
- [ ] Object pooling pour les projectiles (avancé)

## Rendu

### Page web à créer

- [ ] Noms des membres du groupe
- [ ] Description du jeu
- [ ] Screenshot du jeu
- [ ] Exemple de code (montrer 1-2 scripts importants)
- [ ] Démo vidéo du jeu (enregistrée sur Quest ou Unity)
- [ ] Screenshot de l'interface Unity
- [ ] Screenshot de la structure du projet
- [ ] Liste des assets utilisés avec sources
- [ ] Lien de téléchargement du fichier .apk

### Rendu intermédiaire (fin novembre)

- [ ] .apk fonctionnel avec prototype
- [ ] Boucle de gameplay basique qui marche
- [ ] Graphics et juice n'ont pas besoin d'être finaux

## Assets à répertorier

Créez une liste de tous les assets utilisés :

```
MODÈLES 3D:
- [Nom] - [Source] - [Lien]

SONS:
- [Nom] - [Source] - [Lien]

MUSIQUE:
- [Nom] - [Source] - [Lien]

TEXTURES/MATÉRIAUX:
- [Nom] - [Source] - [Lien]

CODE:
- XR Interaction Toolkit Samples - Unity Technologies
```

## Resources recommandées

### Modèles 3D gratuits
- Sketchfab (filtre: downloadable, free)
- Unity Asset Store (filtre: free)
- Kenney.nl (assets low-poly)
- Poly Pizza

### Sons gratuits
- Freesound.org
- OpenGameArt.org
- Unity Asset Store (free audio)
- Sonniss GDC bundles

### Outils
- Blender (modélisation 3D)
- Audacity (édition audio)
- OBS Studio (capture vidéo)

## Conseils

1. **Commencez simple** : Faites d'abord marcher le gameplay de base
2. **Testez souvent** : Testez dans le casque régulièrement
3. **Itérez** : Améliorez progressivement le juice et les graphismes
4. **Thème cohérent** : Choisissez un thème et tenez-vous y
5. **Performance d'abord** : Assurez-vous que le jeu tourne à 60+ FPS
6. **Documentation** : Prenez des screenshots et notes en cours de route

## Exemples de thèmes

- Tir spatial (vaisseaux ennemis)
- Carnaval (ballons, cibles)
- Western (bouteilles, cibles de tir)
- Cyber-punk (drones, hologrammes)
- Fantasy (cibles magiques, cristaux)
- Steampunk (engrenages volants)

Bon courage ! 🎮
