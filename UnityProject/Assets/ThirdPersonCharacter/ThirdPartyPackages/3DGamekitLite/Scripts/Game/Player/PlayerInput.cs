using UnityEngine;
using System;
using System.Collections;
using Gamekit3D;
using TouchControlsKit;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movement;
    protected Vector2 m_Camera;
    protected bool m_Jump;
    protected bool m_AttackTap;
    protected bool m_AimInput;
    protected bool m_Pause;
    protected bool m_ExternalInputBlocked;

    public Vector2 MoveInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 CameraInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool JumpInput
    {
        get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool AimInput
    {
        get { return m_AimInput && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool AttackTap
    {
        get { return m_AttackTap && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Pause
    {
        get { return m_Pause; }
    }

    WaitForSeconds m_AttackInputWait;
    Coroutine m_AttackWaitCoroutine;
    float m_StartTapTime;
    Vector2 startTapPos;

    const float k_AttackInputDuration = 0.03f;
    const float k_AttackMaxTapDuration = 0.5f;
    const float k_AttackMaxTapDeltaPosition = 20f;

    void Awake()
    {
        m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);

        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
    }

    public bool mobileInput = true;

    private bool wasMovingTouchpad;
    void Update()
    {
        if(mobileInput)
        {
            var movement = TCKInput.GetAxis("Joystick");
            m_Movement.Set(movement.x,movement.y);

            Vector2 look = TCKInput.GetAxis("fireBtn") ;
            m_Camera.Set(look.x, look.y);
            m_Jump = TCKInput.GetAction( "jumpBtn", EActionEvent.Press );

            m_AimInput = TCKInput.GetAction("AimBtn", EActionEvent.Click);

            if(!wasMovingTouchpad)
            {
                m_AttackTap = TCKInput.GetAction("fireBtn", EActionEvent.Click);
            }

            wasMovingTouchpad = look != Vector2.zero;

            // if (TCKInput.GetAction( "fireBtn", EActionEvent.Press ))
            // {
            //     if (m_AttackWaitCoroutine != null)
            //         StopCoroutine(m_AttackWaitCoroutine);
            // }
            // m_AttackTap = false;
            // if(TCKInput.GetAction( "fireBtn", EActionEvent.Down))
            // {
            //     m_StartTapTime = Time.time;
            //     startTapPos = Input.mousePosition;
            // }
            // else if(TCKInput.GetAction( "fireBtn", EActionEvent.Up))
            // {
            //     if((startTapPos - (Vector2)Input.mousePosition).magnitude < k_AttackMaxTapDeltaPosition)
            //     {
            //         m_AttackTap = true;
            //     }
            // }

            // m_Pause = Input.GetButtonDown ("Pause");
        }
        else
        {
            m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            m_Jump = Input.GetButton("Jump");

            m_AimInput = Input.GetMouseButtonDown(1);

            if (Input.GetButton("Fire1"))
            {
                if (m_AttackWaitCoroutine != null)
                    StopCoroutine(m_AttackWaitCoroutine);
            }

            m_AttackTap = false;
            if(Input.GetButtonDown("Fire1"))
            {
                m_StartTapTime = Time.time;
                startTapPos = Input.mousePosition;
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                if((startTapPos - (Vector2)Input.mousePosition).magnitude < k_AttackMaxTapDeltaPosition)
                {
                    m_AttackTap = true;
                }
            }

            // m_Pause = Input.GetButtonDown ("Pause");
        }

        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            mobileInput = !mobileInput;
            Debug.Log($"Switched input source: mobile input = {mobileInput}");
        }
        #endif
    }


    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }
}
