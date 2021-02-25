using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneController : VehicleController
{
    #region Private Fields

    private Controls controls;

    private float throttle;
    private Vector2 strafe;
    private float yaw;
    private float pitch;

    private Rigidbody rb;

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        controls = new Controls();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        SetupControls();

        controls.PlaneFlight.Pitch.AddBinding("<Mouse>/position/y")
            .WithProcessor("normalize(" +
                           "min=0," +
                           "max=" + Screen.height + "," +
                           "zero=" + Mathf.Floor(Screen.height / 2.0f) +
                           ")")
            .WithGroup("KB&M");

        controls.PlaneFlight.Yaw.AddBinding("<Mouse>/position/x")
            .WithProcessor("normalize(" +
                           "min=0," +
                           "max=" + Screen.width + "," +
                           "zero=" + Mathf.Floor(Screen.width / 2.0f) +
                           ")")
            .WithGroup("KB&M");
    }

    private void Update()
    {
        RotatePlayer();
        MovePlayer();
    }

    #endregion

    #region Protected Methods

    // Binds actions to values
    protected override void SetupControls()
    {
        controls.PlaneFlight.Throttle.performed += ctx => throttle = ctx.ReadValue<float>();
        controls.PlaneFlight.Throttle.canceled += ctx => throttle = 0;

        controls.PlaneFlight.Strafe.performed += ctx => strafe = ctx.ReadValue<Vector2>();
        controls.PlaneFlight.Strafe.canceled += ctx => strafe = Vector2.zero;

        controls.PlaneFlight.Yaw.performed += ctx => yaw = ctx.ReadValue<float>();
        controls.PlaneFlight.Yaw.canceled += ctx => yaw = 0;

        controls.PlaneFlight.Pitch.performed += ctx => pitch = ctx.ReadValue<float>();
        controls.PlaneFlight.Pitch.canceled += ctx => pitch = 0;
    }

    protected override void MovePlayer()
    {
        rb.AddForce(transform.forward * (throttle * thrustMultiplier * Time.deltaTime));
        rb.AddForce(transform.up * (strafe.y * thrustMultiplier * Time.deltaTime));
        rb.AddForce(transform.right * (strafe.x * thrustMultiplier * Time.deltaTime));
    }

    protected override void RotatePlayer()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(
                pitch * maxTurn * (invertPitch ? -1 : 1),
                transform.rotation.eulerAngles.y + yaw * yawSpeed,
                yaw * maxTurn * -1
            ),
            Time.deltaTime
        );
    }

    #endregion

    #region Public Methods

    public override void OnEnable()
    {
        base.OnEnable();
        controls.Enable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        controls.Disable();
    }

    #endregion
}