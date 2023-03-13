using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: WeaponStats
 * Description: Scriptable object which holds a profile for a weapon's stats. 
 */

[CreateAssetMenu(fileName = "GunScriptableObject")]
public class WeaponStats : ScriptableObject
{
    
    [SerializeField] public string weaponName;
    [SerializeField] public string weaponShortName;
    [SerializeField] public string description;
    [SerializeField] public string modifierName = "STD";
    [SerializeField] public string modifierDescription;
    [Space]
    [SerializeField] public Gun.WeaponTypes weaponType;
    [SerializeField] public Bullet.BulletTypes bulletType;
    [SerializeField] public Gun.ReloadStyle reloadStyle;
    [Space]
    [SerializeField, Range(0, 100)] public float damage;
    [SerializeField, Range(0, 10)] public float zoom;
    [SerializeField, Range(0, 100)] public float accuracy;
    [SerializeField, Range(0, 200)] public float recoil;
    [SerializeField, Range(0, 100)] public float handling; // Controls handling after shooting and accuracy loss from movement.
    [SerializeField, Range(0, 4)] public float reloadTime;
    [SerializeField, Range(60, 4000)] public float fireRate;
    [SerializeField, Range(2, 20)] public int burstSize;
    [Space]
    [SerializeField] public int maxAmmo;
    [SerializeField] public int magazineSize;
    [Space]
    [SerializeField, Range(-1, 1)] public float volumeVariability;
    [SerializeField, Range(-1, 1)] public float pitchVariability;
    [Space]
    [SerializeField] public Vector3 gunOffset;
}
