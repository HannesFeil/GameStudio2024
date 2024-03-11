using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    private GameManagementRefactored gameManagement;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform playerObj;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform focusLookAt;
    [SerializeField]
    private GameObject freeCam;
    [SerializeField]
    private GameObject focusCam;
    [SerializeField]
    private GameObject topdownCam;

    private CameraStyle _cameraStyle = CameraStyle.Free;

    public enum CameraStyle
    {
        Free,
        Focus,
        Topdown
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchCameraStyle(_cameraStyle);
    }

    private void Update()
    {
        if (_cameraStyle != CameraStyle.FREECAM && !Input.GetKey(KeyCode.Mouse1)) {
            SwitchCameraStyle(CameraStyle.FREECAM);
        } else if (_cameraStyle != CameraStyle.FOCUSCAM && Input.GetKey(KeyCode.Mouse1)) {
            SwitchCameraStyle(CameraStyle.FOCUSCAM);
        }

        if (_cameraStyle == CameraStyle.Free || _cameraStyle == CameraStyle.Topdown)
        {
            // rotate orientation
            var position = player.position;
            var position1 = transform.position;
            Vector3 viewDir = position - new Vector3(position1.x, position.y, position1.z);
            orientation.forward = viewDir;
        } else if(_cameraStyle == CameraStyle.Focus)
        {
            var position = focusLookAt.position;
            var transform1 = transform;
            var position1 = transform1.position;
            Vector3 dirToLookAt = position - new Vector3(position1.x, position.y, position1.z);
            orientation.forward = dirToLookAt;
        }
       
    }

    private void SwitchCameraStyle(CameraStyle style)
    {
        freeCam.SetActive(false);
        focusCam.SetActive(false);
        topdownCam.SetActive(false);

        freeCam.SetActive(style == CameraStyle.Free);
        focusCam.SetActive(style == CameraStyle.Focus);
        topdownCam.SetActive(style == CameraStyle.Topdown);
        _cameraStyle = style;
    }

    public CameraStyle GetcameraStyle()
    {
        return _cameraStyle;
    }

    public void SetcameraStyll(CameraStyle cameraStyle)
    {
        _cameraStyle = cameraStyle;
    }
}

