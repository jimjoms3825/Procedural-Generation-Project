using System.Collections;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: Power
 * Description: A power which can be activated by the player. 
 */

public class Power : MonoBehaviour
{
    public PowerStats baseStats;
    public PowerStats powerStats;

    public bool canUse = true;

    private void Awake()
    {
        //Modify the powerup on awake. 
        PowerStatsModifier.modifyPower(this);
        //Allow use on fresh instantiation. 
        canUse = true;
    }

    //Uses the power assigned in the powerStats. Starts the refresh cooldown. 
    public bool usePower()
    {
        if(!canUse) { return false; }
        Debug.Log("Using Power");
        canUse = false;
        powerStats.onPowerUse();
        StartCoroutine(refreshPower());
        return true;
    }


    //Threaded method which refreshes the power after the cooldown time has elapsed. 
    protected IEnumerator refreshPower()
    {
        yield return new WaitForSeconds(powerStats.cooldownTime);
        canUse = true;
    }
}
