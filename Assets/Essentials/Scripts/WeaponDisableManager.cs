using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapons
{
    public GameObject WeaponObject;
    public string WeaponName;
}
public class WeaponDisableManager : MonoBehaviour
{
    public List<Weapons> IncludedWeapons;
    public static WeaponDisableManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public static string WeaponNameToToss;
    public int activeWeapon = 1;
    public static int activeToToss;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DisableAllWeapons();
            activeWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DisableAllWeapons();
            activeWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DisableAllWeapons();
            activeWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DisableAllWeapons();
            activeWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DisableAllWeapons();
            activeWeapon = 4;
        }
        if (IncludedWeapons[activeWeapon] != null)
        {
            IncludedWeapons[activeWeapon].WeaponObject.SetActive(true);
        }

        activeToToss = activeWeapon;
        WeaponNameToToss = IncludedWeapons[activeWeapon].WeaponName;
    }

    public void DisableAllWeapons()
    {
        for (int i = 0; i < IncludedWeapons.Count; i++) 
        {
            if (IncludedWeapons[i] != null)
            {
                IncludedWeapons[i].WeaponObject.SetActive(false);
            }
        }
    }
}