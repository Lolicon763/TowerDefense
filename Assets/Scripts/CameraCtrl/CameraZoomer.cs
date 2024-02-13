using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    public Camera myCamera;
    public float zoomSpeed = 0.1f;
    public float minZoom = 1.0f;
    public float maxZoom = 10.0f;
    public float lerpSpeed = 10.0f; // �ṳ�����Ʋ��ʪ��t��
    private Vector3 dragStartScreenPos;
    private Vector3 dragStartWorldPos;
    private Vector3 dragOrigin;
    void Update()
    {
        if (Input.touchCount == 2)
        {
            // ������Ĳ�N�I
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // ����o����I���e�@�Ӧ�m
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // ����o���Ĳ�N�I���Z���ܤ�
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // ��X�ܤƶq
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // �����ṳ�����j�p�]�`�N�A�o�O�@��²�檺�ܨҡA�z�i��Ʊ�K�[��h���޿�H���T�a�����Y��^
            myCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;
            myCamera.orthographicSize = Mathf.Max(myCamera.orthographicSize, minZoom);
            myCamera.orthographicSize = Mathf.Min(myCamera.orthographicSize, maxZoom);
        }
        else if (Input.mousePresent) // �����O�_���ƹ�
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollData) > 0.01f)
            {
                Vector3 mousePosBefore = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));

                // ��s����j�p
                float newZoom = Mathf.Clamp(myCamera.orthographicSize - scrollData * zoomSpeed, minZoom, maxZoom);
                myCamera.orthographicSize = newZoom;

                Vector3 mousePosAfter = myCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, myCamera.nearClipPlane));

                // ��s�ṳ����m
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
