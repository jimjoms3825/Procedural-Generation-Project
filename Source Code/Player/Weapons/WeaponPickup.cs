using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: WeaponPickup
 * Description: Interactable object which provides the player with the weapon assigned to the object.
 */

public class WeaponPickup : Interactable
{
    [SerializeField] private Gun gun;

    //Gives the player a copy of the stored gun.
    public override void Interact()
    {
        if (gun == null) { Debug.LogError("No Gun in Weapon Pickup"); return; }
        GameManager.player.GetComponent<Player>().addGun(Instantiate(gun).gameObject, null);
    }

    //Static method which creates a Weapon pickup from a passed Gun. 
    public static GameObject createWeaponPickup(Gun _gun)
    {
        GameObject pickup = new GameObject();
        pickup.layer = LayerMask.NameToLayer("Interactable");
        WeaponPickup script = pickup.AddComponent(typeof(WeaponPickup)) as WeaponPickup;
        script.gun = _gun;
        script.gun.transform.position = script.transform.position;
        script.transform.SetParent(script.gun.transform.parent);
        script.gun.transform.SetParent(script.transform);
        SphereCollider col = pickup.AddComponent(typeof(SphereCollider)) as SphereCollider;
        col.isTrigger = true;
        Rigidbody rb = pickup.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rb.isKinematic = true;
        return pickup;
    }
}
