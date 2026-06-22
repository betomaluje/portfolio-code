# Aurora Genesis - Weapons & Bullets Reference

This document lists all weapons and bullets implemented in the project, including their names, descriptions, and file links.

---

## 🔫 Weapons
These are the weapon components defined under `Assets/Scripts/Weapons/Weapons/`.

### Core Weapons

- **AttackType** (`AttackType.cs`)
  - **Description**: Enum defining the style of weapon attack animations (e.g. Sword, Gun, Gauntlet, Hammer, Throwable, Dagger, Fullscreen).
  - **File Link**: [AttackType.cs](AttackType.cs)

- **BaseShootingWeapon** (`BaseShootingWeapon.cs`)
  - **Description**: Abstract base class for all shooting weapons. Manages ammunition count, reloading mechanisms, shooting points, and spawning of bullets.
  - **File Link**: [BaseShootingWeapon.cs](BaseShootingWeapon.cs)

- **BossWeaponManager** (`BossWeaponManager.cs`)
  - **Description**: Controls boss attacks, weapon cycling, and range-based weapon switching for enemy boss AI.
  - **File Link**: [BossWeaponManager.cs](BossWeaponManager.cs)

- **BurstShootingWeapon** (`BurstShootingWeapon.cs`)
  - **Description**: A shooting weapon that fires multiple projectiles in quick succession with a single attack input. It balances high burst damage with a longer cooldown period.
  - **File Link**: [BurstShootingWeapon.cs](BurstShootingWeapon.cs)

- **ChargedWeapon** (`ChargedWeapon.cs`)
  - **Description**: A shooting weapon that modifies its attack characteristics based on a charge duration. Implements ICharge to receive the player's charging input. High charge can mean more damage, faster projectiles, or larger bullets.
  - **File Link**: [ChargedWeapon.cs](ChargedWeapon.cs)

- **CircleMeleeWeapon** (`CircleMeleeWeapon.cs`)
  - **Description**: A melee weapon that performs an area-of-effect (AOE) circular attack centered on the player. It hits all enemies in every direction within a specific radius. Perfect for heavy "Ground Slam" or "Whirlwind" animations.
  - **File Link**: [CircleMeleeWeapon.cs](CircleMeleeWeapon.cs)

- **CompanionWeaponManager** (`CompanionWeaponManager.cs`)
  - **Description**: Manages weapon registration, aiming, and attack execution for companion characters.
  - **File Link**: [CompanionWeaponManager.cs](CompanionWeaponManager.cs)

- **DashMeleeWeapon** (`DashMeleeWeapon.cs`)
  - **Description**: A localized melee weapon that performs a quick forward "dash" when attacking. This helps the player close distances and strike enemies at once.
  - **File Link**: [DashMeleeWeapon.cs](DashMeleeWeapon.cs)

- **EnemyWeaponManager** (`EnemyWeaponManager.cs`)
  - **Description**: Handles weapon selection, execution, and attack collider adjustments for standard enemy AI.
  - **File Link**: [EnemyWeaponManager.cs](EnemyWeaponManager.cs)

- **ICharge** (`ICharge.cs`)
  - **Description**: Interface implemented by weapons that support a hold-to-charge mechanism before attacking.
  - **File Link**: [ICharge.cs](ICharge.cs)

- **IWeaponCollider** (`IWeaponCollider.cs`)
  - **Description**: Interface defining the offset and size properties for melee or area weapon attack colliders.
  - **File Link**: [IWeaponCollider.cs](IWeaponCollider.cs)

- **IWeaponManager** (`IWeaponManager.cs`)
  - **Description**: Interface for component managers that control attacking, weapon changing, and equipping on characters.
  - **File Link**: [IWeaponManager.cs](IWeaponManager.cs)

- **MeleeWeapon** (`MeleeWeapon.cs`)
  - **Description**: A basic melee weapon that triggers a localized swing attack.
  - **File Link**: [MeleeWeapon.cs](MeleeWeapon.cs)

- **MultipleSpreadShootingWeapon** (`MultipleSpreadShootingWeapon.cs`)
  - **Description**: Fired weapon that shoots multiple projectiles at once with an angular spread, with optional delays or customized patterns.
  - **File Link**: [MultipleSpreadShootingWeapon.cs](MultipleSpreadShootingWeapon.cs)

- **OrbitingWeapon** (`OrbitingWeapon.cs`)
  - **Description**: A weapon that spawns orbiting projectiles to protect the player.
  - **File Link**: [OrbitingWeapon.cs](OrbitingWeapon.cs)

- **PlaceTrapWeapon** (`PlaceTrapWeapon.cs`)
  - **Description**: A shooting weapon that deploys stationary trap objects in the world.
  - **File Link**: [PlaceTrapWeapon.cs](PlaceTrapWeapon.cs)

- **RecoilWeapon** (`RecoilWeapon.cs`)
  - **Description**: A high-powered shooting weapon that applies a reactive force to the shooter upon firing. This "recoil" push can be used for mobility, repositioning, or just as a drawback.
  - **File Link**: [RecoilWeapon.cs](RecoilWeapon.cs)

- **RequiredBulletAttribute** (`RequiredBulletAttribute.cs`)
  - **Description**: An attribute used to specify which specialized bullet script a weapon requires. This is used by the Weapon Creator Wizard to automate bullet generation.
  - **File Link**: [RequiredBulletAttribute.cs](RequiredBulletAttribute.cs)

- **SentryWeapon** (`SentryWeapon.cs`)
  - **Description**: A weapon that deploys an automated Sentry Turret.
  - **File Link**: [SentryWeapon.cs](SentryWeapon.cs)

- **ShootingWeapon** (`ShootingWeapon.cs`)
  - **Description**: Concrete shooting weapon that spawns a bullet prefab in the direction of the target.
  - **File Link**: [ShootingWeapon.cs](ShootingWeapon.cs)

- **SpawnObjectWeapon** (`SpawnObjectWeapon.cs`)
  - **Description**: Spawns static or moving objects in the world, commonly used by bosses to summon barriers or minions.
  - **File Link**: [SpawnObjectWeapon.cs](SpawnObjectWeapon.cs)

- **SpreadShootWithMeleeWeapon** (`SpreadShootWithMeleeWeapon.cs`)
  - **Description**: Hybrid weapon that executes a melee swing while simultaneously shooting a spread of projectiles.
  - **File Link**: [SpreadShootWithMeleeWeapon.cs](SpreadShootWithMeleeWeapon.cs)

- **SpreadShootingWeapon** (`SpreadShootingWeapon.cs`)
  - **Description**: Shoots a fan or spread of bullets simultaneously in multiple directions.
  - **File Link**: [SpreadShootingWeapon.cs](SpreadShootingWeapon.cs)

- **StatusWeapon** (`StatusWeapon.cs`)
  - **Description**: A localized weapon that doesn't just deal direct damage, but also applies a specialized status effect (PowerupConfig) to the hit victim. This could be a "Freeze Gun", "Poison Blade", or "Debuff Curse".
  - **File Link**: [StatusWeapon.cs](StatusWeapon.cs)

- **VampiricMeleeWeapon** (`VampiricMeleeWeapon.cs`)
  - **Description**: A melee weapon that heals the user for a percentage of the damage dealt. It hooks into the global damage events to detect when it has successfully landed a hit.
  - **File Link**: [VampiricMeleeWeapon.cs](VampiricMeleeWeapon.cs)

- **Weapon** (`Weapon.cs`)
  - **Description**: Abstract base ScriptableObject class representing a weapon, holding configuration values like damage, range, cooldown, and modifiers.
  - **File Link**: [Weapon.cs](Weapon.cs)

- **WeaponEquip** (`WeaponEquip.cs`)
  - **Description**: ScriptableObject container that stores starting weapons for a character's initial loadout.
  - **File Link**: [WeaponEquip.cs](WeaponEquip.cs)

- **WeaponManager** (`WeaponManager.cs`)
  - **Description**: Centralized weapon management component for the player, handling inputs, switching weapons, reloading, and feeding stats.
  - **File Link**: [WeaponManager.cs](WeaponManager.cs)

- **WeaponSprite** (`WeaponSprite.cs`)
  - **Description**: Component holding visual sprite configurations (direction, flip, offset) for rendering weapons on characters.
  - **File Link**: [WeaponSprite.cs](WeaponSprite.cs)

### DesignEx Weapons

- **BindingGrimoire** (`BindingGrimoire.cs`)
  - **Description**: A magical grimoire that fires binding seals. Mechanics: Projectile pins a target. While pinned, a field is created that slows and potentially binds other nearby enemies.
  - **File Link**: [BindingGrimoire.cs](DesignEx/BindingGrimoire.cs)

- **BlackHoleGun** (`BlackHoleGun.cs`)
  - **Description**: A high-tech black hole gun, inspired by Enter the Gungeon. Mechanics: Fires a massive, slow-moving black hole that attracts enemies and projectiles in a radius. Great for grouping targets.
  - **File Link**: [BlackHoleGun.cs](DesignEx/BlackHoleGun.cs)

- **CaseyBatReflector** (`CaseyBatReflector.cs`)
  - **Description**: A heavy metal bat inspired by Casey from Enter the Gungeon. Mechanics: Slow swing, massive knockback. If it hits an enemy projectile (IBullet), it reflects it back towards the aim direction.
  - **File Link**: [CaseyBatReflector.cs](DesignEx/CaseyBatReflector.cs)

- **ChargeSurgeLanceWeapon** (`ChargeSurgeLanceWeapon.cs`)
  - **Description**: A melee weapon utilizing ICharge. Unlike standard ChargedWeapon (which shoots big bullets), this weapon scales a physical lunge and melee cleave based on charge time.
  - **File Link**: [ChargeSurgeLanceWeapon.cs](DesignEx/ChargeSurgeLanceWeapon.cs)

- **ChargedMeleeWeapon** (`ChargedMeleeWeapon.cs`)
  - **Description**: Base configuration for committal melee weapons that require a charge-up phase before executing. Useful for Lances, Heavy Hammers, or Big Beam Cannons. Scales damage based on how long the attack was held.
  - **File Link**: [ChargedMeleeWeapon.cs](DesignEx/ChargedMeleeWeapon.cs)

- **CursedBindingChain** (`CursedBindingChain.cs`)
  - **Description**: Configuration for a binding weapon that roots enemies in place. Mastery: Hitting an already bound enemy refreshes the stun duration and adds a high-damage "Blight" explosion.
  - **File Link**: [CursedBindingChain.cs](DesignEx/CursedBindingChain.cs)

- **DashSlashKataWeapon** (`DashSlashKataWeapon.cs`)
  - **Description**: Uniquely implements an internal "Stamina/Charge" model inside a Melee Weapon. Provides 3 rapid dashes that slowly recharge, completely distinct from standard DashMeleeWeapon cooldowns.
  - **File Link**: [DashSlashKataWeapon.cs](DesignEx/DashSlashKataWeapon.cs)

- **DrillBitRailgun** (`DrillBitRailgun.cs`)
  - **Description**: Configuration for a linear pierce weapon. The bullet speeds up and deals more damage for every enemy it penetrates.
  - **File Link**: [DrillBitRailgun.cs](DesignEx/DrillBitRailgun.cs)

- **FestiveFogLauncher** (`FestiveFogLauncher.cs`)
  - **Description**: A lobbing weapon that creates a "Festive Fog" area. Enemies inside the fog are intoxicated (slowed) and take damage over time. Lobs a slow-moving, arcing bottle that shatters on impact. It releases a lingering toxin fog that persists for 5 seconds. Every half-second, it deals tick damage to all enemies within its 4-meter radius
  - **File Link**: [FestiveFogLauncher.cs](DesignEx/FestiveFogLauncher.cs)

- **FrostScytheWeapon** (`FrostScytheWeapon.cs`)
  - **Description**: A heavy scythe inspired by Ember Knights. Mechanics: Slow, wide swings. Every 3rd hit triggers a Frost Nova that slows enemies and makes them vulnerable.
  - **File Link**: [FrostScytheWeapon.cs](DesignEx/FrostScytheWeapon.cs)

- **GravityImpactHammer** (`GravityImpactHammer.cs`)
  - **Description**: A heavy melee weapon with massive knockback. Mastery: Smashing enemies into walls triggers "Collision Damage" which far exceeds base damage.
  - **File Link**: [GravityImpactHammer.cs](DesignEx/GravityImpactHammer.cs)

- **GunderfuryRifle** (`GunderfuryRifle.cs`)
  - **Description**: A fast-firing carbine inspired by Gunderfury and WoW artifacts. Mechanics: Every 5th bullet is a "Lightning Bolt" that chains between up to 3 nearby enemies.
  - **File Link**: [GunderfuryRifle.cs](DesignEx/GunderfuryRifle.cs)

- **IronSandAegisWeapon** (`IronSandAegisWeapon.cs`)
  - **Description**: A defensive/offensive manipulation tool. Spawns a floating iron-sand cloud that protects the player. Mastery: Using ICharge (Holding Attack) consumes the sand to fire a massive spear. This rewards players for choosing the perfect timing to switch from defense to offense.
  - **File Link**: [IronSandAegisWeapon.cs](DesignEx/IronSandAegisWeapon.cs)

- **LinkSpearWeapon** (`LinkSpearWeapon.cs`)
  - **Description**: A spear that sticks to walls and sustains a damaging electric beam back to the owner. Mastery: Moving the player "sweeps" the beam through enemies to clothesline them. Recasting the attack while a spear is lodged will recall it.
  - **File Link**: [LinkSpearWeapon.cs](DesignEx/LinkSpearWeapon.cs)

- **LivingHiveGun** (`LivingHiveGun.cs`)
  - **Description**: Configuration for a Living Swarm weapon. Fires Stinger Bits that aren't linear bullets; they are small flocking entities. Mastery: Holding ICharge concentration pheromones on the aim point. Releasing ICharge returns them to a "Scatter-Shield" cloud around the player.
  - **File Link**: [LivingHiveGun.cs](DesignEx/LivingHiveGun.cs)

- **MagneticShrapnelCannon** (`MagneticShrapnelCannon.cs`)
  - **Description**: A cannon that fires metal shrapnel. Mastery: Using ICharge (Holding Attack) activates the magnetic field. All active shards in the scene will rapidly return to the player, ripping through enemies along the way.
  - **File Link**: [MagneticShrapnelCannon.cs](DesignEx/MagneticShrapnelCannon.cs)

- **MarkDetonatePistolWeapon** (`MarkDetonatePistolWeapon.cs`)
  - **Description**: A weapon where primary fire applies a setup marker, and a secondary mechanic executes the explosive payoff.
  - **File Link**: [MarkDetonatePistolWeapon.cs](DesignEx/MarkDetonatePistolWeapon.cs)

- **MirrorRefractionWeapon** (`MirrorRefractionWeapon.cs`)
  - **Description**: A sword that creates ghost-like mirror images in a fan. Mechanics: Attacking with a full charge (ICharge) creates mirror copies that mirror the player's position and strike.
  - **File Link**: [MirrorRefractionWeapon.cs](DesignEx/MirrorRefractionWeapon.cs)

- **MissilePodLauncher** (`MissilePodLauncher.cs`)
  - **Description**: A shoulder-mounted missile pod inspired by Enter the Gungeon. Mechanics: Rapidly fires 8 small homing missiles in a wider arc. Missiles actively steer towards the nearest enemy in their detection cone.
  - **File Link**: [MissilePodLauncher.cs](DesignEx/MissilePodLauncher.cs)

- **MjolnirHammerWeapon** (`MjolnirHammerWeapon.cs`)
  - **Description**: A throwing hammer that returns to the owner. Mechanics: Thrown at high speed. While returning, it connects a lightning chain to the owner, damaging anything caught in the return path.
  - **File Link**: [MjolnirHammerWeapon.cs](DesignEx/MjolnirHammerWeapon.cs)

- **PhoenixStaffWeapon** (`PhoenixStaffWeapon.cs`)
  - **Description**: A phoenix staff that fires fireballs and a giant returning phoenix. Mechanics: Fires small fireballs (BulletPrefab). If you hold the fire button (Charge, ICharge), it fires the Phoenix that travels through enemies and returns, healing the player on return.
  - **File Link**: [PhoenixStaffWeapon.cs](DesignEx/PhoenixStaffWeapon.cs)

- **ResonanceEchoShotgun** (`ResonanceEchoShotgun.cs`)
  - **Description**: *No description available.*
  - **File Link**: [ResonanceEchoShotgun.cs](DesignEx/ResonanceEchoShotgun.cs)

- **RicochetChakramWeapon** (`RicochetChakramWeapon.cs`)
  - **Description**: A localized weapon that resets its own cooldown instantly if the player catches the returning projectile. Features unique return-trip logic via the custom Chakram bullet.
  - **File Link**: [RicochetChakramWeapon.cs](DesignEx/RicochetChakramWeapon.cs)

- **ScatterBurstShotgunWeapon** (`ScatterBurstShotgunWeapon.cs`)
  - **Description**: A shotgun-style weapon that fires multiple projectiles in a cone spread simultaneously.
  - **File Link**: [ScatterBurstShotgunWeapon.cs](DesignEx/ScatterBurstShotgunWeapon.cs)

- **ShatterPointRapier** (`ShatterPointRapier.cs`)
  - **Description**: Configuration for a delayed execution melee weapon. Melee hits apply "Shatter Points" instead of dealing immediate damage. Mastery: Waiting 1.5s after the last hit triggers a massive detonation.
  - **File Link**: [ShatterPointRapier.cs](DesignEx/ShatterPointRapier.cs)

- **SolarFlareBitWeapon** (`SolarFlareBitWeapon.cs`)
  - **Description**: Configuration for an Orbital Beacon Bit weapon. Spawns a drone that orbits the character. Mastery: Using ICharge (Holding Attack) cinches the bit closer to the player.
  - **File Link**: [SolarFlareBitWeapon.cs](DesignEx/SolarFlareBitWeapon.cs)

- **SpiritSlashKatana** (`SpiritSlashKatana.cs`)
  - **Description**: A hybrid katana that launches crescent-shaped spirit waves on every swing. Mastery: "True Strike" bonus—hitting an enemy with both the physical blade AND the projectile at close range deals massive combined damage.
  - **File Link**: [SpiritSlashKatana.cs](DesignEx/SpiritSlashKatana.cs)

- **StarRailgun** (`StarRailgun.cs`)
  - **Description**: A high-precision railgun that fires stellar beams. Mechanics: Instant hit along a beam path (Raycast-based). Penetrates all targets. If an enemy dies from the beam, they explode into a Supernova.
  - **File Link**: [StarRailgun.cs](DesignEx/StarRailgun.cs)

- **TetherHarpoonWeapon** (`TetherHarpoonWeapon.cs`)
  - **Description**: A localized weapon that shoots a harpoon. If an active tether exists, recasting will pull the target instead of shooting.
  - **File Link**: [TetherHarpoonWeapon.cs](DesignEx/TetherHarpoonWeapon.cs)

- **ThunderclapFlashWeapon** (`ThunderclapFlashWeapon.cs`)
  - **Description**: A high-speed dash katana inspired by "Thunderclap and Flash". Mechanics: Charge attack (ICharge). On release, player instantly teleports/dashes to max range. All enemies in the trail take crit damage.
  - **File Link**: [ThunderclapFlashWeapon.cs](DesignEx/ThunderclapFlashWeapon.cs)

- **DragonBurstGauntletWeapon** (`DragonBurstGauntletWeapon.cs`)
  - **Description**: Rapid hand-to-hand gauntlet inspired by Goku's Dragon Fist (Dragon Ball GT) and MHA's Detroit Smash. Mechanics: Each hit builds a "Ki Stack" (up to a configurable max). At max stacks, the next attack detonates a massive golden energy burst [DragonBurstExplosionBullet](../Bullets/DesignEx/DragonBurstExplosionBullet.cs) centered on the last hit enemy, dealing heavy AOE damage. Requires a DragonBurstExplosionBullet prefab.
  - **File Link**: [DragonBurstGauntletWeapon.cs](DesignEx/DragonBurstGauntletWeapon.cs)

- **GetsugaKatanaWeapon** (`GetsugaKatanaWeapon.cs`)
  - **Description**: An evolving katana inspired by Ichigo's Zangetsu / Getsuga Tenshō (Bleach). Mechanics: Every 3rd standard swing fires a CrescentWaveBullet. Holding ICharge then releasing fires a massive, slow, dark GetsugaHeavyWaveBullet that pierces many targets and deals greatly amplified damage. Requires both a StandardWavePrefab [CrescentWaveBullet](../Bullets/DesignEx/CrescentWaveBullet.cs) and a HeavyWavePrefab [GetsugaHeavyWaveBullet](../Bullets/DesignEx/GetsugaHeavyWaveBullet.cs).
  - **File Link**: [GetsugaKatanaWeapon.cs](DesignEx/GetsugaKatanaWeapon.cs)

- **RaikiriLightningBladeWeapon** (`RaikiriLightningBladeWeapon.cs`)
  - **Description**: Concentrates lightning into the palm, then releases it as a high-speed dashing strike. Inspired by Kakashi's Raikiri / Sasuke's Chidori (Naruto). Mechanics: Charging forces a hold. On release, the player dashes at full speed through enemies, teleporting forward. The first enemy struck is electrocuted and a LightningChainBullet is spawned to arc the lightning to adjacent targets. Inherits from DashMeleeWeapon, implements ICharge. Requires a [LightningChainBullet](../Bullets/DesignEx/LightningChainBullet.cs) prefab.
  - **File Link**: [RaikiriLightningBladeWeapon.cs](DesignEx/RaikiriLightningBladeWeapon.cs)

- **RuyiStaffWeapon** (`RuyiStaffWeapon.cs`)
  - **Description**: An extending magical staff inspired by Goku's Nyoi-bō / Sun Wukong's Ruyi Jingu Bang. Mechanics: Implements both ICharge and IWeaponCollider — the attack BoxCollider2D dynamically scales its size and offset based on charge level, making the staff physically reach further. Enemies struck at the tip of the staff (>60% charge) receive a critical damage bonus.
  - **File Link**: [RuyiStaffWeapon.cs](DesignEx/RuyiStaffWeapon.cs)

- **SpiritBladeWeapon** (`SpiritBladeWeapon.cs`)
  - **Description**: A blade of pure spirit energy inspired by Goku Black's Azure Slicer / Vegito's Spirit Sword (Dragon Ball Super) and Kuwabara's Spirit Sword (YuYu Hakusho). Mechanics: A quick tap performs a standard melee swing. Holding ICharge then releasing fires a piercing SpiritBladeBullet beam that grows in length, size, and damage based on charge duration. Requires a [SpiritBladeBullet](../Bullets/DesignEx/SpiritBladeBullet.cs) prefab.
  - **File Link**: [SpiritBladeWeapon.cs](DesignEx/SpiritBladeWeapon.cs)

- **BerserkerFrenzyAxeWeapon** (`BerserkerFrenzyAxeWeapon.cs`)
  - **Description**: A massive axe inspired by Guts from Berserk. Mechanics: Each confirmed enemy kill grants a Frenzy stack (up to a configurable max). Each stack additively increases GetDamage(). Stacks start decaying after a kill-window of inactivity expires, losing one stack per _decayRateSeconds. The longer and faster you kill, the more dangerous you become.
  - **File Link**: [BerserkerFrenzyAxeWeapon.cs](DesignEx/BerserkerFrenzyAxeWeapon.cs)

- **ConquerorDomainStrikeWeapon** (`ConquerorDomainStrikeWeapon.cs`)
  - **Description**: An omnidirectional shockwave strike inspired by Conqueror's Haki — Haōshoku (One Piece). Mechanics: ShouldMoveAttackCollider = false. On attack, OverlapCircleAll in a wide radius. Damage is proximity-scaled: enemies at the epicenter take full damage, those at the edge take minimum. All enemies receive a radial Rigidbody2D knockback impulse.
  - **File Link**: [ConquerorDomainStrikeWeapon.cs](DesignEx/ConquerorDomainStrikeWeapon.cs)

- **CounterStrikeBladeWeapon** (`CounterStrikeBladeWeapon.cs`)
  - **Description**: A parry-and-punish blade inspired by Muichiro / Genya counter techniques (Demon Slayer) and Vergil's Royal Guard (DMC). Mechanics: First press enters a Guard window. If the player is hit during this window (detected via CharacterHealth.OnAnyDamagePerformed on the owner transform), the absorbed damage charges a massive counter-strike with a guaranteed critical hit and damage scaling. Counter window expires after a set time.
  - **File Link**: [CounterStrikeBladeWeapon.cs](DesignEx/CounterStrikeBladeWeapon.cs)

- **HakiArmamentGauntletWeapon** (`HakiArmamentGauntletWeapon.cs`)
  - **Description**: A gauntlet imbued with Armament Haki inspired by Luffy's Gear 4 and Zoro (One Piece). Mechanics: Internally cycles between a Haki-ON phase (massively boosted GetDamage() and extended AttackSize) and a Haki-OFF recharge phase. The weapon is always usable — the player must learn the phase rhythm to land big hits in the empowered window.
  - **File Link**: [HakiArmamentGauntletWeapon.cs](DesignEx/HakiArmamentGauntletWeapon.cs)

- **HinokamiWhistlingFlameWeapon** (`HinokamiWhistlingFlameWeapon.cs`)
  - **Description**: A spinning flame dance inspired by Tanjiro's Hinokami Kagura (Demon Slayer). Mechanics: Radial attack (ShouldMoveAttackCollider = false). Before the spin animation lands, uses OverlapCircleAll + Rigidbody2D.AddForce to magnetize all nearby enemies inward, ensuring the full ring of damage connects. ICharge scales the pull radius and damage multiplier.
  - **File Link**: [HinokamiWhistlingFlameWeapon.cs](DesignEx/HinokamiWhistlingFlameWeapon.cs)

- **ThunderStepWeapon** (`ThunderStepWeapon.cs`)
  - **Description**: A single-target precision strike inspired by Zenitsu's Thunder Breathing — Thunderclap and Flash (Demon Slayer). Mechanics: Extends DashMeleeWeapon. Quick tap fires a short lunge. Holding ICharge then releasing raycasts to the first enemy in aim direction, teleports the player directly beside them, and delivers a massive damage hit with a crit chance that scales to 100% at full charge.
  - **File Link**: [ThunderStepWeapon.cs](DesignEx/ThunderStepWeapon.cs)

- **WeaponDebugger** (`WeaponDebugger.cs`)
  - **Description**: A simple diagnostic component to test new weapon behaviors in the editor. Attach this to a GameObject in your scene to simulate weapon firing.
  - **File Link**: [WeaponDebugger.cs](DesignEx/WeaponDebugger.cs)

- **WormholePortalGun** (`WormholePortalGun.cs`)
  - **Description**: A tactical tool weapon that fires two linked portals. Mastery: Teleports any projectile (friendly or hostile) between the two portals. Skill is using it to redirect boss projectiles or extend your own range.
  - **File Link**: [WormholePortalGun.cs](DesignEx/WormholePortalGun.cs)

### Spawn Weapons Weapons

- **BaseSpawnWeapon** (`BaseSpawnWeapon.cs`)
  - **Description**: Abstract base class for weapons that spawn complex external behaviors (like beams or fullscreen hazards).
  - **File Link**: [BaseSpawnWeapon.cs](Spawn Weapons/BaseSpawnWeapon.cs)

- **BossBeamWeapon** (`BossBeamWeapon.cs`)
  - **Description**: Weapon that spawns a sweeping or static beam hazard centered on the boss.
  - **File Link**: [BossBeamWeapon.cs](Spawn Weapons/BossBeamWeapon.cs)

- **BossFullscreenConfig** (`BossFullscreenConfig.cs`)
  - **Description**: Configuration settings for fullscreen boss room hazards.
  - **File Link**: [BossFullscreenConfig.cs](Spawn Weapons/BossFullscreenConfig.cs)

- **FullscreenBossWeapon** (`FullscreenBossWeapon.cs`)
  - **Description**: Weapon that triggers room-wide fullscreen hazards or attacks.
  - **File Link**: [FullscreenBossWeapon.cs](Spawn Weapons/FullscreenBossWeapon.cs)

- **ISpawnedWeapon** (`ISpawnedWeapon.cs`)
  - **Description**: Interface representing an object spawned by a weapon that can have its parameters initialized.
  - **File Link**: [ISpawnedWeapon.cs](Spawn Weapons/ISpawnedWeapon.cs)

- **StraightSpawnWeapon** (`StraightSpawnWeapon.cs`)
  - **Description**: Spawns a straight-moving combat hazard or obstacle.
  - **File Link**: [StraightSpawnWeapon.cs](Spawn Weapons/StraightSpawnWeapon.cs)

### Spawn Weapons/Movements Weapons

- **FollowTargetMovementConfig** (`FollowTargetMovementConfig.cs`)
  - **Description**: Movement profile for spawned objects to actively steer and follow a target.
  - **File Link**: [FollowTargetMovementConfig.cs](Spawn Weapons/Movements/FollowTargetMovementConfig.cs)

- **MovementConfig** (`MovementConfig.cs`)
  - **Description**: Abstract ScriptableObject representing movement rules for spawned objects.
  - **File Link**: [MovementConfig.cs](Spawn Weapons/Movements/MovementConfig.cs)

- **StraightMovementConfig** (`StraightMovementConfig.cs`)
  - **Description**: Movement profile representing straight linear motion for spawned hazards.
  - **File Link**: [StraightMovementConfig.cs](Spawn Weapons/Movements/StraightMovementConfig.cs)

### Throwable Weapons

- **IThrowable** (`IThrowable.cs`)
  - **Description**: Interface implemented by throwable projectiles.
  - **File Link**: [IThrowable.cs](Throwable/IThrowable.cs)

- **ThrowablePath** (`ThrowablePath.cs`)
  - **Description**: Abstract class for defining trajectory curves of throwable objects.
  - **File Link**: [ThrowablePath.cs](Throwable/ThrowablePath.cs)

- **ThrowableProjectile** (`ThrowableProjectile.cs`)
  - **Description**: Projectile component that follows a calculated path/curve (like an arc) when thrown.
  - **File Link**: [ThrowableProjectile.cs](Throwable/ThrowableProjectile.cs)

- **ThrowableWeapon** (`ThrowableWeapon.cs`)
  - **Description**: Weapon that launches throwable projectiles following custom path trajectories.
  - **File Link**: [ThrowableWeapon.cs](Throwable/ThrowableWeapon.cs)

### Throwable/Paths Weapons

- **ArcPath** (`ArcPath.cs`)
  - **Description**: Trajectory path following a parabolic arc curve.
  - **File Link**: [ArcPath.cs](Throwable/Paths/ArcPath.cs)

- **BackAndForthPath** (`BackAndForthPath.cs`)
  - **Description**: Trajectory path where the projectile travels forward and then loops back.
  - **File Link**: [BackAndForthPath.cs](Throwable/Paths/BackAndForthPath.cs)

- **IRangeLimitable** (`IRangeLimitable.cs`)
  - **Description**: Interface for paths or projectiles with max range bounds.
  - **File Link**: [IRangeLimitable.cs](Throwable/Paths/IRangeLimitable.cs)

- **IThrowablePath** (`IThrowablePath.cs`)
  - **Description**: Interface representing path calculation for throwables.
  - **File Link**: [IThrowablePath.cs](Throwable/Paths/IThrowablePath.cs)

- **LinearPath** (`LinearPath.cs`)
  - **Description**: Straight linear path for throwables.
  - **File Link**: [LinearPath.cs](Throwable/Paths/LinearPath.cs)

- **ParabolicPath** (`ParabolicPath.cs`)
  - **Description**: Parabolic curved path for throwables.
  - **File Link**: [ParabolicPath.cs](Throwable/Paths/ParabolicPath.cs)

- **SineWavePath** (`SineWavePath.cs`)
  - **Description**: Sinusoidal wave path for throwables.
  - **File Link**: [SineWavePath.cs](Throwable/Paths/SineWavePath.cs)


---

## ☄️ Bullets / Projectiles
These are the bullet and projectile components defined under `Assets/Scripts/Weapons/Bullets/`.

### Core Bullets

- **AutoDetachBullet** (`AutoDetachBullet.cs`)
  - **Description**: Bullet component that detaches its children (like particle trails) upon destruction for clean visual decay.
  - **File Link**: [AutoDetachBullet.cs](../Bullets/AutoDetachBullet.cs)

- **BaseBullet** (`BaseBullet.cs`)
  - **Description**: Abstract base class for all bullets, managing velocity, damage calculation, target filters, and collision logic.
  - **File Link**: [BaseBullet.cs](../Bullets/BaseBullet.cs)

- **BounceUntilCollision** (`BounceUntilCollision.cs`)
  - **Description**: Bullet component that bounces off walls multiple times before self-destructing on final contact or timeout.
  - **File Link**: [BounceUntilCollision.cs](../Bullets/BounceUntilCollision.cs)

- **Bullet** (`Bullet.cs`)
  - **Description**: Standard projectile that moves in a straight line and deals direct damage to targets.
  - **File Link**: [Bullet.cs](../Bullets/Bullet.cs)

- **ChainBullet** (`ChainBullet.cs`)
  - **Description**: A projectile that jumps between multiple targets upon impact. Each jump can potentially deal reduced damage.
  - **File Link**: [ChainBullet.cs](../Bullets/ChainBullet.cs)

- **ChaseBullet** (`ChaseBullet.cs`)
  - **Description**: *No description available.*
  - **File Link**: [ChaseBullet.cs](../Bullets/ChaseBullet.cs)

- **ExplosiveBullet** (`ExplosiveBullet.cs`)
  - **Description**: A projectile that triggers an area-of-effect (AOE) explosion upon hitting a target or obstacle. Deals damage to all enemies within the blast radius.
  - **File Link**: [ExplosiveBullet.cs](../Bullets/ExplosiveBullet.cs)

- **IBullet** (`IBullet.cs`)
  - **Description**: Interface detailing basic bullet functions like setting owner, weapon, speed, and triggering firing logic.
  - **File Link**: [IBullet.cs](../Bullets/IBullet.cs)

- **JumpBullet** (`JumpBullet.cs`)
  - **Description**: Bullet that chain-bounces between targets upon successful collision.
  - **File Link**: [JumpBullet.cs](../Bullets/JumpBullet.cs)

- **NoDestructBullet** (`NoDestructBullet.cs`)
  - **Description**: Projectile that does not destroy itself on contact, allowing it to penetrate objects.
  - **File Link**: [NoDestructBullet.cs](../Bullets/NoDestructBullet.cs)

- **OrbitBullet** (`OrbitBullet.cs`)
  - **Description**: A projectile that orbits around its owner rather than traveling in a straight line. Hits enemies that come in close range of the character.
  - **File Link**: [OrbitBullet.cs](../Bullets/OrbitBullet.cs)

- **ParticleCollision** (`ParticleCollision.cs`)
  - **Description**: Bullet component that utilizes Unity's particle system collision callbacks to deal damage.
  - **File Link**: [ParticleCollision.cs](../Bullets/ParticleCollision.cs)

- **PoolBullet** (`PoolBullet.cs`)
  - **Description**: Optimized bullet component designed to be recycled via object pooling.
  - **File Link**: [PoolBullet.cs](../Bullets/PoolBullet.cs)

- **ReturningBullet** (`ReturningBullet.cs`)
  - **Description**: A projectile that travels a specific distance and then returns to the shooter. It can deal damage both while traveling away and while returning.
  - **File Link**: [ReturningBullet.cs](../Bullets/ReturningBullet.cs)

- **SentryTurret** (`SentryTurret.cs`)
  - **Description**: A spawned object that stays in position and automatically fires at nearby enemies until its lifetime expires.
  - **File Link**: [SentryTurret.cs](../Bullets/SentryTurret.cs)

- **SplitterBullet** (`SplitterBullet.cs`)
  - **Description**: A projectile that travels a specific distance or hits a target, then splits into multiple smaller "fragment" bullets. Fragmented bullets carry a portion of the original damage.
  - **File Link**: [SplitterBullet.cs](../Bullets/SplitterBullet.cs)

- **Tentacle** (`Tentacle.cs`)
  - **Description**: Specialized projectile or hazard that extends like a tentacle towards targets.
  - **File Link**: [Tentacle.cs](../Bullets/Tentacle.cs)

- **WaveBullet** (`WaveBullet.cs`)
  - **Description**: A projectile that travels in a wave-like (sinusoidal) pattern. It covers a wider area than a straight bullet and can sometimes "avoid" narrow cover while traveling.
  - **File Link**: [WaveBullet.cs](../Bullets/WaveBullet.cs)

### DesignEx Bullets

- **BindingSealBullet** (`BindingSealBullet.cs`)
  - **Description**: A projectile that forms a binding field upon impact. It can trap multiple enemies in a magical seal.
  - **File Link**: [BindingSealBullet.cs](../Bullets/DesignEx/BindingSealBullet.cs)

- **CrescentWaveBullet** (`CrescentWaveBullet.cs`)
  - **Description**: A wide, crescent-shaped projectile launched on blade swings. Mastery: Its wide hitbox allows it to hit entire crowds in a single line.
  - **File Link**: [CrescentWaveBullet.cs](../Bullets/DesignEx/CrescentWaveBullet.cs)

- **CursedChainBullet** (`CursedChainBullet.cs`)
  - **Description**: The primary chain projectile that roots enemies on impact. Mastery: Hitting already bound targets causes a massive explosion.
  - **File Link**: [CursedChainBullet.cs](../Bullets/DesignEx/CursedChainBullet.cs)

- **DrillBitBullet** (`DrillBitBullet.cs`)
  - **Description**: Specialized projectile for the Railgun that doesn't self-destruct on hit. Instead, it consumes momentum to pierce and then speeds back up.
  - **File Link**: [DrillBitBullet.cs](../Bullets/DesignEx/DrillBitBullet.cs)

- **FestiveFogProjectile** (`FestiveFogProjectile.cs`)
  - **Description**: A lobbed bottle or orb that breaks and releases a toxin fog cloud. Mechanics: Cloud creates tick-damage and applies a slow status.
  - **File Link**: [FestiveFogProjectile.cs](../Bullets/DesignEx/FestiveFogProjectile.cs)

- **GravityWellBullet** (`GravityWellBullet.cs`)
  - **Description**: A slow black hole entity that pulls in enemies and objects. Mechanics: Periodic pulling force towards center. High final damage on collapse.
  - **File Link**: [GravityWellBullet.cs](../Bullets/DesignEx/GravityWellBullet.cs)

- **HomingMissileBullet** (`HomingMissileBullet.cs`)
  - **Description**: A small homing missile that identifies and tracks enemies within its search radius. Mechanics: High speed with steering. Explodes on contact with target or wall.
  - **File Link**: [HomingMissileBullet.cs](../Bullets/DesignEx/HomingMissileBullet.cs)

- **IronSandAegis** (`IronSandAegis.cs`)
  - **Description**: Defensive/Offensive sand drone that orbits the owner. Mastery: Cinching (ICharge) solidifies the cloud into a high-damage spear.
  - **File Link**: [IronSandAegis.cs](../Bullets/DesignEx/IronSandAegis.cs)

- **MagnetShrapnel** (`MagnetShrapnel.cs`)
  - **Description**: Metal shrapnel that lodges in objects and can be recalled magnetically. Mastery: Returning shards deal high damage to enemies on the return path.
  - **File Link**: [MagnetShrapnel.cs](../Bullets/DesignEx/MagnetShrapnel.cs)

- **MarkDartBullet** (`MarkDartBullet.cs`)
  - **Description**: A dart that deals 0 direct damage but embeds itself onto enemies yielding an explosion later.
  - **File Link**: [MarkDartBullet.cs](../Bullets/DesignEx/MarkDartBullet.cs)

- **MirrorRefractionBullet** (`MirrorRefractionBullet.cs`)
  - **Description**: A visual 'ghost' of an attack strike. It doesn't move but deals damage once in its forward zone using HitData.
  - **File Link**: [MirrorRefractionBullet.cs](../Bullets/DesignEx/MirrorRefractionBullet.cs)

- **MjolnirHammerBullet** (`MjolnirHammerBullet.cs`)
  - **Description**: A hammer that travels and returns to the owner. Handles the lightning chain logic while in its 'Return' state.
  - **File Link**: [MjolnirHammerBullet.cs](../Bullets/DesignEx/MjolnirHammerBullet.cs)

- **PhoenixProjectile** (`PhoenixProjectile.cs`)
  - **Description**: A giant fiery phoenix that travels and returns to the owner. Hits all enemies along its path. Returns health to the owner on touch.
  - **File Link**: [PhoenixProjectile.cs](../Bullets/DesignEx/PhoenixProjectile.cs)

- **PortalNode** (`PortalNode.cs`)
  - **Description**: The physical portal on the wall. Mastery: Teleports any IBullet component coming through its threshold.
  - **File Link**: [PortalNode.cs](../Bullets/DesignEx/PortalNode.cs)

- **PortalProjectile** (`PortalProjectile.cs`)
  - **Description**: Shot by the Portal Gun to place a portal node on a wall. Mastery: Placement geometry matters for bullet redirection.
  - **File Link**: [PortalProjectile.cs](../Bullets/DesignEx/PortalProjectile.cs)

- **RicochetChakramBullet** (`RicochetChakramBullet.cs`)
  - **Description**: Bounces off walls. Under its final bounce, it initiates a homing trajectory back to the owner. If it touches the owner, it resets their cooldown.
  - **File Link**: [RicochetChakramBullet.cs](../Bullets/DesignEx/RicochetChakramBullet.cs)

- **SolarFlareBit** (`SolarFlareBit.cs`)
  - **Description**: Specialized drone acting as a projectile. Orbits the owner and zaps targets nearby. Mastery: Its orbit radius is controlled via player input (Charge value). Its fire-rate increases as it is cinched closer to the player.
  - **File Link**: [SolarFlareBit.cs](../Bullets/DesignEx/SolarFlareBit.cs)

- **SonicEchoBullet** (`SonicEchoBullet.cs`)
  - **Description**: A slow sonic wave that reflects off static obstacles. Triggers area-of-effect ripple when bouncing to reward geometry-based positioning.
  - **File Link**: [SonicEchoBullet.cs](../Bullets/DesignEx/SonicEchoBullet.cs)

- **StingerBitComponent** (`StingerBitComponent.cs`)
  - **Description**: A persistent organic swarmer entity spawned by the Living Hive Gun. Mastery: It responds to the player's pheromone (Charge/Aim).
  - **File Link**: [StingerBitComponent.cs](../Bullets/DesignEx/StingerBitComponent.cs)

- **TetherHarpoonBullet** (`TetherHarpoonBullet.cs`)
  - **Description**: A bullet script that tethers an enemy hit by attaching to their transform and optionally dragging them towards the owner.
  - **File Link**: [TetherHarpoonBullet.cs](../Bullets/DesignEx/TetherHarpoonBullet.cs)

- **TetherSpearBullet** (`TetherSpearBullet.cs`)
  - **Description**: Spear behavior that lodges in objects and sustains a damaging beam between owner and self. Mastery: Moving the owner sweeps the beam across the battlefield. Recalls when triggered by the weapon.
  - **File Link**: [TetherSpearBullet.cs](../Bullets/DesignEx/TetherSpearBullet.cs)

- **DragonBurstExplosionBullet** (`DragonBurstExplosionBullet.cs`)
  - **Description**: A stationary golden AOE burst spawned by the Dragon Burst Gauntlet on reaching max Ki stacks. On Fire(), immediately detonates and deals damage to all enemies in its explosion radius. Scales with the weapon's damage multiplier. Self-destructs after a brief lifetime.
  - **File Link**: [DragonBurstExplosionBullet.cs](../Bullets/DesignEx/DragonBurstExplosionBullet.cs)

- **GetsugaHeavyWaveBullet** (`GetsugaHeavyWaveBullet.cs`)
  - **Description**: A massive, slow-moving dark crescent wave released on a full Getsuga Katana charge. Pierces through a large number of enemies before vanishing. Larger and slower than a standard CrescentWaveBullet, making it feel like a powerful, committal ultimate attack.
  - **File Link**: [GetsugaHeavyWaveBullet.cs](../Bullets/DesignEx/GetsugaHeavyWaveBullet.cs)

- **LightningChainBullet** (`LightningChainBullet.cs`)
  - **Description**: A short-range chaining lightning arc spawned by the Raikiri weapon on its first target hit. On Fire(), it immediately searches nearby enemies and chains arcing damage to up to N targets with configurable damage falloff per jump. Stationary — no projectile travel.
  - **File Link**: [LightningChainBullet.cs](../Bullets/DesignEx/LightningChainBullet.cs)

- **SpiritBladeBullet** (`SpiritBladeBullet.cs`)
  - **Description**: A piercing beam of spirit energy launched by the Spirit Blade Weapon's charged attack. Travels at high speed in a straight line, passing through enemies up to a configurable pierce count before self-destructing. Destroyed immediately on wall contact.
  - **File Link**: [SpiritBladeBullet.cs](../Bullets/DesignEx/SpiritBladeBullet.cs)
