using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: Teleporter
 * Description: Teleports the player to the next level on collision. 
 */
public class Teleporter : MonoBehaviour
{
    //Sends the player to the next level when they collide with this object.
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.instance.changeLevel();
            Destroy(this);
        }
    }
}
