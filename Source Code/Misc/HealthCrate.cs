/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: HealthCrate
 * Description: Interactable object which heals the player in interaction. 
 */
public class HealthCrate : Interactable
{
    public override void Interact()
    {
        GameManager.player.GetComponent<Player>().life.heal(999999);
    }
}
