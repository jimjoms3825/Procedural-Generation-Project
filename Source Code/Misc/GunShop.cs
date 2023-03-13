using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: GunShop
 * Description: Allows the player to buy from one of several pre-generated weapons. Gun shops can
 * only be used once, and will not allow the player to retrieve a deposited weapon. 
 */
public class GunShop : Interactable
{
    //List of potential guns to spawn. 
    [SerializeField] private Gun[] gunsToSpawn;
    //List of spawned weapons. 
    private LinkedList<GameObject> gunList;
    //The currently selected gun from the gunlist. 
    private GameObject currentGun;

    //The name of the gun. 
    [SerializeField] private TextMeshProUGUI gunName;
    //Whether the item is single use. 
    [SerializeField] private bool infinite;
    //The canvas used to display the gunInfo.
    [SerializeField] private Canvas displayCanvas;


    // Start is called before the first frame update
    void Start()
    {
        displayText = "Press G to buy gun.";
        populateNewGunList();
    }

    //Called once per frame. 
    private void LateUpdate()
    {
        //enable the canvas based on player distance. 
        if(GameManager.player != null && Vector3.Distance(transform.position, GameManager.player.transform.position) < 5)
        {
            displayCanvas.enabled = true;
        }
        else
        {
            displayCanvas.enabled = false;
        }
    }

    //Called when player presses the interact button. 
    public override void Interact()
    {
        //Applies a modifier to the currently selected weapon. 
        WeaponStatsModifier.modifyWeaponStats(currentGun.GetComponent<Gun>());
        //Gives the player the weapon and sets it active. . 
        currentGun.SetActive(true);
        GameManager.player.GetComponent<Player>().addGun(currentGun, null);
        //functionality for infinite spawning. 
        if (infinite)
        {
            gunList.Remove(currentGun);
            populateNewGunList();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Creates a list of guns for the player to take. 
    private void populateNewGunList()
    {
        //Generate 3-7 guns. 
        int numberOfGuns = Random.Range(3, 7);
        //Dont generate more guns than there are number of spawnable guns. 
        numberOfGuns = Mathf.Clamp(numberOfGuns, 0, gunsToSpawn.Length);
        //Clear the current gunlist. 
        if(gunList != null)
        {
            foreach (GameObject go in gunList)
            {
                Destroy(go);
            }
            gunList = null;
        }
        //Create a new list. 
        gunList = new LinkedList<GameObject>();

        //While more guns should be generated. 
        while(gunList.Count < numberOfGuns)
        {
            //Instantiate a new gun and ensure no membership in gunlist. 
            GameObject go = Instantiate(gunsToSpawn[Random.Range(0, gunsToSpawn.Length - 1)].gameObject);
            foreach(GameObject other in gunList)
            {
                if(go.GetComponent<Gun>().gunStats.weaponName.Equals(other.GetComponent<Gun>().gunStats.weaponName))
                {
                    Destroy(go.gameObject);
                    go = null;
                    break;
                }
            }
            //if no conflict found, add gun to gunlist. 
            if(go == null) { continue; }
            gunList.AddLast(go);
            gunList.Last.Value.SetActive(false);
        }
        //Assign gunName and currentgun..
        currentGun = gunList.First.Value;
        gunName.text = currentGun.GetComponent<Gun>().gunStats.weaponName;
    }

    //Allows the player to scroll between gun options. 
    public void setInput(float f)
    {

        if(f < 1)
        {
            if(currentGun.Equals(gunList.First.Value))
            {
                currentGun = gunList.Last.Value;
            }
            else
            {
                currentGun = gunList.Find(currentGun).Previous.Value;
            }
        }
        else
        {
            if (currentGun.Equals(gunList.Last.Value))
            {
                currentGun = gunList.First.Value;
            }
            else
            {
                currentGun = gunList.Find(currentGun).Next.Value;
            }
        }
        gunName.text = currentGun.GetComponent<Gun>().gunStats.weaponName;
    }
}
