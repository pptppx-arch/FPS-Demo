using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI health;
    public TextMeshProUGUI activeWeaponName;
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Sprite MainWeaponImageUI;
    public Sprite SideWeaponImageUI;
    public Sprite Equipment1;
    public Sprite Equipment2;
    public Sprite Equipment3;

    public static UIManager Instance { get; set; }
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
    
    public void Update()
    {
        Debug.Log($"{RaycastFiring.magAmmoUI}");
        health.text = $"{PlayerHealth.health}";
        magazineAmmoUI.text = $"{RaycastFiring.magAmmoUI}";
        totalAmmoUI.text = $"{RaycastFiring.totalAmmoUI}";
        activeWeaponName.text = $"{WeaponDisableManager.WeaponNameToToss}";
    }
}
