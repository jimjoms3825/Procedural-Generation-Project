
/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: AmmoCrate
 * Description: Interactable object which refils the player's ammo. 
 */

public class AmmoCrate : Interactable
{
    private void Start()
    {
        displayText = "Press G to refill your ammo!";
    }

    //Called when player presses the interact button. 
    public override void Interact()
    {
        if(GameManager.player != null)
        {
            GameManager.player.GetComponent<Player>().refillAmmo();
        }
    }
}
