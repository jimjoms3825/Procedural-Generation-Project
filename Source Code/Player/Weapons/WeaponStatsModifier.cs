using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: WeaponStatsModifier
 * Description: Simple class which modifies WeaponStats through a singular static method. 
 * Weapon stat modifications are based on a randomly chosen profile, which modifies pre-set stats. 
 */
public class WeaponStatsModifier : MonoBehaviour
{
    public static void modifyWeaponStats(Gun gun)
    {
        WeaponStats newStats = Instantiate(gun.baseStats);

        float index = Random.Range(0, 1f);
        switch (index)
        {
            case < 0.1f:
                newStats.modifierName = "RAPID";
                newStats.modifierDescription = "Greatly increases fire speed, reload speed, and ammo capacity at a small cost to damage.";
                newStats.damage *= 0.8f;
                newStats.fireRate *= Random.Range(1.5f, 1.8f);
                newStats.maxAmmo = (int)(newStats.maxAmmo * 1.5f);
                newStats.magazineSize = (int)(newStats.magazineSize * 1.5f);
                newStats.reloadTime *= Random.Range(0.3f, 0.7f);
                break;
            case < 0.2f:
                newStats.modifierName = "PWRFL";
                newStats.modifierDescription = "Provides a significant damage boost at a cost of firespeed and increased recoil.";
                newStats.damage *= Random.Range(1.5f, 1.8f);
                newStats.fireRate *= 0.7f;
                newStats.recoil *= 1.5f;
                break;
            case < 0.3f:
                newStats.modifierName = "CNVSN";
                newStats.modifierDescription = "Increases damage slightly. Changes pump action wapons into semi auto and increases fire rate;" +
                    " Changes semi auto weapons to burst weapons with a larger damage boost; Changes burst weapons to full auto with better firerates;" +
                    " Changes full auto weapons to burst weapons with greatly increased damage and fire speed";
                newStats.damage *= Random.Range(1.2f, 1.3f);
                if (newStats.weaponType == Gun.WeaponTypes.PumpAction)
                {
                    newStats.weaponType = Gun.WeaponTypes.SemiAutomatic;
                    newStats.fireRate *= 2;
                }
                else if(newStats.weaponType == Gun.WeaponTypes.SemiAutomatic)
                {
                    newStats.weaponType = Gun.WeaponTypes.BurstFire;
                    newStats.damage *= 2;
                    newStats.burstSize = Random.Range(2, 5);
                }
                else if (newStats.weaponType == Gun.WeaponTypes.BurstFire)
                {
                    newStats.weaponType = Gun.WeaponTypes.Automatic;
                    newStats.fireRate *= 1.5f;
                }
                else if (newStats.weaponType == Gun.WeaponTypes.Automatic)
                {
                    newStats.weaponType = Gun.WeaponTypes.BurstFire;
                    newStats.damage *= 2;
                    newStats.fireRate *= 1.5f;
                }
                break;
            case < 0.4f:
                newStats.modifierName = "STD";
                newStats.modifierDescription = "Ol' reliable.";
                break;
            case < 0.5f:
                if(newStats.weaponType == Gun.WeaponTypes.SemiAutomatic)
                {
                    newStats.modifierName = "SHPSTR";
                    newStats.modifierDescription = "Hey there Quickdraw! Extreme increase in firerate, reload, and handling.";
                    newStats.fireRate = 2000;
                    newStats.handling = 99;
                    newStats.reloadTime *= 0.6f;
                }
                else if(newStats.weaponType == Gun.WeaponTypes.PumpAction)
                {
                    newStats.modifierName = "SLMFR";
                    newStats.modifierDescription = "SLAM FIRE! fire rate and reload speed drastically increased.";
                    newStats.fireRate *= 10;
                    newStats.reloadTime *= 0.4f;
                }
                else
                {
                    newStats.modifierName = "DMR";
                    newStats.modifierDescription = "This weapon has been converted to a rifle spec. It's semi auto now, but has greatly increased damage" +
                        " and will fire much more accuratey.";
                    newStats.damage *= 2;
                    newStats.accuracy *= 2;
                    newStats.handling = 95;
                    newStats.weaponType = Gun.WeaponTypes.SemiAutomatic;
                }
                break;
            case < 0.6f:
                newStats.modifierName = "AMMO";
                newStats.modifierDescription = "This weapon is decked out with extra ammo capacity!";
                newStats.maxAmmo = (int)(newStats.maxAmmo * 2);
                newStats.magazineSize = (int)(newStats.magazineSize * 2);
                break;
            case < 0.7f:
                newStats.modifierName = "LSR";
                newStats.modifierDescription = "Converts the weapon to a laser weapon. slightly increases damage and firerate.";
                newStats.bulletType = Bullet.BulletTypes.Energy;
                newStats.damage *= 1.2f;
                newStats.fireRate *= 1.2f;
                break;
            case < 0.8f:
                newStats.modifierName = "XPRT";
                newStats.modifierDescription = "Seems to be an experimental prototype. Might be good, might not be good... Try pulling the trigger!";
                float bulletIndex = Random.Range(0, 1f);
                if(bulletIndex < 0.5f)
                {
                    newStats.bulletType = Bullet.BulletTypes.Ballistic;
                }
                else if (bulletIndex < 0.75f)
                {
                    newStats.bulletType = Bullet.BulletTypes.Energy;
                }
                else
                {
                    newStats.bulletType = Bullet.BulletTypes.Explosive;
                }

                newStats.damage *= Random.Range(0.7f, 1.5f);
                newStats.fireRate *= Random.Range(0.7f, 1.5f);
                break;
            case < 0.9f:
                newStats.modifierName = "ELITE";
                newStats.modifierDescription = "A darn fine weapon in every regard!";
                newStats.damage *= Random.Range(1.5f, 2f);
                newStats.fireRate *= Random.Range(1.5f, 2f);
                newStats.maxAmmo = (int)(newStats.maxAmmo * Random.Range(1.5f, 2f));
                newStats.magazineSize = (int)(newStats.magazineSize * Random.Range(1.5f, 2f));
                newStats.recoil *= Random.Range(0.7f, 0.8f);
                newStats.handling *= Random.Range(1.5f, 2f);
                newStats.burstSize = (int)(newStats.burstSize * Random.Range(1.5f, 2f));
                newStats.reloadTime *= Random.Range(0.7f, 0.8f);
                break;
            case < 0.93f:
                newStats.modifierName = "CHAOS";
                newStats.modifierDescription = "Who knows whats going on here!";
                newStats.damage *= Random.Range(0.3f, 3f); 
                newStats.fireRate *= Random.Range(0.3f, 3f);
                newStats.maxAmmo = (int)(newStats.maxAmmo * Random.Range(0.3f, 3f));
                newStats.magazineSize = (int)(newStats.magazineSize * Random.Range(0.3f, 3f));
                if(newStats.magazineSize < 1) { newStats.magazineSize = 1; }
                newStats.recoil *= Random.Range(0.3f, 3f);
                newStats.handling *= Random.Range(0.3f, 3f);
                newStats.burstSize = (int)(newStats.burstSize * Random.Range(0.3f, 3f));
                newStats.reloadTime *= Random.Range(0.3f, 3f);
                break;
            case < 0.96f:
                newStats.modifierName = "EXPLD";
                newStats.modifierDescription = "Converts the weapon to an explosive weapon. Greatly increases damage, but reduces firespeed and accuracy.";
                newStats.bulletType = Bullet.BulletTypes.Explosive;
                newStats.damage *= 1.8f;
                newStats.fireRate *= 0.7f;
                newStats.accuracy *= 0.5f;
                break;
            case < 1f:
                newStats.modifierName = "SUPER";
                newStats.modifierDescription = "Was this on purpose?";
                newStats.damage *= Random.Range(2f, 3f);
                newStats.fireRate *= Random.Range(2f, 3f);
                newStats.maxAmmo = (int)(newStats.maxAmmo * Random.Range(2f, 3f));
                newStats.magazineSize = (int)(newStats.magazineSize * Random.Range(2f, 3f));
                newStats.recoil *= Random.Range(0.3f, 0.5f);
                newStats.handling *= Random.Range(2f, 3f);
                newStats.burstSize = (int)(newStats.burstSize * Random.Range(2f, 3f));
                newStats.reloadTime *= Random.Range(0.3f, 0.5f);
                newStats.weaponType = Gun.WeaponTypes.Automatic;
                break;
        }

        //Update gun variables.
        gun.CurrentMagazine = newStats.magazineSize;
        gun.TotalAmmo = newStats.maxAmmo;
        gun.gunStats = newStats;
    }
}
