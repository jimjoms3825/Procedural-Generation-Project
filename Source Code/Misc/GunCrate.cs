using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: GunCrate
 * Description: Interactable object which contains a gun for the player to grab. Stores 
 * old weapons if player was at max gun capacity. 
 */
public class GunCrate : Interactable
{
    //Struct representing the chance that each gun prefab has to spawn. 
    [System.Serializable]
    struct GunLoot {
        public Gun Gun;
        [Range(0, 10)] public float chanceAddition; // Will be pooled together. 
    }

    //A collection of lootable guns from the box. 
    [SerializeField] private GunLoot[] lootTable;
    //If set to true, the crate will give infinite weapons. 
    [SerializeField] private bool infinite = false;
    //The gun that was traded in. 
    private Gun oldGun;
    //If the gun crate has been interacted with. 
    private bool used = false;

    private void Start()
    {
        displayText = "Press G to get a random weapon!";
    }

    public override void Interact()
    {
        if(lootTable != null && (oldGun == null || infinite)) {
            Gun toReturn = lootTable[0].Gun;
            float total = 0;
            foreach (GunLoot loot in lootTable)
            {
                total += loot.chanceAddition; // Pool chances
            }
            //Pick a random weapon from the potential weapons. 
            float itemDraw = Random.Range(0, total);
            foreach (GunLoot loot in lootTable)
            {
                itemDraw -= loot.chanceAddition; // decremental traversal of list.
                if (itemDraw <= 0)
                {
                    toReturn = loot.Gun;
                    break;
                }
            }
            Gun gun = Instantiate(toReturn);
            WeaponStatsModifier.modifyWeaponStats(gun); // Randomize stats of box guns. 
            GameManager.player.GetComponent<Player>().addGun(gun.gameObject, this);
        }
        else if(oldGun != null)
        {
            GameManager.player.GetComponent<Player>().addGun(oldGun.gameObject, this);
        }
        //Set text
        if (oldGun == null || infinite)
        {
            displayText = "Press G to get a random weapon!";
        }
        else if (oldGun != null)
        {
            displayText = "Press G to get your old " + oldGun.gunStats.weaponName + " back.";
        }
        used = true; // Update used status.
        if (used && !infinite && oldGun == null)
        {
            Destroy(this.gameObject);
        }
    }

    //Gives a weapon to the gunCrate. 
    public void giveWeapon(Gun oldWeapon)
    {
        oldGun = oldWeapon;
        oldGun.transform.SetParent(this.transform);
    }
}
