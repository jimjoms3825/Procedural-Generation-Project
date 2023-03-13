using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: PowerPickup
 * Description: Interactable object which gives the player a random powerup when interacted with. 
 */

public class PowerPickup : Interactable
{
    //The power stored in the pickup.
    private Power power;
    //Whether the pickup gives infinite powers. 
    [SerializeField] private bool infinite = false;
    //The potential powers to spawn. 
    [SerializeField] private Power[] powers;

    private void Start()
    {
        generatePower();
    }

    //Sets the players power to the stored power. 
    public override void Interact()
    {
        GameManager.player.GetComponent<Player>().setPower(power);
        if (infinite)
        {
            generatePower();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Chooses and instantiates a power from the available list and assigns it to the power variable.
    private void generatePower()
    {
        if(powers == null) { return; }
        power = Instantiate(powers[Random.Range(0, powers.Length)]);
        displayText = "Press G to gain the power " + power.powerStats.powerName + ".";
    }
}
