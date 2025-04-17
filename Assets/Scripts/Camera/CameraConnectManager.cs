using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraConnectManager : MonoBehaviour
{
    public Transform target;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ConnectCameraAfterSceneLoads());
    }

    IEnumerator ConnectCameraAfterSceneLoads()
    {
        yield return null;
        

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera Tag 확인");
            yield break;
        }

        var controller = cam.GetComponent<CameraFollowing>();
        if (controller == null)
        {
            yield break;
        }

        controller.target = transform;
    }

}