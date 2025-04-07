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

        Debug.Log("씬 전환");

        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera Tag 확인");
            yield break;
        }

        var controller = cam.GetComponent<CameraFollowing>();
        if (controller == null)
        {
            Debug.LogError("스크립트가 카메라에 없음!");
            yield break;
        }

        controller.target = transform;
        Debug.Log("카메라가 Player를 따라가도록 설정완료");
    }

}