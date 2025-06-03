using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip purchaseClip;
    public AudioClip attackClip;
    public AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayPurchaseSound()
    {
        audioSource.PlayOneShot(purchaseClip);
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackClip);
    }
}