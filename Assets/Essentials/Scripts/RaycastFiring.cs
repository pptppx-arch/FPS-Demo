using Unity.VisualScripting;
using UnityEngine;

public class RaycastFiring : MonoBehaviour
{
    [Header("Bot Settings")]
    public bool IsBotUsing;

    [Header("Weapon Stats")]
    public int weaponDamage;
    public float FireRateDelay = 0.2f;
    public float spreadIntensity;
    public float distance = 30f;

    [Header("Magazine & Ammo")]
    public int magazineSize;
    public int TotalBullets;
    public bool autoReloadOnEmpty;

    [Header("Firing Mode")]
    public FiringMode currentFiringMode;
    public int bulletsPerBurst = 3;

    [Header("Effects")]
    public GameObject MuzzleFlash;
    public float MuzzleTime = 0.1f;
    public GameObject ImpactEffect;
    public float ImpactTime = 0.5f;

    [Header("References")]
    public Transform bulletSpawn;
    private Animator animator;

    // Internal State
    private int bulletsLeft;
    private int BurstBulletsLeft;
    public bool isFiring; // Public so BotController can set it
    private bool readyToFire;
    private bool isReloading;

    // Broadcast (UI)
    public static int magAmmoUI;
    public static int totalAmmoUI;
    public static Sprite weaponSprite;

    public enum FiringMode { Single, Burst, Auto }

    private void Awake()
    {
        bulletsLeft = magazineSize;
        BurstBulletsLeft = bulletsPerBurst;
        readyToFire = true;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isReloading) return;

        if (!IsBotUsing)
        {
            HandlePlayerInput();
            HandlePlayerReloadInput();
        }
        else
        {
            HandleBotReloadLogic();
        }

        if (readyToFire && isFiring && bulletsLeft >= 1 && !isReloading)
        {
            if (currentFiringMode == FiringMode.Burst && BurstBulletsLeft <= 0) return;
            FireWeapon();
        }

        // UI Broadcast logic (simplified for example)
        magAmmoUI = bulletsLeft;
        totalAmmoUI = TotalBullets;
    }

    public void FireWeapon()
    {
        if (!readyToFire || bulletsLeft <= 0 || isReloading) return;

        readyToFire = false;
        bulletsLeft--;

        // Handle burst count
        if (currentFiringMode == FiringMode.Burst)
        {
            BurstBulletsLeft--;
        }

        // Apply spread
        Vector3 direction = transform.forward;
        if (spreadIntensity > 0)
        {
            direction = ApplySpread(direction);
        }

        RaycastHit hit;
        if (Physics.Raycast(bulletSpawn.position, direction, out hit, distance))
        {
            Debug.DrawRay(bulletSpawn.position, direction * hit.distance, Color.yellow);

            if (MuzzleFlash != null)
            {
                GameObject EffectMuzzle = Instantiate(MuzzleFlash, bulletSpawn.position, Quaternion.identity);
                Destroy(EffectMuzzle, MuzzleTime);
            }

            if (ImpactEffect != null)
            {
                GameObject EffectImpact = Instantiate(ImpactEffect, hit.point, Quaternion.identity);
                Destroy(EffectImpact, ImpactTime);
            }

            if (SoundEffectManager.Instance != null)
                SoundEffectManager.Instance.PlayFiringSound();
        }

        // Handle shot reset
        if (currentFiringMode == FiringMode.Burst && BurstBulletsLeft > 0)
        {
            // Rapid fire during burst
            Invoke(nameof(FireWeapon), 0.05f);
        }
        else
        {
            Invoke(nameof(ResetShot), FireRateDelay);
        }

        if (bulletsLeft == 0 && autoReloadOnEmpty)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (bulletsLeft == magazineSize || TotalBullets <= 0 || isReloading) return;

        isReloading = true;
        if (SoundEffectManager.Instance != null)
            SoundEffectManager.Instance.PlayReloadingSound();

        if (animator != null)
            animator.SetTrigger("Reload");

        Invoke(nameof(ReloadCompleted), 2.0f); // Default reload time if not specified
    }

    private void ReloadCompleted()
    {
        int needed = magazineSize - bulletsLeft;
        int toAdd = Mathf.Min(needed, TotalBullets);

        bulletsLeft += toAdd;
        TotalBullets -= toAdd;

        isReloading = false;
        BurstBulletsLeft = bulletsPerBurst;
    }

    private void ResetShot()
    {
        readyToFire = true;
        if (currentFiringMode != FiringMode.Burst || BurstBulletsLeft <= 0)
        {
            BurstBulletsLeft = bulletsPerBurst;
        }
        isFiring = false;
    }

    private Vector3 ApplySpread(Vector3 direction)
    {
        Vector3 spread = new Vector3(
            Random.Range(-spreadIntensity, spreadIntensity),
            Random.Range(-spreadIntensity, spreadIntensity),
            Random.Range(-spreadIntensity, spreadIntensity)
        );
        return (direction + spread).normalized;
    }

    private void HandlePlayerInput()
    {
        if (currentFiringMode == FiringMode.Auto)
            isFiring = Input.GetMouseButton(0);
        else
            isFiring = Input.GetMouseButtonDown(0);
    }

    private void HandlePlayerReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    private void HandleBotReloadLogic()
    {
        if (bulletsLeft == 0 && TotalBullets > 0 && !isReloading)
            Reload();
    }
}
