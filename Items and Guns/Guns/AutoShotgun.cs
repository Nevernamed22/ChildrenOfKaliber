﻿using UnityEngine;
using Alexandria.ItemAPI;
using Gungeon;

namespace Items
{
    class AutoShotgun : AdvancedGunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Auto Shotgun", "auto_shotgun");
            Game.Items.Rename("outdated_gun_mods:auto_shotgun", "cel:auto_shotgun");
            gun.gameObject.AddComponent<AutoShotgun>();
            gun.SetShortDescription("Become Decease");
            gun.SetLongDescription("An automatic shotgun. Prefered by those who want the coverage of a shotgun paired with the speed of an assault rifle.");
            gun.SetupSprite(null, "auto_shotgun_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 10);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);
            for (int i = 0; i < 3; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "38_special");
            }
            foreach (ProjectileModule projectileModule in gun.Volley.projectiles)
            {
                projectileModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(projectileModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                projectileModule.projectiles[0] = projectile;
                projectileModule.angleVariance = 6;
                projectile.transform.parent = gun.barrelOffset;
                projectile.baseData.damage *= .72f;
                projectile.baseData.speed *= 1f;
                projectileModule.numberOfShotsInClip = 8;
                projectileModule.cooldownTime = .4f;
                projectile.baseData.range = 35f;
                projectileModule.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
                bool flag = projectileModule == gun.DefaultModule;
                if (flag)
                {
                    projectileModule.ammoCost = 1;
                }
                else
                {
                    projectileModule.ammoCost = 0;
                }
                
            }
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(51) as Gun).gunSwitchGroup;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(51) as Gun).muzzleFlashEffects;
            gun.Volley.UsesShotgunStyleVelocityRandomizer = true;
            gun.Volley.DecreaseFinalSpeedPercentMin = -10f;
            gun.Volley.IncreaseFinalSpeedPercentMax = 10f;
            gun.SetBaseMaxAmmo(360);
            
            gun.quality = PickupObject.ItemQuality.C;
            gun.encounterTrackable.EncounterGuid = "fricc u spapi imma use guids all i want >:c";
            gun.sprite.IsPerpendicular = true;
            gun.gunClass = GunClass.FULLAUTO;
            gun.reloadTime = 2.2f;
            ETGMod.Databases.Items.Add(gun.GetComponent<PickupObject>());
        }
        private bool HasReloaded;
        protected override void Update()
        {
            base.Update();
            if (gun.CurrentOwner)
            {

                if (gun.PreventNormalFireAudio)
                {
                    this.gun.PreventNormalFireAudio = true;
                }
                if (!gun.IsReloading && !HasReloaded)
                {
                    this.HasReloaded = true;
                }
            }
        }

        protected override void OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickedUpByPlayer(player);
        }
        protected override void OnPostDroppedByPlayer(PlayerController player)
        {
            base.OnPostDroppedByPlayer(player);
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            if (gun.IsReloading && this.HasReloaded)
            {
                HasReloaded = false;
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                base.OnReloadPressed(player, gun, bSOMETHING);
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);

        }

        public AutoShotgun()
        {

        }
    }
}
