using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraZoomer : MonoBehaviour
{
    public Camera myCamera;
    public float zoomSpeed = 0.1f;
    public float minZoom = 1.0f;
    public float maxZoom = 10.0f;
    public float lerpSpeed = 10.0f; // 攝像機平滑移動的速度
    private Vector3 dragOrigin;
    public Slider mySlider; // 參考到Slider
    private void Start()
    {
        mySlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }
    void Update()
    {
        bool isSingleTouchOrMouseClick = Input.touchCount == 1 || (Input.touchCount == 0 && Input.GetMouseButton(0));

        if (!BuildManager.BuildManagerInstance.IsSelecting && isSingleTouchOrMouseClick)
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }


        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePos = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));
            Vector3 move = dragOrigin - currentMousePos;
            myCamera.transform.position += new Vector3(move.x, move.y, 0);
        }
    }
    void ValueChangeCheck()
    {
        myCamera.orthographicSize = mySlider.value;
    }
    public void ResetCamera()
    {
        Camera camera = Camera.main;
        camera.transform.position = new Vector3(14, 10.6f, -10);
        camera.orthographicSize = 11.1f;
    }
}
