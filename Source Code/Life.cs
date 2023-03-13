using UnityEngine;
using UnityEngine.Events;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: Life
 * Description: Agnostic life script with event driven design compatibility. . 
 */

public class Life : MonoBehaviour
{
    //The maximum life of the system. 
    private float maxLife = 10;
    //The current life. 
    private float life;
    //Whether the system can recieve damage. 
    public bool invulnerable = false;
    //For event driven programming. 
    public UnityEvent die = new UnityEvent();
    public UnityEvent hurt = new UnityEvent();

    //Initialize. 
    private void Awake()
    {
        life = maxLife;
    }

    //Getter for the current life. 
    public float getLife()
    {
        return life;
    }

    //Sets and clamps the life value. 
    public void setLife(float newLife)
    {
        life = newLife;
        life = Mathf.Clamp(life, 0, maxLife);
    }

    //Getter for max life. 
    public float getMaxLife()
    {
        return maxLife;
    }

    //Sets the max life value. 
    public void setMaxLife(float newMaxLife)
    {
        maxLife = newMaxLife;
    }

    //Deals damage to the life. Clamped at 0 and can trigger the Hurt and Die events. 
    public void dealDamage(float damage)
    {
        if (invulnerable) { return; }
        life -= damage;
        life = Mathf.Clamp(life, 0, maxLife);
        hurt.Invoke();
        if(life == 0)
        {
            die.Invoke();
        }
    }

    //Heals the system and clamps the value. 
    public void heal(float healAmount)
    {
        if (invulnerable) { return; }
        life += healAmount;
        life = Mathf.Clamp(life, 0, maxLife);
    }
}
