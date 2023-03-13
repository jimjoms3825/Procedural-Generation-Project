using UnityEngine;

/*
 * James Bombardier
 * COMP 495
 * November 23rd, 2022
 * 
 * Class: BulletFactory
 * Description: Provides a simple interface for the creation of bullets. 
 */
public class BulletFactory : MonoBehaviour
{
    //Static instance.
    private static BulletFactory instance;

    //Bullet prefabs. 
    [SerializeField] public GameObject ballisticPrefab;
    [SerializeField] public GameObject energyPrefab;
    [SerializeField] public GameObject explosivePrefab;

    //Creates a ballistic bullet and gives it velocity in the correct direction. 
    public static void createBallisticBullet(Vector3 position, Vector3 direction, float damage)
    {
        Bullet bullet = Instantiate(getInstance().ballisticPrefab).GetComponent<Bullet>();
        bullet.transform.Rotate(direction);
        bullet.GetComponent<Rigidbody>().velocity = 200 * bullet.transform.forward;
        bullet.damage = damage;
        bullet.transform.SetParent(getInstance().transform);
        bullet.transform.position = position + bullet.transform.forward * Random.Range(0f, 1f);
    }

    //Creates an energy bullet and gives it velocity in the correct direction.
    public static void createEnergyBullet(Vector3 position, Vector3 direction, float damage)
    {
        Bullet bullet = Instantiate(getInstance().energyPrefab).GetComponent<Bullet>();
        bullet.transform.Rotate(direction);
        bullet.GetComponent<Rigidbody>().velocity = 200 * bullet.transform.forward;
        bullet.damage = damage;
        bullet.transform.SetParent(getInstance().transform);
        bullet.transform.position = position + bullet.transform.forward * Random.Range(0f, 1f);
    }

    //Creates a gravity affected explosive shell and gives it velocity in the correct direction.
    public static void createExplosiveBullet(Vector3 position, Vector3 direction, float damage)
    {
        Bullet bullet = Instantiate(getInstance().explosivePrefab).GetComponent<Bullet>();
        bullet.transform.Rotate(direction);
        bullet.GetComponent<Rigidbody>().velocity = 50 * bullet.transform.forward;
        bullet.damage = damage;
        bullet.transform.SetParent(getInstance().transform);
        bullet.transform.position = position + bullet.transform.forward * Random.Range(0f, 1f);
    }

    //Creates a shotgun blast from a shell. The damage provided is spread evenly between each pellet.
    public static void createShotgunBlast(Vector3 position, Vector3 direction, float damage)
    {
        float choke = 1.5f;
        damage = damage / 20;
        for(int i = 0; i < 20; i++)
        {
            Bullet bullet = Instantiate(getInstance().ballisticPrefab).GetComponent<Bullet>();
            direction.x += Random.Range(-choke, choke);
            direction.y += Random.Range(-choke, choke);
            bullet.transform.Rotate(direction);
            bullet.GetComponent<Rigidbody>().velocity = Random.Range(40f, 50f) * bullet.transform.forward;
            bullet.damage = damage;
            bullet.transform.SetParent(getInstance().transform);
            bullet.transform.position = position + bullet.transform.forward * Random.Range(0f, 1f);
        }
    }

    //Gets an instance of the bulletfactory. 
    public static BulletFactory getInstance()
    {
        if(instance == null)
        {
            GameObject go = Instantiate(GameManager.instance.bulletFactoryPrefab);
            instance = go.GetComponent<BulletFactory>();
        }
        return instance;
    }
}
