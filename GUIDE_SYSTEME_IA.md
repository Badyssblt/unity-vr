# Guide du Système d'IA et Combat VR

## Vue d'ensemble

Votre projet Unity VR dispose maintenant d'un système complet d'IA ennemie avec:
- ✅ Système de points de vie (PV) pour le joueur et les IA
- ✅ IA qui peuvent marcher, patrouiller, poursuivre et attaquer
- ✅ IA qui peuvent tenir une arme et tirer sur le joueur
- ✅ Joueur qui peut tirer sur les IA
- ✅ Système de hitbox (headshots, dégâts différenciés)
- ✅ Système d'équipe (évite le friendly fire)
- ✅ Mort et respawn du joueur

---

## 🎮 Configuration rapide (3 étapes)

### Étape 1: Créer un ennemi IA

**Option A: Avec l'outil automatique**
1. Dans Unity, allez dans `GameObject > AI Setup > Create Enemy AI`
2. Configurez les paramètres dans la fenêtre qui s'ouvre
3. Cliquez sur "Créer l'ennemi IA"

**Option B: Création manuelle**
1. Créez un GameObject vide nommé "Enemy"
2. Ajoutez les composants suivants:
   - `NavMeshAgent`
   - `HealthSystem`
   - `AIController`
   - `AIWeaponHandler`
   - `CapsuleCollider`
   - `Rigidbody` (isKinematic = true)

### Étape 2: Configurer le joueur

Sur votre objet joueur (XR Origin ou Camera Offset):
1. Ajoutez `HealthSystem` (si pas déjà présent)
   - Max Health: 100
   - Team Tag: "Player"
2. Ajoutez `PlayerDeathHandler` (si pas déjà présent)

### Étape 3: Bake le NavMesh

1. Allez dans `Window > AI > Navigation`
2. Dans l'onglet "Bake":
   - Agent Radius: 0.5
   - Agent Height: 2
3. Cliquez sur "Bake"

---

## 📋 Scripts principaux

### 1. HealthSystem
**Fonction:** Gère les points de vie d'une entité (joueur ou IA)

**Paramètres clés:**
- `maxHealth`: PV maximum
- `currentHealth`: PV actuels
- `teamTag`: "Player" ou "Enemy" (pour éviter le friendly fire)

**Événements:**
- `onHealthChanged`: Déclenché quand les PV changent
- `onDamageTaken`: Déclenché quand des dégâts sont reçus
- `onDeath`: Déclenché à la mort

**Méthodes publiques:**
```csharp
TakeDamage(float damage, string attackerTeam)
Heal(float amount)
Revive(float healthAmount)
InstantKill()
```

---

### 2. AIController
**Fonction:** Contrôle le comportement de l'IA (patrouille, poursuite, attaque)

**États de l'IA:**
- `Idle`: Ne fait rien
- `Patrol`: Patrouille dans une zone
- `Chase`: Poursuit le joueur
- `Attack`: Attaque le joueur
- `Dead`: Mort

**Paramètres clés:**
- `player`: Transform du joueur (auto-détecté si tag "Player")
- `detectionRange`: Distance de détection (15m par défaut)
- `attackRange`: Distance d'attaque (10m par défaut)
- `fieldOfView`: Angle de vision (90° par défaut)
- `patrolRadius`: Rayon de patrouille (10m par défaut)
- `attackCooldown`: Temps entre les tirs (1s par défaut)

**Debug:**
- `showDebugGizmos`: Affiche les rayons de détection dans la Scene
- `showDebugLogs`: Affiche les logs de comportement

---

### 3. AIWeaponHandler
**Fonction:** Permet à l'IA d'utiliser une arme (WeaponController)

**Paramètres clés:**
- `weaponController`: Référence au WeaponController
- `accuracy`: Précision de tir (0-1, où 1 = parfait)
- `autoReload`: Recharge automatique
- `unlimitedAmmo`: Munitions infinies

**Méthodes publiques:**
```csharp
Shoot()
CanShoot()
Reload()
```

---

### 4. DamageableHitbox
**Fonction:** Hitbox avec multiplicateur de dégâts (headshots, etc.)

**Types de hitbox:**
- `Head`: x2.0 dégâts (headshot)
- `Body`: x1.0 dégâts (normal)
- `Limb`: x0.75 dégâts (membres)

**Configuration:**
1. Créez des colliders enfants sur votre IA (Head, Body, Legs)
2. Ajoutez `DamageableHitbox` sur chaque collider
3. Assignez le `HealthSystem` du parent

---

### 5. WeaponController
**Fonction:** Contrôle une arme (tir, munitions, rechargement)

**Paramètres clés:**
- `damage`: Dégâts de base (25 par défaut)
- `ownerTeam`: "Player" ou "Enemy"
- `maxAmmo`: Munitions max
- `fireRate`: Cadence de tir
- `useRaycast`: true = hitscan, false = projectiles

**Modes de tir:**
- Raycast (hitscan): Instantané, parfait pour les fusils
- Projectile: Balle physique, parfait pour les lanceurs

---

### 6. PlayerDeathHandler
**Fonction:** Gère la mort et le respawn du joueur

**Paramètres clés:**
- `respawnPoint`: Point de réapparition
- `respawnDelay`: Délai avant respawn (3s par défaut)
- `autoRespawn`: Respawn automatique
- `resetScene`: Recharger la scène au lieu de respawn

---

## 🔧 Configuration détaillée

### Créer un ennemi IA complet

1. **Créer le GameObject principal**
```
Enemy (GameObject)
├─ Visual (Capsule ou modèle 3D)
├─ Weapon (Prefab d'arme)
└─ Hitboxes
   ├─ Head Hitbox (BoxCollider + DamageableHitbox)
   ├─ Body Hitbox (BoxCollider + DamageableHitbox)
   └─ Legs Hitbox (BoxCollider + DamageableHitbox)
```

2. **Composants sur "Enemy":**
   - `NavMeshAgent`
   - `HealthSystem` (teamTag = "Enemy")
   - `AIController`
   - `AIWeaponHandler`
   - `CapsuleCollider` (pour la détection physique)
   - `Rigidbody` (isKinematic = true)

3. **Configuration de l'arme:**
   - Sur le WeaponController de l'arme:
     - `ownerTeam` = "Enemy"
     - Désactiver `XRGrabInteractable` (l'IA n'utilise pas VR)

4. **Configuration AIWeaponHandler:**
   - Assigner le `weaponController`
   - Ajuster `accuracy` (0.7 = 70% de précision)

---

### Configurer le joueur pour recevoir des dégâts

1. **Sur XR Origin (ou Camera Offset):**
   - Ajoutez `HealthSystem`
     - maxHealth: 100
     - teamTag: "Player"
   - Ajoutez `PlayerDeathHandler`
     - autoRespawn: true
     - respawnDelay: 3

2. **Créer un point de respawn:**
   - Créez un GameObject vide "PlayerSpawnPoint"
   - Assignez-le dans `PlayerDeathHandler > respawnPoint`

3. **Collider sur le joueur:**
   - Ajoutez un `CapsuleCollider` sur le XR Origin
     - Height: 1.8
     - Radius: 0.3
     - Center: (0, 0.9, 0)

---

### Configurer une arme pour l'IA

1. **Sur le prefab d'arme:**
   - Assurez-vous qu'il a un `WeaponController`
   - Configurez le `firePoint` (point d'où partent les balles)

2. **Pour une arme de joueur devenant arme d'IA:**
   - Dupliquez le prefab d'arme
   - Sur le `WeaponController`:
     - `ownerTeam` = "Enemy"
   - Désactivez `XRGrabInteractable`

3. **Position de l'arme sur l'IA:**
   - Attachez l'arme comme enfant de l'IA
   - Position approximative: (0.5, 1.5, 0.5)
   - Rotation: (0, 90, 0)
   - Ajustez selon votre modèle 3D

---

## 🎯 Système de dégâts

Le système de dégâts fonctionne en cascade:

1. **Tir avec WeaponController**
   ```csharp
   weaponController.Shoot();
   ```

2. **Détection de la cible**
   - Raycast ou collision de projectile

3. **Recherche des composants (dans l'ordre)**
   - `DamageableHitbox` (priorité: headshots)
   - `HealthSystem` (dégâts directs)
   - `Target` (ancien système, compatibilité)

4. **Application des dégâts**
   ```csharp
   healthSystem.TakeDamage(damage, attackerTeam);
   ```

5. **Vérification de l'équipe**
   - Si attaquant et victime ont le même teamTag → pas de dégâts

6. **Mort**
   - Si health ≤ 0 → événement `onDeath`

---

## 🔍 Système d'équipe (Friendly Fire)

Le système évite le friendly fire automatiquement:

**Tags d'équipe:**
- `"Player"`: Joueur et alliés
- `"Enemy"`: Ennemis
- `""` (vide): Neutre (prend des dégâts de tout le monde)

**Configuration:**
1. Sur `HealthSystem`: définir `teamTag`
2. Sur `WeaponController`: définir `ownerTeam`
3. Sur `BulletProjectile`: appeler `Initialize(damage, ownerTeam)`

**Exemple:**
- Joueur (team "Player") tire sur IA (team "Enemy") → Dégâts
- IA1 (team "Enemy") tire sur IA2 (team "Enemy") → Pas de dégâts
- Joueur (team "Player") tire sur allié (team "Player") → Pas de dégâts

---

## 🐛 Debug et visualisation

### Gizmos dans la Scene View

**AIController** (sélectionnez l'IA):
- 🔵 Cercle bleu: Rayon de détection
- 🔴 Cercle rouge: Rayon d'attaque
- 🟢 Cercle vert: Rayon de patrouille
- 🟡 Cône jaune: Champ de vision
- 🔴 Ligne rouge: Ligne de vue vers le joueur (si visible)

**WeaponController** (sélectionnez l'arme):
- 🔴 Sphère rouge: Position du FirePoint
- 🔵 Flèche cyan: Direction de tir configurée
- 🟣 Ligne magenta: Trajectoire du raycast
- 🟢 Sphère verte: Point d'impact (si collision)

**DamageableHitbox** (sélectionnez une hitbox):
- 🔴 Rouge transparent: Hitbox Head (x2 dégâts)
- 🟡 Jaune transparent: Hitbox Body (x1 dégâts)
- 🟢 Vert transparent: Hitbox Limb (x0.75 dégâts)

### Console Logs

Activez les logs dans l'Inspector:
- `AIController > showDebugLogs`: Comportement de l'IA
- `AIWeaponHandler > showDebugLogs`: Tirs de l'IA
- `HealthSystem > showDebugLogs`: Dégâts et mort
- `WeaponController > showDebugRaycast`: Affichage des gizmos

**Logs typiques:**
```
✅ Enemy_1 a trouvé le joueur: XR Origin
🔄 Enemy_1 : Patrol → Chase
🔫 Enemy_1 attaque le joueur!
💥 Player prend 25 dégâts | PV: 75/100
💀 Player est mort!
✨ Le joueur respawn!
```

---

## ⚙️ Paramètres recommandés

### IA agressive (type "Rusher")
```
AIController:
- detectionRange: 20
- attackRange: 5
- chaseSpeed: 5
- patrolSpeed: 3
- attackCooldown: 0.3

AIWeaponHandler:
- accuracy: 0.6
- autoReload: true
```

### IA défensive (type "Sniper")
```
AIController:
- detectionRange: 30
- attackRange: 25
- chaseSpeed: 2
- patrolSpeed: 1
- attackCooldown: 2

AIWeaponHandler:
- accuracy: 0.9
- autoReload: true
```

### IA équilibrée
```
AIController:
- detectionRange: 15
- attackRange: 10
- chaseSpeed: 3.5
- patrolSpeed: 2
- attackCooldown: 1

AIWeaponHandler:
- accuracy: 0.7
- autoReload: true
```

---

## 🚨 Problèmes courants

### L'IA ne bouge pas
- ✅ Vérifiez que le NavMesh est bake
- ✅ Vérifiez que `NavMeshAgent` est activé
- ✅ Vérifiez que l'IA est sur le NavMesh (doit être bleu)

### L'IA ne détecte pas le joueur
- ✅ Le joueur a-t-il le tag "Player"?
- ✅ Activez `AIController > showDebugGizmos` pour voir le champ de vision
- ✅ Vérifiez `obstacleMask` dans AIController

### L'IA ne tire pas
- ✅ Le `weaponController` est-il assigné dans AIWeaponHandler?
- ✅ L'arme a-t-elle des munitions?
- ✅ Vérifiez `ownerTeam` sur le WeaponController

### Le joueur ne prend pas de dégâts
- ✅ Le joueur a-t-il un `HealthSystem`?
- ✅ Le joueur a-t-il un `Collider`?
- ✅ Vérifiez que `teamTag` est "Player"
- ✅ Vérifiez le `targetLayer` sur le WeaponController

### Friendly fire
- ✅ Vérifiez `teamTag` sur le HealthSystem
- ✅ Vérifiez `ownerTeam` sur le WeaponController
- ✅ Les deux doivent être différents pour infliger des dégâts

### L'IA tire à travers les murs
- ✅ Configurez `obstacleMask` dans AIController
- ✅ Ajoutez les murs au layer approprié

---

## 🎨 Personnalisation avancée

### Créer un nouveau type d'IA

```csharp
// Hériter de AIController
public class BossAI : AIController
{
    protected override void UpdateAttack()
    {
        // Comportement d'attaque personnalisé
        base.UpdateAttack();

        // Ajouter des attaques spéciales
        if (Random.value < 0.1f)
        {
            SpecialAttack();
        }
    }

    void SpecialAttack()
    {
        Debug.Log("💥 Attaque spéciale du boss!");
        // Votre logique ici
    }
}
```

### Ajouter des effets de mort

```csharp
// Dans AIController ou sur un script séparé
void OnDeath()
{
    // Effet de particules
    GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    Destroy(deathEffect, 3f);

    // Lâcher des objets
    DropLoot();

    // Score
    GameManager.Instance.AddScore(100);
}
```

### Système de vagues d'ennemis

```csharp
public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;

    void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
```

---

## 📊 Performance et optimisation

### Optimisation du NavMesh
- Réduire la qualité du bake si beaucoup d'IA
- Utiliser NavMeshLinks pour les zones complexes

### Optimisation des IA
- Limiter le nombre d'IA actives simultanément
- Désactiver l'update des IA hors de vue
- Utiliser des LOD pour les modèles 3D

### Optimisation du système de combat
- Utiliser des object pools pour les projectiles
- Limiter la distance de détection
- Réduire la fréquence d'update (ex: tous les 0.1s au lieu de chaque frame)

---

## 📚 Ressources additionnelles

### Layers recommandés
- **Player**: Joueur et équipement
- **Enemy**: Ennemis et leurs armes
- **Environment**: Murs, obstacles
- **Projectile**: Balles et projectiles

### Tags recommandés
- **Player**: Joueur principal
- **Enemy**: Ennemis
- **Weapon**: Armes

### Collision Matrix (Edit > Project Settings > Physics)
- Player ↔ Enemy: ✅
- Player ↔ Environment: ✅
- Enemy ↔ Environment: ✅
- Projectile ↔ Player: ✅
- Projectile ↔ Enemy: ✅
- Projectile ↔ Projectile: ❌

---

## 🎓 Tutoriel pas-à-pas

### Créer votre première IA ennemie en 5 minutes

1. **Bake le NavMesh**
   - Window > AI > Navigation
   - Bake

2. **Créer l'ennemi**
   - GameObject > AI Setup > Quick Enemy (No Weapon)

3. **Ajouter une arme**
   - Glissez un prefab d'arme comme enfant de l'IA
   - Sur le WeaponController: ownerTeam = "Enemy"
   - Désactivez XRGrabInteractable

4. **Lier l'arme**
   - Sur AIWeaponHandler: assignez weaponController

5. **Configurer le joueur**
   - Ajoutez HealthSystem au XR Origin
   - teamTag = "Player"

6. **Tester**
   - Lancez le jeu
   - Approchez-vous de l'IA
   - Elle devrait vous poursuivre et tirer!

---

## 🆘 Support

Si vous rencontrez des problèmes:
1. Vérifiez la section "Problèmes courants"
2. Activez tous les debug logs
3. Vérifiez les gizmos dans la Scene View
4. Consultez les logs de la Console

**Checklist de démarrage:**
- ✅ NavMesh bake
- ✅ Tag "Player" sur le joueur
- ✅ HealthSystem sur joueur et IA
- ✅ teamTag configurés
- ✅ WeaponController avec ownerTeam correct
- ✅ Colliders sur joueur et IA
- ✅ AIWeaponHandler avec weaponController assigné

---

Bon développement! 🎮
