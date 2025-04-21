using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalOutside : MonoBehaviour
{
    private Plyaer player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("추후 구현 예정");
        //FindAnyObjectByType<FadeOutController>().StartFadeOut("다음 맵");
    }
}