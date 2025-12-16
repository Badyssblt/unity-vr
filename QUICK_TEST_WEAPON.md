# Test Rapide de l'Arme - XR Device Simulator

## 🚀 Configuration Rapide (5 minutes)

### Étape 1 : Vérifier XR Origin dans la scène

1. Ouvrez votre scène `Assets/Scenes/SampleScene.unity`
2. Regardez dans la **Hierarchy**
3. Vérifiez qu'il y a :
   - ✅ **XR Origin (XR Rig)** - Si NON, ajoutez-le maintenant :
     - Allez dans `Assets/Samples/XR Interaction Toolkit/2.6.5/Starter Assets/Prefabs/`
     - Glissez **XR Origin (XR Rig).prefab** dans la scène
   - ✅ **XR Interaction Manager** - Si NON :
     - `GameObject > XR > Interaction Manager`

4. Vérifiez qu'il y a **XR Device Simulator** :
   - Si NON, allez dans `Assets/Samples/XR Interaction Toolkit/2.6.5/XR Device Simulator/`
   - Glissez **XR Device Simulator.prefab** dans la scène

### Étape 2 : Créer une arme simple RAPIDEMENT

1. **Créer le GameObject** :
   ```
   - Clic droit dans Hierarchy > Create Empty
   - Nommez-le "Weapon"
   - Position: (0, 1.2, 1) - Devant le joueur
   ```

2. **Ajouter un visuel** :
   ```
   - Clic droit sur "Weapon" > 3D Object > Cube
   - Dans le Cube, Transform > Scale: (0.1, 0.05, 0.3)
   - Dans le Cube, Transform > Position: (0, 0, 0.15)
   ```

3. **Sur le GameObject "Weapon", ajouter les composants** :

   a. **Rigidbody** :
   ```
   - Add Component > Rigidbody
   - Use Gravity: DÉCOCHÉ (unchecked)
   - Is Kinematic: DÉCOCHÉ
   - Mass: 0.5
   - Collision Detection: Continuous
   ```

   b. **Box Collider** (devrait être automatique, sinon) :
   ```
   - Add Component > Box Collider
   - Center: (0, 0, 0.15)
   - Size: (0.1, 0.05, 0.3)
   ```

   c. **XR Grab Interactable** :
   ```
   - Add Component > XR Grab Interactable
   - Dans Interactable Events, vérifiez que tout est là
   - Movement Type: Instantaneous
   - Track Position: ✅ COCHÉ
   - Track Rotation: ✅ COCHÉ
   - Throw On Detach: ❌ DÉCOCHÉ (pour commencer)
   ```

   d. **DebugGrab** (notre script de debug) :
   ```
   - Add Component > Debug Grab
   ```

### Étape 3 : Vérifier le XR Origin

1. Dans la **Hierarchy**, dépliez **XR Origin (XR Rig)**
2. Dépliez **Camera Offset**
3. Vous devriez voir :
   - **LeftHand Controller**
   - **RightHand Controller**

4. Sélectionnez **RightHand Controller** et vérifiez :
   - Il doit avoir un composant **XR Direct Interactor** OU **XR Ray Interactor**
   - Si c'est vide, il faut les ajouter

### Étape 4 : Ajouter les Interactors si manquants

Si RightHand Controller n'a pas d'interactors :

1. Sélectionnez **RightHand Controller**
2. Allez dans `Assets/Samples/XR Interaction Toolkit/2.6.5/Starter Assets/Prefabs/Interactors/`
3. Glissez **Direct Interactor.prefab** comme ENFANT de RightHand Controller

Faites pareil pour **LeftHand Controller**

### Étape 5 : TESTER !

1. **Appuyez sur Play** ▶️

2. **Regardez la Console** - Le script DebugGrab devrait afficher :
   ```
   ✅ XRGrabInteractable trouvé sur Weapon
   ```

3. **Contrôles** :
   - **Tab** = Basculer entre contrôleurs (voyez lequel est actif en haut)
   - **Déplacez la souris** vers l'arme dans la Game View
   - **Approchez-vous** avec WASD si besoin

4. **Console doit afficher** quand vous approchez :
   ```
   🟢 HOVER ENTER - Le contrôleur est proche de l'arme
   ```
   - Si vous voyez ça, c'est bon signe !

5. **Pour GRAB** :
   - **Maintenez Clic Droit** (ou appuyez sur **G**)
   - Console doit afficher :
   ```
   ✅ GRABBED! - L'arme est saisie
   ```

6. **Bougez la souris** - L'arme devrait suivre !

## 🔍 Diagnostic si ça ne marche toujours pas

### Problème 1 : Pas de "HOVER ENTER"

**Cause** : Le contrôleur ne détecte pas l'arme

**Solutions** :
1. Vérifiez que l'arme a un **Collider** (Box Collider)
2. Vérifiez que le Collider n'est pas en mode **Trigger**
3. Vérifiez les **Interaction Layers** :
   - Sur XR Grab Interactable : Interaction Layer Mask = "Default" ✅
   - Sur XR Direct Interactor : Interaction Layer Mask = "Default" ✅

### Problème 2 : HOVER fonctionne mais pas GRAB

**Solutions** :
1. Dans la Console, ouvrez le menu hamburger (3 lignes) et cochez "Collapse"
2. Appuyez sur **G** au lieu de Clic Droit
3. Vérifiez que Use Gravity est DÉCOCHÉ sur le Rigidbody

### Problème 3 : Rien ne s'affiche dans la Console

**Solutions** :
1. Le script DebugGrab n'est pas attaché
2. Relancez Unity
3. Vérifiez qu'il n'y a pas d'erreurs de compilation

## 🎮 Contrôles XR Device Simulator Complets

```
CAMÉRA (Casque VR):
- WASD = Déplacer
- Q/E = Haut/Bas
- Clic Droit + Souris = Regarder autour (maintenir)

CONTRÔLEURS:
- Tab = Basculer Gauche/Droit
- Souris = Position du contrôleur actif
- Scroll = Avant/Arrière
- G = Toggle Grip (GRAB)
- Clic Droit = Grip (maintenir pour grab)
- Clic Gauche = Activate (Trigger)
- Espace = Activate

RACCOURCIS:
- Shift + Clic Gauche = Ray Interactor
- Ctrl + Souris = Rotation du contrôleur
```

## 📋 Checklist de Debug

- [ ] XR Origin (XR Rig) dans la scène
- [ ] XR Interaction Manager dans la scène
- [ ] XR Device Simulator dans la scène
- [ ] Arme a un Rigidbody (Use Gravity = OFF)
- [ ] Arme a un Collider (Is Trigger = OFF)
- [ ] Arme a XR Grab Interactable
- [ ] Arme a DebugGrab script
- [ ] RightHand Controller a un Direct Interactor
- [ ] Console affiche "✅ XRGrabInteractable trouvé"
- [ ] En Play mode, Console affiche "🟢 HOVER" quand proche

## 🆘 Si toujours bloqué

Prenez un screenshot de :
1. La Hierarchy avec XR Origin déplié
2. L'Inspector de votre Weapon
3. L'Inspector du RightHand Controller
4. La Console

Et je pourrai vous aider plus précisément !
