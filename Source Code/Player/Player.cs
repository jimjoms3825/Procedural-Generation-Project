using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * James Bombardier
 * COMP 495
 * November 24th, 2022
 * 
 * Class: Player
 * Description: A player controller class. 
 */
public class Player : MonoBehaviour
{
    //References.
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera handCam;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Gun defaultGunPrefab;

    //Variables to edit.
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveLerpSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float lookSensitivity = 0.5f;
    [SerializeField] private float lookClamp = 85f;

    //Programatically assigned variables.
    public float currentAccuracy;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation;
    private bool isJumping;
    private bool isADS;
    private float playerCamFOV;
    private float handCamFOV;

    //Inventory. 
    [SerializeField] private GameObject currentGun;
    [SerializeField] private GameObject gun1;
    [SerializeField] private GameObject gun2;
    [SerializeField] private GameObject gun3;

    [SerializeField] private Power currentPower;

    private GameObject currentGrenade;
    public int currentGrenades = 3;

    //Player life. 
    public Life life;

    //LayerMasks. 
    private int groundMask;
    private int interactableMask;

    //Instantiation. 
    private void Awake()
    {
        groundMask = LayerMask.GetMask("Ground");
        interactableMask = LayerMask.GetMask("Interactable");
        playerCamFOV = playerCamera.fieldOfView;
        handCamFOV = handCam.fieldOfView;
        life = gameObject.AddComponent(typeof(Life)) as Life;
        life.setMaxLife(100);
        life.setLife(life.getMaxLife());
        life.die.AddListener(onDeath);
        life.hurt.AddListener(onHurt);
        addGun(Instantiate(defaultGunPrefab).gameObject, null);
    }

    //Called every physics update. 
    private void FixedUpdate()
    {
        Vector3 velTarget = new Vector3(0, rb.velocity.y, 0);

        velTarget += transform.forward * moveInput.y * moveSpeed;
        velTarget += transform.right * moveInput.x * moveSpeed;

        //Ground check. 
        if (isGrounded())
        {
            rb.velocity = Vector3.Lerp(rb.velocity, velTarget, moveLerpSpeed * Time.deltaTime); // Normal Physics
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, velTarget, moveLerpSpeed / 15 * Time.deltaTime); // Less control in air
        }


        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity); // Horizontal rotation about y axis. 

        xRotation -= lookInput.y * lookSensitivity;
        xRotation = Mathf.Clamp(xRotation, -lookClamp, lookClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        playerCamera.transform.eulerAngles = targetRotation;

        updateGun();
        updateAccuracy();
    }

    //Called every frame. 
    private void LateUpdate()
    {
        if (isADS && currentGun != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, playerCamFOV - getCurrentWeapon().gunStats.zoom, Time.deltaTime * 5);
            handCam.fieldOfView = Mathf.Lerp(handCam.fieldOfView, handCamFOV - getCurrentWeapon().gunStats.zoom, Time.deltaTime * 5);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, playerCamFOV, Time.deltaTime * 5);
            handCam.fieldOfView = Mathf.Lerp(handCam.fieldOfView, handCamFOV, Time.deltaTime * 5);
        }
    }

    //---------- Power Related ----------

    //Assigns the new power as the player's power. 
    public void setPower(Power newPower)
    {
        currentPower = newPower;
    }

    //Returns the player's current power. 
    public Power getCurrentPower()
    {
        return currentPower;
    }

    //AmmoRegeration public call. 
    public void ammoRegeneration(float time)
    {
        StartCoroutine(ammoRegenerationCoroutine(time));
    }

    //Threaded method that applies ammo regen to the current weapon at regular intervals. 
    private IEnumerator ammoRegenerationCoroutine(float time)
    {
        if (currentGun != null)
        {
            float total = time;
            while (total > 0)
            {
                total -= 0.5f;
                yield return new WaitForSeconds(0.5f);
                if (currentGun.GetComponent<Gun>().CurrentMagazine < currentGun.GetComponent<Gun>().gunStats.magazineSize)
                {
                    int bulletsToAdd = (currentGun.GetComponent<Gun>().gunStats.magazineSize / 10);
                    if (bulletsToAdd < 1) { bulletsToAdd = 1; }
                    currentGun.GetComponent<Gun>().CurrentMagazine += bulletsToAdd;
                }
            }
        }
    }

    //Heals the player by the passed value. 
    public void heal(float f)
    {
        life.heal(f);
    }

    //Gives the player f seconds of invulnerability. 
    public void setInvulnerability(float f)
    {
        StartCoroutine(setInvulnerabilityCoroutine(f));
    }

    //Threaded routine to handle invulnerability. 
    private IEnumerator setInvulnerabilityCoroutine(float f)
    {
        life.invulnerable = true;
        yield return new WaitForSeconds(f);
        life.invulnerable = false;
    }

    //---------- Input Related ----------

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            isJumping = false;
        }
        else if (context.started && isGrounded() && !isJumping)
        {
            isJumping = true;
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (currentGun == null) { return; }

        if (context.started)
        {
            currentGun.GetComponent<Gun>().pullTrigger();
        }
        else if (context.canceled)
        {
            currentGun.GetComponent<Gun>().releaseTrigger();
        }

    }

    public void ADS(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isADS = true;
        }
        else if (context.canceled)
        {
            isADS = false;
        }
    }

    public void ChangeGun(InputAction.CallbackContext context)
    {
        if (!context.started && currentGun != null) { return; }
        SoundManager.GetInstance().PlaySound("event:/Click");
        if (context.ReadValue<Vector2>().y > 0)
        {
            if (currentGun == gun1 && gun3 != null)
            {
                equipGun(gun3);
            }
            else if (currentGun == gun1 && gun2 != null)
            {
                equipGun(gun2);
            }
            else if (currentGun == gun2 && gun1 != null)
            {
                equipGun(gun1);
            }
            else if (currentGun == gun3 && gun2 != null)
            {
                equipGun(gun2);
            }
        }
        else
        {
            if (currentGun == gun1 && gun2 != null)
            {
                equipGun(gun2);
            }
            else if (currentGun == gun2 && gun3 != null)
            {
                equipGun(gun3);
            }
            else if (currentGun == gun2 && gun1 != null)
            {
                equipGun(gun1);
            }
            else if (currentGun == gun3 && gun1 != null)
            {
                equipGun(gun1);
            }
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (!context.started || currentGun == null) { return; }
        currentGun.GetComponent<Gun>().reload();
    }

    public void ThrowGrenade(InputAction.CallbackContext context)
    {
        if (context.started && currentGrenades > 0)
        {
            currentGrenades--;
            currentGrenade = Instantiate(grenadePrefab);
            SoundManager.GetInstance().PlaySound("event:/Pull Pin");
            currentGrenade.transform.position = playerCamera.transform.position + Vector3.up * 0.5f; // keep the grenade out of view until thrown.
            currentGrenade.GetComponent<Rigidbody>().useGravity = false;
            currentGrenade.transform.SetParent(playerCamera.transform);
        }
        else if (context.canceled)
        {
            if (currentGrenade != null)
            {
                currentGrenade.GetComponent<Rigidbody>().useGravity = true;
                currentGrenade.transform.SetParent(null);
                currentGrenade.GetComponent<Rigidbody>().velocity += playerCamera.transform.forward * 20;
            }
        }
    }

    public void UsePower(InputAction.CallbackContext context)
    {
        if (context.started && currentPower != null)
        {
            if (!currentPower.usePower())
            {
                SoundManager.GetInstance().PlaySound("event:/Click");
            }
        }
    }

    public void Select(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3, interactableMask) &&
                hit.transform.GetComponent<GunShop>() != null)
            {
                SoundManager.GetInstance().PlaySound("event:/Click");
                hit.transform.gameObject.GetComponent<GunShop>().setInput(context.ReadValue<float>());
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3, interactableMask))
            {
                SoundManager.GetInstance().PlaySound("event:/Accept");
                hit.transform.gameObject.GetComponent<Interactable>().Interact();
            }
            else
            {
                SoundManager.GetInstance().PlaySound("event:/Click");
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if(GameManager.instance.paused)
            {
                GameManager.instance.unPause();
            }
            else
            {
                GameManager.instance.pause();
            }
        }
    }

    //---------- Weapon Related ----------

    //Adds the passed gun to the player's inventory. Gives the passed crate the player's old gun if necessary. 
    public void addGun(GameObject newGun, GunCrate crate)
    {
        GameObject gunToDrop = null;
        //Take in a new Gun object and equip it. 
        if (gun1 == null)
        {
            gun1 = newGun;
        }
        else if (gun2 == null)
        {
            gun2 = newGun;
        }
        else if (gun3 == null)
        {
            gun3 = newGun;
        }
        //if no gun slots available, swap out the currently equipped gun.
        else
        {
            if (currentGun == gun1)
            {
                gunToDrop = gun1;
                gun1 = newGun;
            }
            else if (currentGun == gun2)
            {
                gunToDrop = gun2;
                gun2 = newGun;
            }
            else if (currentGun == gun3)
            {
                gunToDrop = gun3;
                gun3 = newGun;
            }
        }

        equipGun(newGun);
        newGun.transform.parent = handCam.transform;
        Vector3 gunsOffset = newGun.GetComponent<Gun>().gunStats.gunOffset;
        Vector3 gunOffset = (handCam.transform.forward * gunsOffset.x) + (handCam.transform.up * gunsOffset.y) + (handCam.transform.right * gunsOffset.z);
        newGun.transform.position = handCam.transform.position + gunOffset;
        newGun.GetComponent<Gun>().recursiveSetLayer(newGun.transform);
        newGun.transform.rotation = handCam.transform.rotation;

        if (crate != null && gunToDrop != null)
        {
            crate.giveWeapon(gunToDrop.GetComponent<Gun>());
        }
        else if (gunToDrop != null)
        {
            Destroy(gunToDrop.gameObject);
        }
    }

    //Equips the passed gun. 
    private void equipGun(GameObject newGun)
    {
        if (currentGun != null)
        {
            currentGun.SetActive(false);
        }
        currentGun = newGun;
        currentGun.SetActive(true);
    }

    //Gets the currently equipped weapon. 
    public Gun getCurrentWeapon()
    {
        if (currentGun == null) { return null; }
        return currentGun.GetComponent<Gun>();
    }

    //Updates the gun logic. 
    private void updateGun()
    {
        if (currentGun == null) { return; }
        Gun gun = getCurrentWeapon();
        gun.currentRecoil = Mathf.Lerp(gun.currentRecoil, 0, gun.gunStats.handling / 100);

        //Animation.
        Vector3 gunsOffset = gun.gunStats.gunOffset;
        Vector3 gunOffset = (handCam.transform.forward * gunsOffset.x) + (handCam.transform.up * gunsOffset.y) + (handCam.transform.right * gunsOffset.z);
        Vector3 gunRecoilOffset = handCam.transform.up * (gun.currentRecoil / 100);
        currentGun.transform.position = handCam.transform.position + gunOffset + gunRecoilOffset;
        currentGun.transform.Rotate(new Vector3(-lookInput.y, lookInput.x, 0) * 0.1f); // Adding input to roatation for procedural anim.

        //Calculate kickback
        float gunKickback = gun.currentRecoil * 10;
        currentGun.transform.Rotate(new Vector3(-gunKickback, 0, 0)); // add recoil kickback

        currentGun.transform.rotation = Quaternion.Slerp(gun.transform.rotation, handCam.transform.rotation, (gun.gunStats.handling / 10) * Time.deltaTime); // Slerping backward to target rotation.
    }

    //Refills the player's ammo and grenades. 
    public void refillAmmo()
    {
        currentGrenades = 3;
        if (currentGun != null)
        {
            currentGun.GetComponent<Gun>().TotalAmmo = currentGun.GetComponent<Gun>().gunStats.maxAmmo;
        }
    }

    //---------- Physics and Control Related ----------

    //Returns a bool based on whether the player is on the ground. 
    private bool isGrounded()
    {
        if (Physics.CheckSphere(transform.position + Vector3.down, 0.2f, groundMask))
        {
            return true;
        }
        return false;
    }

    //Updates the player's accuracy stat based on the weapon and recent actions. 
    private void updateAccuracy()
    {
        float targetAccuracy = 0;
        float accuracyLerpSpeed = 0.8f;
        if (currentGun != null)
        {
            WeaponStats stats = getCurrentWeapon().gunStats;
            targetAccuracy = 100 - stats.accuracy;
            targetAccuracy += (lookInput.magnitude + rb.velocity.magnitude) / (stats.handling / 50);
            targetAccuracy += getCurrentWeapon().currentRecoil * 100;
            accuracyLerpSpeed = stats.handling / 100;
        }
        currentAccuracy = Mathf.Lerp(currentAccuracy, targetAccuracy, 0.5f + accuracyLerpSpeed / 2); // lerp between 0.5 and 1. Later half comes from gun handling.
    }

    //---------- Life Related ----------

    //Listener called methods. 
    private void onDeath()
    {
        GameManager.instance.playerDead();
    }

    private void onHurt()
    {
        SoundManager.GetInstance().PlaySound("event:/Player Hit");
    }
}
