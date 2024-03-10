using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class BirdGlide : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private AnimalType aniaml = AnimalType.BIRD;

    private Rigidbody _rb;
    private PlayerMovement _pm;

    [Header("Gliding")]
    [SerializeField] float glideBoost = 2;
    [SerializeField] float staminaDrain = 20f;

    [Header("Keybinds")]
    [SerializeField]
    private KeyCode SpecialKey = KeyCode.Mouse0;

    [Header("Inputs")]
    private bool _specialInput;
    private Vector3 _glideforward;

    // Start is called before the first frame update
    public void Setup()
    {
        _pm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody>();
    }

    public void SwitchOf()
    {
        StopGlide();
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        if (_specialInput && _pm.GetStamina((int)aniaml) > 0 && !_pm.IsGroundet())
        {
            StartGlide();
        } else
        {
            StopGlide();
        }
    }

    private void FixedUpdate()
    {
        if (_pm.IsGliding())
        {
            glideMove();
        }
    }

    private void MyInput()
    {
        _specialInput = Input.GetKey(SpecialKey);
    }

    private void StartGlide()
    {
        _pm.SetGliding(true);
        _glideforward = orientation.forward;
    }

    void glideMove()
    {
        _pm.SetStamina((int)aniaml, _pm.GetStamina((int)aniaml) - (staminaDrain * Time.deltaTime));
        _rb.AddForce(_glideforward * glideBoost, ForceMode.Force);
    }

    private void StopGlide()
    {
        _pm.SetGliding(false);
    }
}
