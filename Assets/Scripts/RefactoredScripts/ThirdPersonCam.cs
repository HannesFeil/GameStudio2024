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

    private CameraStyle _cameraStyle = CameraStyle.FREECAM;

    public enum CameraStyle
    {
        FREECAM,
        FOCUSCAM,
        TOPDOWN
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SwitchCameraStyle(_cameraStyle);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchCameraStyle(CameraStyle.FREECAM);
        } 
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchCameraStyle(CameraStyle.FOCUSCAM);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchCameraStyle(CameraStyle.TOPDOWN);
        }

        if (_cameraStyle == CameraStyle.FREECAM || _cameraStyle == CameraStyle.TOPDOWN)
        {
            // rotate orientation
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir;
        } else if(_cameraStyle == CameraStyle.FOCUSCAM)
        {
            Vector3 dirToLookAt = focusLookAt.position - new Vector3(transform.position.x, focusLookAt.position.y, transform.position.z);
            orientation.forward = dirToLookAt;
        }
       
    }

    private void SwitchCameraStyle(CameraStyle style)
    {
        freeCam.SetActive(false);
        focusCam.SetActive(false);
        topdownCam.SetActive(false);

        freeCam.SetActive(style == CameraStyle.FREECAM);
        focusCam.SetActive(style == CameraStyle.FOCUSCAM);
        topdownCam.SetActive(style == CameraStyle.TOPDOWN);
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

