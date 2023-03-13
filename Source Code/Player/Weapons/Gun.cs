using System.Collections;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: Gun
 * Description: A class which handles gun behaviour for all weapon types in game. 
 */
public class Gun : MonoBehaviour
{
    //Scriptable object reference for gun's base stats. 
    public WeaponStats baseStats;
    //Reference to stats after modifications made. Identical to baseStats if no changes made. 
    public WeaponStats gunStats;

    //Enum representing the form of weapon. 
    public enum WeaponTypes { SemiAutomatic, PumpAction, BurstFire, Automatic };
    //Enum representing the method the gun uses to reload. 
    public enum ReloadStyle { Clip, Pump }

    
    private int burstShots = 0; //Track how many rounds fired in this burst
    private bool triggerPressed; //Whether the trigger is currently depressed. 
    private bool canShoot = true; //Determined by gun fire rate. Handles shot timing.

    public GameObject firePoint; //End of the gun barrel. 

    private int currentMagazine; //Number of bullets in magazine. 
    private int totalAmmo; //Number of bullets in reserve. 
    public bool hasMag = true; //So no shooting takes place without a magazine.
    private bool reloading = false; //To avoid double reloads and shooting while reloading.

    public float currentRecoil; //Gun's current recoil state. 

    [SerializeField] private GameObject muzzleFlashPrefab; //Prefab of the muzzle flash.

    //Getters and setters. 
    public int CurrentMagazine { get => currentMagazine; set => currentMagazine = value; }
    public bool TriggerPressed { get => triggerPressed; set => triggerPressed = value; }
    public int TotalAmmo { get => totalAmmo; set => totalAmmo = value; }

    //Coroutine for controlling automatic weapon fire. 
    private IEnumerator automaticCoroutine;

    //Initializations. 
    void Awake()
    {
        gunStats = baseStats;
        currentMagazine = gunStats.magazineSize;
        TotalAmmo = gunStats.maxAmmo;
        name = gunStats.weaponName;
    }

    //The method for firing the gun.
    private void fire()
    {
        //If on cooldown.
        if(!canShoot) { return; }
        //If has a magazine inserted and magazine has ammo. 
        if(currentMagazine > 0 && hasMag)
        {
            fireBullet();
            //Play sound with variability. 
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Shot", 
                gunStats.pitchVariability, gunStats.volumeVariability);
            //Add recoil to the system. 
            currentRecoil += gunStats.recoil / gunStats.handling;
            currentMagazine--;
            //Increase burst shots in a burst system. 
            if(gunStats.weaponType == WeaponTypes.BurstFire)  {
                burstShots++;
            }
        }
        else // empty clip
        {
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " dry fire",
                gunStats.pitchVariability, gunStats.volumeVariability);
        }

        //Different post-fire behaviour based on weapon type.
        switch (gunStats.weaponType)
        {
            case WeaponTypes.BurstFire:
                if(currentMagazine <= 0)
                {
                    burstShots = gunStats.burstSize; //Immediately stop the burst.
                }
                break;
            case WeaponTypes.SemiAutomatic:
                canShoot = false;
                StartCoroutine(pauseBetweenShots());
                break;
            case WeaponTypes.PumpAction:
                canShoot = false;
                if (currentMagazine > 0)
                {
                    StartCoroutine(pumpGun());
                }
                break;
        }
    }

    //Simulates triger pull action for different weapon types. 
    public void pullTrigger()
    {
        if (triggerPressed) { return; } // avoid multiple trigger presses.
        triggerPressed = true;

        switch (gunStats.weaponType)
        {
            case WeaponTypes.SemiAutomatic:
                fire();
                break;
            case WeaponTypes.PumpAction:
                fire();
                break;
            case WeaponTypes.BurstFire:
                if (burstShots <= 0 && canShoot)//Dissalow overlapping bursts.
                {
                    fire();
                    if (currentMagazine <= 0) //Don't rapid dry fire the trigger for auto and burst.
                    {
                        burstShots = 0;
                        StartCoroutine(pauseBetweenShots());
                        return;
                    }
                    if (gunStats.burstSize <= 2) //Burst is minimum of two. Anything less is semi.
                    {
                        StartCoroutine(queueLastShot());
                    }
                    else
                    {
                        StartCoroutine(queueNextShot());
                    }
                }
                break;
            case WeaponTypes.Automatic:
                fire();
                if (currentMagazine <= 0) //Don't rapid dry fire the trigger for auto and burst.
                {
                    StartCoroutine(pauseBetweenShots());
                    return;
                }
                automaticCoroutine = queueNextShot(); 
                StartCoroutine(automaticCoroutine);
                break;

        };
    }

    //Simulates releasing a gun's trigger. 
    public void releaseTrigger()
    {
        triggerPressed = false;
        if(automaticCoroutine != null) { StopCoroutine(automaticCoroutine); }
    }

    //Reloads the weapon. 
    public void reload()
    {
        //Dump the current mag if one is inserted. 
        if (hasMag && gunStats.reloadStyle != ReloadStyle.Pump && !reloading)
        {
            unloadMag();
        }
        else if(gunStats.reloadStyle != ReloadStyle.Pump && !reloading) // empty non pump.
        {
            reloading = true;
            StartCoroutine(reloadMag());
        }
        else // reload pump weapon.
        {
            if(currentMagazine < gunStats.magazineSize && !reloading) // reload if missing shells.
            {
                reloading = true;
                StartCoroutine(loadShells());
            }
        }
    }

    //Ejects the current magazine from the weapon. 
    public void unloadMag()
    {
        currentMagazine = 0;
        hasMag = false;
        if(gunStats.weaponType != WeaponTypes.PumpAction)
        {
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Unload Mag",
        gunStats.pitchVariability, gunStats.volumeVariability);
        }
    }

    //Reloads a magazine into the gun. 
    public IEnumerator reloadMag()
    {
        if (totalAmmo > gunStats.magazineSize)
        {
            totalAmmo -= gunStats.magazineSize;
            currentMagazine = gunStats.magazineSize;
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Load Mag",
                gunStats.pitchVariability, gunStats.volumeVariability);
        }
        else if (totalAmmo > 0)
        {
            currentMagazine = totalAmmo;
            totalAmmo = 0;
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Load Mag",
                gunStats.pitchVariability, gunStats.volumeVariability);
        }
        else
        {
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " dry fire",
                gunStats.pitchVariability, gunStats.volumeVariability);
        }

        yield return new WaitForSeconds(gunStats.reloadTime * 0.8f);
        SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Rack",
                gunStats.pitchVariability, gunStats.volumeVariability);
        yield return new WaitForSeconds(gunStats.reloadTime * 0.2f);
        hasMag = true;
        reloading = false;
    }

    //Threaded method that inserts shotgun shells one at a time. 
    private IEnumerator loadShells()
    {
        //Dissalow shooting during reload. 
        canShoot = false;
        hasMag = false;
        //Shell capacity isnt full and has spare shells. 
        if(CurrentMagazine < gunStats.magazineSize && totalAmmo > 0)
        {
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Insert Round",
        gunStats.pitchVariability, gunStats.volumeVariability);
            currentMagazine++;
            TotalAmmo--;
            yield return new WaitForSeconds(gunStats.reloadTime);
            StartCoroutine(loadShells());
        }
        else
        {
            SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " Pump",
                gunStats.pitchVariability, gunStats.volumeVariability);
            hasMag = true;
            canShoot = true;
            reloading = false;
        }
    }

    //Queues the next shot in automatic and burst fire weapons. 
    private IEnumerator queueNextShot()
    {
        yield return new WaitForSeconds(60 / gunStats.fireRate);
        switch (gunStats.weaponType)
        {
            case WeaponTypes.BurstFire:
                fire();
                if (currentMagazine <= 0 || burstShots >= gunStats.burstSize - 1) // ends burst
                { 
                    StartCoroutine(queueLastShot());
                }
                else if (canShoot && currentMagazine > 0) //Dissalow overlapping bursts. 
                {
                    StartCoroutine(queueNextShot());
                }
                break;
            case WeaponTypes.Automatic:
                if (triggerPressed && currentMagazine > 0)
                {
                    fire();
                    StartCoroutine(queueNextShot());
                }
                else if (triggerPressed && currentMagazine <= 0)
                {
                    fire();
                    StartCoroutine(queueLastShot());
                }
                break;
        };
    }

    //Queues the last shot in a burst or automatic sequence. 
    private IEnumerator queueLastShot()
    {
        yield return new WaitForSeconds(60 / gunStats.fireRate);
        switch (gunStats.weaponType)
        {
            case WeaponTypes.BurstFire:
                fire();
                canShoot = false;
                burstShots = 0;
                StartCoroutine(pauseBetweenShots());
                break;
            case WeaponTypes.Automatic:
               if(triggerPressed) { fire(); }
                break;
        };
    }

    //Provides a pause between shots for pump and automatic weapon platforms. 
    private IEnumerator pauseBetweenShots()
    {
        if(gunStats.weaponType == WeaponTypes.BurstFire)
        {
            yield return new WaitForSeconds(30 / gunStats.fireRate); // Wait even longer for burst weapons.
        }
        yield return new WaitForSeconds(60 / gunStats.fireRate);
        canShoot = true;
    }

    //Pumps s gun in between shots. 
    private IEnumerator pumpGun()
    {
        yield return new WaitForSeconds(30 / gunStats.fireRate);
        SoundManager.GetInstance().PlaySound("event:/Weapon Sounds/" + gunStats.weaponName + "/" + gunStats.weaponName + " pump",
            gunStats.pitchVariability, gunStats.volumeVariability);
        yield return new WaitForSeconds(30 / gunStats.fireRate);
        canShoot = true;
    }

    //Sets the gun to the render on top layer.
    public void recursiveSetLayer(Transform transformTarget)
    {
        transformTarget.gameObject.layer = 3;
        if (transformTarget.childCount > 0)
        {
            for(int i = 0; i < transformTarget.childCount; i++)
            {
                recursiveSetLayer(transformTarget.GetChild(i));
            }
        }
    }

    //Fires a bullet of the gun's type.
    private void fireBullet()
    {
        Vector3 bulletRotation = Camera.main.transform.rotation.eulerAngles;
        float playerAccuracy = GameManager.player.GetComponent<Player>().currentAccuracy / 50; // Approx calc based on crosshairs.
        bulletRotation.x += Random.Range(-playerAccuracy, playerAccuracy);
        bulletRotation.y += Random.Range(-playerAccuracy, playerAccuracy);

        if(muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab);
            flash.transform.position = firePoint.transform.position;
            flash.transform.rotation = firePoint.transform.rotation;
        }

        Vector3 bulletPosition = Camera.main.transform.position;

        switch (gunStats.bulletType)
        {
            case Bullet.BulletTypes.Ballistic:
                BulletFactory.createBallisticBullet(bulletPosition, bulletRotation, gunStats.damage);
                break;
            case Bullet.BulletTypes.Energy:
                BulletFactory.createEnergyBullet(bulletPosition, bulletRotation, gunStats.damage);
                break;
            case Bullet.BulletTypes.Explosive:
                BulletFactory.createExplosiveBullet(bulletPosition, bulletRotation, gunStats.damage);
                break;
            case Bullet.BulletTypes.Shell:
                BulletFactory.createShotgunBlast(bulletPosition, transform.rotation.eulerAngles, gunStats.damage);
                break;
        }

    }
}
