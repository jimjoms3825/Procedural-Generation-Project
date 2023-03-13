using UnityEngine;
using UnityEngine.AI;

/*
 * James Bombardier
 * COMP 495
 * November 18th, 2022
 * 
 * Class: Enemy
 * Description: The script which handles enemy operations. AI is handled
 * through the assigned behaviour tree. Movement AI is handled through the
 * UNITY NavMeshAgent experimental component. 
 */

public class Enemy : MonoBehaviour
{
    //Prefab and item assignments. 
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject shootFX;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject explosiveBulletPrefab;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private BehaviorTree behaviorTree;

    //Variables. 
    [SerializeField] private float damage;
    [SerializeField] private float innacuracy;
    [SerializeField] private float maxHealth;

    //For medic enemies. 
    [SerializeField] private ParticleSystem healingParticles;
    [SerializeField] private float healingPerSecond;

    //For playing voice lines. 
    public enum VoiceLines { Moving, Searching, Engaging, Explosive, Flanking, EnemyDefeated }

    private float lastVoiceLine = 0;

    //Public Variables.
    public int explosives = 3;
    public bool isMedic = false;

    //Timing variables. 
    public float explosiveTime = 3;
    public float meleeTime = 1f;
    public float shootTime = 0.1f;

    //Programatically assigned components.  
    private Life life;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    public Squad squad;

    /*
     * Similar to a constructor method. Assigns and instantiates variables. 
     */
    void Awake()
    {
        lastVoiceLine = Random.Range(-3, 0);
        life = gameObject.AddComponent(typeof(Life)) as Life;
        life.setMaxLife(maxHealth);
        life.setLife(life.getMaxLife());
        //Listeners for life. 
        life.die.AddListener(onDeath);
        life.hurt.AddListener(onHurt);

        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        behaviorTree = GetComponent<BehaviorTree>();
        tag = "Enemy";
    }

    /*
     * Called every physics update. 
     */
    private void FixedUpdate()
    {
        //Programatically controls walking and idle animation through float. 
        anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        //Set shooting anim if recently shot. 
        if(behaviorTree.blackBoard.hasData("Last Shot") && Time.time - (float)behaviorTree.blackBoard.getData("Last Shot") < 0.2f)
        {
            anim.SetBool("Shooting", true);
        }
        else
        {
            anim.SetBool("Shooting", false);
        }
        //If navMeshAgent is removed from navmesh, cull the enemy.
        if (!navMeshAgent.isOnNavMesh)
        {
            onDeath();
        }
    }

    //Update squad blackboard every frame. 
    private void LateUpdate()
    {
        updateSquadBlackBoard();
    }

    //Cleans up the enemy gameobject on death.
    private void onDeath()
    {
        if(deathEffect != null)
        {
            GameObject go = Instantiate(deathEffect);
            go.transform.position = transform.position;
        }

        if(squad != null)
        {
            squad.removeEnemy(this);
        }
        Destroy(gameObject);
    }

    //Play sound when enemy gets hurt. 
    private void onHurt()
    {
        SoundManager.GetInstance().PlaySound("event:/Enemy Hit");
    }

    //Debugs the destination of the agent in Unity editor. 
    private void OnDrawGizmos()
    {
        if(navMeshAgent != null)
        {
            Gizmos.DrawWireSphere(navMeshAgent.destination, 1);
        }
    }

    //Convenience method for updating blackboard information. 
    private void updateSquadBlackBoard()
    {
        if(squad == null) { return; }
        if(GameManager.player != null)
        {
            Vector3 playerDirection = GameManager.player.transform.position - transform.position;
            playerDirection.Normalize();
            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.up, playerDirection, out hit, 50);
            if(hit.collider != null && hit.collider.tag == "Player")
            {
                squad.blackBoard.addData("Player Position", GameManager.player.transform.position);
                squad.blackBoard.addData("Player Last Seen", Time.time);
                squad.blackBoard.addData("In Combat", true);
            }
        }
    }

    //Shoots an explosive at the player. 
    public void useExplosive()
    {
        behaviorTree.blackBoard.addData("Last Explosive Use", Time.time);
        explosives--;
        playVoiceLine(VoiceLines.Explosive);

        SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Explosive Fire", gameObject, 0.1f, 0.1f);

        GameObject flash = Instantiate(shootFX);
        flash.transform.position = firePoint.position;
        flash.transform.rotation = firePoint.rotation;
        flash.transform.localScale = Vector3.one * 3;

        Bullet bullet = Instantiate(explosiveBulletPrefab).GetComponent<Bullet>();
        bullet.transform.rotation = Quaternion.LookRotation(GameManager.player.transform.position - firePoint.transform.position);
        Vector3 euler = bullet.transform.eulerAngles;
        euler.x += Random.Range(-innacuracy, innacuracy);
        euler.y += Random.Range(0, innacuracy); //only up to account for explosive arc
        bullet.transform.eulerAngles = euler;

        bullet.GetComponent<Rigidbody>().velocity = 100 * bullet.transform.forward;

        bullet.damage = damage;
        bullet.transform.SetParent(BulletFactory.getInstance().transform);
        bullet.transform.position = firePoint.position + bullet.transform.forward * Random.Range(0f, 1f);
    }

    //Shoots a bullet at the player. 
    public void shoot()
    {
        SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Enemy Shot", gameObject, 0.1f, 0.1f);
        behaviorTree.blackBoard.addData("Last Shot", Time.time);
        GameObject flash = Instantiate(shootFX);
        flash.transform.position = firePoint.position;
        flash.transform.rotation = firePoint.rotation;
        flash.transform.localScale = Vector3.one * 3;

        Bullet bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
        bullet.transform.rotation = Quaternion.LookRotation(GameManager.player.transform.position- firePoint.transform.position);
        Vector3 euler = bullet.transform.eulerAngles;
        euler.x += Random.Range(-innacuracy, innacuracy);
        euler.y += Random.Range(-innacuracy, innacuracy);
        bullet.transform.eulerAngles = euler;
        
        bullet.GetComponent<Rigidbody>().velocity = 100 * bullet.transform.forward;

        bullet.damage = damage;
        bullet.transform.SetParent(BulletFactory.getInstance().transform);
        bullet.transform.position = firePoint.position + bullet.transform.forward * Random.Range(0f, 1f);
    }

    /*
     * Shell of the melee method. I did not have time to animate the
     * melee functions, so I left this code as a shell. The behaviour tree 
     * calls this method successfully, however it is just a stub.
     */
    public void melee()
    {
        behaviorTree.blackBoard.addData("Last Melee Use", Time.time);
        Debug.Log("Melee");
    }

    //Requests a voiceline to be played. 
    public void playVoiceLine(VoiceLines line)
    {
        if(Time.time - lastVoiceLine < 3) { return; } // Avoid saying things more than once every 3 seconds.
        lastVoiceLine = Time.time;
        switch (line)
        {
            case VoiceLines.Moving:
                SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Moving", gameObject, 0.1f, 0.1f);
                break;
            case VoiceLines.Searching:
                SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Searching", gameObject, 0.1f, 0.1f);
                break;
            case VoiceLines.Engaging:
                SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Engaging", gameObject, 0.1f, 0.1f);
                break;
            case VoiceLines.Explosive:
                SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Explosive", gameObject, 0.1f, 0.1f);
                break;
            case VoiceLines.Flanking:
                SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Flanking", gameObject, 0.1f, 0.1f);
                break;
            case VoiceLines.EnemyDefeated:
                SoundManager.GetInstance().playSoundAtPosition("event:/Enemy Sounds/Threat Eliminated", gameObject, 0.1f, 0.1f);
                break;
        }
    }

    //Heals the enemy.
    public void heal(Enemy healTarget)
    {
        Life healTargetLife = healTarget.GetComponent<Life>();
        healTargetLife.heal(healingPerSecond * Time.deltaTime);
        healingParticles.Play();
        if(healTargetLife.getLife() <= healTargetLife.getMaxLife())
        {
            squad.releaseMedicRequest(healTarget);
        }
    }
}
