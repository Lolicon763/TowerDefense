using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    public Camera myCamera;
    public float zoomSpeed = 0.1f;
    public float minZoom = 1.0f;
    public float maxZoom = 10.0f;
    public float lerpSpeed = 10.0f; // 攝像機平滑移動的速度
    private Vector3 dragStartScreenPos;
    private Vector3 dragStartWorldPos;
    private Vector3 dragOrigin;
    void Update()
    {
        if (Input.touchCount == 2)
        {
            // 獲取兩個觸摸點
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // 獲取這兩個點的前一個位置
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 獲取這兩個觸摸點的距離變化
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 找出變化量
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 改變攝像機的大小（注意，這是一個簡單的示例，您可能希望添加更多的邏輯以更精確地控制縮放）
            myCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;
            myCamera.orthographicSize = Mathf.Max(myCamera.orthographicSize, minZoom);
            myCamera.orthographicSize = Mathf.Min(myCamera.orthographicSize, maxZoom);
        }
        else if (Input.mousePresent) // 偵測是否有滑鼠
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollData) > 0.01f)
            {
                Vector3 mousePosBefore = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));

                // 更新正交大小
                float newZoom = Mathf.Clamp(myCamera.orthographicSize - scrollData * zoomSpeed, minZoom, maxZoom);
                myCamera.orthographicSize = newZoom;

                Vector3 mousePosAfter = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));

                // 更新攝像機位置
                Vector3 offset = mousePosAfter - mousePosBefore;
                myCamera.transform.position -= offset;
            }
        }
        if (!BuildManager.BuildManagerInstance.IsSelecting)
        {
            Drag();
        }
    }
    private void Drag()
    {

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));
            Vector3 move = dragOrigin - currentMousePos;
            myCamera.transform.position += new Vector3(move.x, move.y, 0);
        }
    }
    public void ResetCamera()
    {
        Camera camera = Camera.main;
        camera.transform.position = new Vector3(14, 10.6f, -10);
        camera.orthographicSize = 11.1f;
    }
}
