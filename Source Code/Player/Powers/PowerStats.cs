using UnityEngine;
using UnityEngine.Events;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: PowerStats
 * Description: Scriptable objects which contain the variable of a power. 
 */

[CreateAssetMenu(fileName = "PowerScriptableObject")]
public class PowerStats : ScriptableObject
{
    //Variables for the powers.
    public string powerName;
    public string powerDescription;
    public string modifierName;
    public string modifierDescription;
    public float strength;
    public float cooldownTime = 10f;

    [SerializeField]
    public UnityEvent power;

    public void onPowerUse()
    {
        if(power != null)
        {
            //Invokes the Unity event stored in the scriptable object.
            power.Invoke();
        }
    }

    //Activates the player's ammo regen method. 
    public void ammoRegeneration()
    {
        if (GameManager.player == null) { return; }
        GameManager.player.GetComponent<Player>().ammoRegeneration(strength / 2);
    }

    //Provides health to the player. 
    public void heal()
    {
        if(GameManager.player == null) { return; }
        GameManager.player.GetComponent<Player>().heal(strength);
    }

    //Dashes the player forward. 
    public void dash()
    {
        if (GameManager.player == null) { return; }
        GameManager.player.GetComponent<Rigidbody>().AddForce(strength * 100 * GameManager.player.transform.forward);
    }

    //Gives the player invulnerability. 
    public void invulnerability()
    {
        if (GameManager.player == null) { return; }
        GameManager.player.GetComponent<Player>().setInvulnerability(strength);
    }

    //Pushes physics enabled objects back (including explosives). 
    public void repulsion()
    {
        if (GameManager.player == null) { return; }

        Collider[] colliding = Physics.OverlapSphere(GameManager.player.transform.position, strength);
        foreach(Collider col in colliding)
        {
            if(col.tag == "Player") { continue; }
            if(col.GetComponent<Rigidbody>() != null)
            {
                col.GetComponent<Rigidbody>().AddExplosionForce(strength * 50, GameManager.player.transform.position, strength);
            }
            else if (col.GetComponentInParent<Rigidbody>() != null)
            {
                col.GetComponentInParent<Rigidbody>().AddExplosionForce(strength * 50, GameManager.player.transform.position, strength);
            }
        }
    }
}
