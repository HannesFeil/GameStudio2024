using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private SnakeMove sm;
    public Transform orientation;
    public Transform tip;
    public LayerMask whatToGrapple;
    public LineRenderer lr;


    [SerializeField] private float maxDistance;
    [SerializeField] private float DelayTime;
    [SerializeField] private float overshootYAxis;

    private Vector3 grapplePoint;

    private bool grappling;

    [SerializeField] private float grapplingCD;
    private float grapplingCDTimer;

    // Start is called before the first frame update
    void Start()
    {
        lr.enabled = false;
        sm = GetComponent<SnakeMove>();
    }

    public void StartGrapple()
    {
        if (grapplingCDTimer > 0) return;

        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(orientation.position, orientation.forward, out hit, maxDistance, whatToGrapple))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), DelayTime);
        }
        else
        {
            grapplePoint = orientation.position + orientation.forward * maxDistance;

            Invoke(nameof(StopGrapple), DelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    public void ExecuteGrapple()
    {
        float grapplePointRelativeY = grapplePoint.y - transform.position.y;
        float highestPointOnArc = Mathf.Max(grapplePointRelativeY, 0) + overshootYAxis;

        sm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        grappling = false;
        grapplingCDTimer = grapplingCD;
        lr.enabled = false;
    }

    private void Update()
    {
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if(grappling)
        {
            lr.SetPosition(0, tip.position);
        }
    }
}
