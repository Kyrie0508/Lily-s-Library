using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    void Start()
    {
        // 플레이어 찾기 (태그로)
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // 플레이어 위치를 이 오브젝트 위치로 이동
            player.transform.position = transform.position;
        }
        else
        {
            Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }
}
