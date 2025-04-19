using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalOutside : MonoBehaviour
{
    private Plyaer player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("Title");
    }
}