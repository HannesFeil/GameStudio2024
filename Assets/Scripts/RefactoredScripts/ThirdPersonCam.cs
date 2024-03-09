using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform playerObj;
    [SerializeField]
    private Rigidbody rb;

    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x,player.position.y,transform.position.z);
        orientation.forward = viewDir;
    }
}

