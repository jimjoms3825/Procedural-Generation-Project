using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: PowerStatsModifier
 * Description: Modifies the stats of a power.
 */

public class PowerStatsModifier : MonoBehaviour
{
    public static void modifyPower(Power power)
    {
        PowerStats stats = Instantiate(power.baseStats);

        float index = Random.Range(0, 1f);

        if (index < 0.5f)
        {
            stats.modifierName = "STD";
            stats.modifierDescription = "Run of the mill powerup!";
        }
        else if (index < 0.75)
        {
            stats.modifierName = "RPD";
            stats.modifierDescription = "A fast alternative!";
            stats.cooldownTime *= 0.75f;
        }
        else if (index < 0.825)
        {
            stats.modifierName = "PWR";
            stats.modifierDescription = "A much stronger version!";
            stats.strength *= 1.5f;
        }
        else if (index < 0.92)
        {
            stats.modifierName = "ELITE";
            stats.modifierDescription = "Run of the mill powerup!";
            stats.strength *= 1.5f;
            stats.cooldownTime *= 0.75f;
        }
        else
        {
            stats.modifierName = "SPRM";
            stats.modifierDescription = "The supreme choice!";
            stats.strength *= 2;
            stats.cooldownTime *= 0.5f;
        }
        power.powerStats = stats;
    }
}
