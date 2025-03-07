﻿using System;
using System.Collections;
using Alexandria.ItemAPI;
using UnityEngine;
using Dungeonator;

namespace Items
{
    class DragunWing : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Dragun Wing";

            string resourceName = "Items/Resources/ItemSprites/Passives/dragun_wing.png";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DragunWing>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Where Eagles Dare";
            string longDesc = "Increases movement speed. Killing an enemy creates a ring of fire around the player.\n\nThe wing of an elderly Dragun that is too weak to allow for flight anymore. It glows in the presence of gunfire." +
                "\n\nRight before their home was destroyed, the Draguns sent their bravest warrior to the Gungeon in an attempt to prevent the onset of the apocolypse.";
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "cel");
            
            item.quality = ItemQuality.B;
            item.sprite.IsPerpendicular = true;
            item.CanBeDropped = true;
            item.CanBeSold = true;
            dragunWingId = item.PickupObjectId;
        }

        public void OnKill(PlayerController player)
        {
            if (this.m_indicator)
            {
                return;
            }
            Vector2 vector = Owner.sprite.WorldCenter;
            GameManager.Instance.StartCoroutine(this.HandleHeatEffectsCR(TRadius, TDuration, THeatEffect, vector));
            
        }
        private IEnumerator HandleHeatEffectsCR(float Radius, float Duration, GameActorFireEffect HeatEffect, Vector2 pos)
        {

            
            this.HandleRadialIndicator(TRadius, pos);
            float elapsed = 0f;
            RoomHandler r = Owner.CurrentRoom;
            
            Action<AIActor, float> AuraAction = delegate (AIActor actor, float dist)
            {
                actor.ApplyEffect(THeatEffect, 1f, null);
            };
            while (elapsed < TDuration)
            {
                elapsed += BraveTime.DeltaTime;
                r.ApplyActionToNearbyEnemies(pos, TRadius, AuraAction);
                yield return null;
            }
            this.UnhandleRadialIndicator();
            yield break;
        }
        private void HandleRadialIndicator(float Radius, Vector2 pos)
        {
           
            if (!this.m_indicator)
            {

                this.m_indicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), pos, Quaternion.identity, sprite.transform)).GetComponent<HeatIndicatorController>();
                this.m_indicator.CurrentRadius = TRadius;
                
            }
        }
        private void UnhandleRadialIndicator()
        {
            if (this.m_indicator)
            {
                this.m_indicator.EndEffect();
                this.m_indicator = null;
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnKilledEnemy += this.OnKill;

            
            if (player.HasPickupID(DragunClaw.dragunClawID) && player.HasPickupID(DragunSkull.dragunSkullId) && player.HasPickupID(DragunClaw.dragunClawID) && !player.HasPickupID(SpiritOfTheDragun.gunID))
            {
                AkSoundEngine.PostEvent("Play_VO_dragun_death_01", gameObject);
                player.inventory.AddGunToInventory(PickupObjectDatabase.GetById(SpiritOfTheDragun.gunID) as Gun, true);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.OnKilledEnemy -= this.OnKill;
            debrisObject.GetComponent<DragunWing>().m_pickedUpThisRun = true;
            
            return debrisObject;
        }
        
        private float TDuration = 8f;
        private float TRadius = 3.5f;

        private GameActorFireEffect THeatEffect = new GameActorFireEffect()
        {
                IsGreenFire = false,
                AffectsEnemies = true,
                
                DamagePerSecondToEnemies = 10f,


        };
        private HeatIndicatorController m_indicator;
        public static int dragunWingId;
        
    }
}
