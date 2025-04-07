using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraFollowing : MonoBehaviour
{
    public Transform target; // 플레이어
    public Grid grid;        // 타일맵 부모
    public float smoothSpeed = 5f;

    private Vector2 minPosition;
    private Vector2 maxPosition;
    private float camHalfWidth;
    private float camHalfHeight;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;

        if (grid != null)
        {
            Bounds totalBounds = new Bounds();
            Tilemap[] tilemaps = grid.GetComponentsInChildren<Tilemap>();
            bool first = true;

            foreach (Tilemap tilemap in tilemaps)
            {
                if (first)
                {
                    totalBounds = tilemap.localBounds;
                    first = false;
                }
                else
                {
                    totalBounds.Encapsulate(tilemap.localBounds);
                }
            }

            minPosition = totalBounds.min;
            maxPosition = totalBounds.max - new Vector3(1f, 0f, 0f); // 오른쪽만 보정
        }
        else
        {
            Debug.LogWarning("Grid가 할당되지 않았습니다.");
        }
    }


    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPos = new Vector3(target.position.x, target.position.y, transform.position.z);

            // 제한 영역 적용
            float clampX = Mathf.Clamp(desiredPos.x, minPosition.x + camHalfWidth, maxPosition.x - camHalfWidth);
            float clampY = Mathf.Clamp(desiredPos.y, minPosition.y + camHalfHeight, maxPosition.y - camHalfHeight);

            Vector3 clampedPos = new Vector3(clampX, clampY, transform.position.z);

            transform.position = Vector3.Lerp(transform.position, clampedPos, smoothSpeed * Time.fixedDeltaTime);
        }
    }
}
