using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Grappling : MonoBehaviour
{
    private SnakeMove _sm;
    private PlayerManagement _pm;
    public Transform tip;
    public LayerMask whatToGrapple;
    public LineRenderer lr;


    [SerializeField] private float maxDistance;
    [SerializeField] private float delayTime;
    [SerializeField] private float overshootYAxis;

    private Vector3 _grapplePoint;

    public bool GrapplingNow;

    [SerializeField] private float grapplingCd;
    private float _grapplingCdTimer;

    // Start is called before the first frame update
    void Start()
    {
        lr.enabled = false;
        _sm = GetComponent<SnakeMove>();
    }

    public void StartGrapple()
    {
        if (_grapplingCdTimer > 0) return;

        GrapplingNow = true;

        RaycastHit hit;
        if (Physics.Raycast(_pm.GetCamTransform().position, _pm.GetCamDir(), out hit, maxDistance, whatToGrapple))
        {
            _grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), delayTime);
        }
        else
        {
            
            _grapplePoint = _pm.GetCamTransform().position + _pm.GetCamDir() * maxDistance;

            Invoke(nameof(StopGrapple), delayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, _grapplePoint);
    }

    public void ExecuteGrapple()
    {
        float grapplePointRelativeY = _grapplePoint.y - transform.position.y;
        float highestPointOnArc = Mathf.Max(grapplePointRelativeY, 0) + overshootYAxis;

        _sm.JumpToPosition(_grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        GrapplingNow = false;
        _grapplingCdTimer = grapplingCd;
        lr.enabled = false;
    }

    private void Update()
    {
        if (_grapplingCdTimer > 0)
        {
            _grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if(GrapplingNow)
        {
            lr.SetPosition(0, tip.position);
        }
    }

    public void SetPm(PlayerManagement pm)
    {
        _pm = pm;
    }
}
