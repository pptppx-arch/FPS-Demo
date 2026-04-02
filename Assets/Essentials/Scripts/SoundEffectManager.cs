using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundItem
{
    public AudioClip FiringEffect;
    public AudioClip ReloadingEffect;
}

public class SoundEffectManager : MonoBehaviour
{
    public AudioSource audioSource;

    public List<SoundItem> soundList = new List<SoundItem>();
    public static SoundEffectManager Instance { get; set; }
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

    public void PlayFiringSound()
    {
        int weaponIndex = (WeaponDisableManager.activeToToss);
        SoundItem soundItem = soundList[weaponIndex];
        AudioClip audioClip = soundItem.FiringEffect;
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayReloadingSound()
    {
        int weaponIndex = (WeaponDisableManager.activeToToss);
        SoundItem soundItem = soundList[weaponIndex];
        AudioClip audioClip = soundItem.ReloadingEffect;
        audioSource.PlayOneShot(audioClip);
    }
}
