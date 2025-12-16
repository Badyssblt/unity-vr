# Guide: AttachTransform pour Arme VR

## 🎯 C'est quoi un AttachTransform ?

Le **AttachTransform** définit **où et comment** l'objet s'attache à la main du joueur quand il le grab.

**Sans AttachTransform** : L'objet s'attache où vous le saisissez (peut être bizarre)
**Avec AttachTransform** : L'objet s'attache toujours au même endroit (comme tenir une vraie arme)

## 📐 Créer un AttachTransform pour votre Pistolet

### Étape 1 : Créer le GameObject AttachTransform

1. Sélectionnez votre **Weapon** dans la Hierarchy
2. **Clic droit** > **Create Empty**
3. Nommez-le **"AttachTransform"**
4. Positionnez-le là où vous voulez que la main tienne l'arme

**Exemple pour un pistolet** :
```
Position: (0, -0.05, 0.1) - Un peu en dessous et en arrière
Rotation: (0, 0, 0) - Ou ajustez selon votre modèle
```

**Astuce** : En Play mode, testez et ajustez jusqu'à ce que ça soit naturel !

### Étape 2 : Assigner l'AttachTransform

1. Sélectionnez votre **Weapon**
2. Dans le composant **XR Grab Interactable**
3. Scrollez jusqu'à **Attach Transform**
4. Glissez votre GameObject **AttachTransform** dans ce champ

### Étape 3 : Tester

1. **Play** ▶️
2. Grabbez l'arme
3. Elle devrait s'aligner parfaitement dans votre main virtuelle !

## 🎨 Visualiser l'AttachTransform (Optionnel)

Pour mieux voir où est le point d'attache en mode Edition :

```csharp
// Collez ce script sur AttachTransform pour le visualiser
using UnityEngine;

public class VisualizeAttachPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.02f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.1f);
    }
}
```

## 📋 Configuration Complète d'une Arme

Voici la hiérarchie typique :

```
Weapon (GameObject)
├── XR Grab Interactable
├── Rigidbody
├── Box Collider
├── WeaponController (ou SimpleGunBehaviour)
├── Disable Collider On Grab
├── Debug Grab
│
├── Visual (Mesh/Modèle 3D)
│   └── Mesh Renderer
│
├── AttachTransform (Empty GameObject) ← POINT D'ATTACHE
│   └── Transform configuré pour la poignée
│
└── FirePoint (Empty GameObject) ← POINT DE TIR
    └── Position au bout du canon
```

## 🔧 Configuration du XR Grab Interactable

Avec AttachTransform configuré :

```
XR Grab Interactable:
✅ Movement Type: Instantaneous
✅ Track Position: ON
✅ Track Rotation: ON
✅ Smooth Position: OFF (pour une réponse instantanée)
✅ Smooth Rotation: OFF
✅ Throw On Detach: OFF (ou ON si vous voulez lancer l'arme)
✅ Attach Transform: [Votre AttachTransform GameObject]
✅ Attach Ease In Time: 0.15 (transition douce)
```

## 🎯 Configurer le Tir avec "Activated"

1. Dans **XR Grab Interactable**
2. Section **Interactable Events**
3. Dépliez **Activated**
4. Cliquez **+**
5. Glissez le **Weapon** GameObject
6. Sélectionnez **WeaponController > Shoot()** (ou SimpleGunBehaviour > Shoot())

## 🎮 Tester dans le XR Device Simulator

```
1. Play ▶️
2. Tab (activer contrôleur droit)
3. Approchez de l'arme avec la souris
4. G ou Clic Droit (grab)
   → L'arme devrait s'aligner parfaitement !
5. Clic Gauche ou Espace (tirer)
   → BANG! 💥
```

## 💡 Conseils pour Positionner l'AttachTransform

### Pour un Pistolet
```
Position: Au milieu de la poignée
Rotation: Aligné avec le canon vers l'avant (forward = direction de tir)
```

### Pour un Fusil/Rifle
```
Créez DEUX AttachTransform:
- AttachTransform_Right (main dominante, sur la gâchette)
- AttachTransform_Left (main de support, sur le devant)
Utilisez XR Grab Interactable avec "Multiple Attach Points"
```

### Pour un Arc
```
Position: Au centre de la poignée de l'arc
Rotation: Perpendiculaire au corps
```

### Pour une Épée
```
Position: Au milieu de la poignée (pas sur la lame!)
Rotation: Lame vers le haut
```

## 🐛 Problèmes Courants

### L'arme est de travers quand je la saisis

**Solution** : Ajustez la **Rotation** de l'AttachTransform

### L'arme est trop loin/proche de la main

**Solution** : Ajustez la **Position** de l'AttachTransform

### L'arme "saute" vers la main bizarrement

**Solution** :
- Réduisez **Attach Ease In Time** à 0
- Ou augmentez-le pour une transition plus douce

### Je veux que l'arme reste où je la saisis

**Solution** :
- Laissez le champ **Attach Transform** vide
- OU décochez **Use Dynamic Attach** (si disponible)

## 📏 Valeurs Recommandées

### Pour un jeu d'arcade (fun, moins réaliste)
```
Attach Transform: Configuré
Attach Ease In Time: 0
Throw On Detach: ON (lancer des armes c'est fun!)
Throw Smoothing Duration: 0.25
```

### Pour un jeu réaliste (simulation)
```
Attach Transform: Configuré précisément
Attach Ease In Time: 0.15
Throw On Detach: OFF
Movement Type: Velocity Tracking (pour physique réaliste)
```

### Pour du prototypage rapide
```
Attach Transform: Pas nécessaire au début
Movement Type: Instantaneous
Track Position/Rotation: ON
```

---

**Résumé** : L'AttachTransform rend votre arme beaucoup plus agréable à utiliser ! Prenez le temps de bien le positionner. 🎯
