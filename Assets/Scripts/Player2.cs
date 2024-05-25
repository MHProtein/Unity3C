using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player2 : MonoBehaviour
{ 
    public float speed;
    public float gravity;
    public LayerMask layerMask;
    public float turnSmoothTime = 0.1f;
    public float stepOffset = 0.3f;
    public float skin = 0.05f;
    private Vector3 m_inputDir;
    private bool m_isGround = false;
    private Vector3 m_velocity;
    private float m_height;
    private float _turnSmoothVelocity;
    float x;
    float z;
    float y;
    RaycastHit hit;
    [SerializeField] private Transform _camera;
    private void Start()
    {
        m_height = GetComponentInChildren<CapsuleCollider>().height;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;
        y = Input.GetButton("Jump") ? 5.0f : 0.0f;
    }

    private void FixedUpdate()
    {
        m_inputDir = new Vector3(x, y, z);
        
        if (m_isGround)
        {
            m_velocity = m_inputDir * (speed * Time.fixedDeltaTime);
            m_velocity = Vector3.ProjectOnPlane(m_velocity, hit.normal);
            m_velocity.y = m_inputDir.y * (speed * Time.fixedDeltaTime);
            m_isGround = false;
        }
        else
        {
            m_velocity.y -= gravity * Time.fixedDeltaTime;
        }
    
        Debug.DrawLine(transform.position + Vector3.up,
            transform.position + Vector3.up + m_velocity, Color.black);
        Debug.DrawLine(transform.position + Vector3.up,
            transform.position + Vector3.up + m_velocity, Color.black);

 
        
        if (Physics.Raycast(transform.position + new Vector3(0, stepOffset, 0), 
                new Vector3(m_velocity.x, 0.0f, m_velocity.z), out hit,
                0.5f + float.Epsilon))
        {
            float normalside = Vector3.Dot(m_velocity, hit.normal);
            m_velocity -= hit.normal * normalside;
        }
        transform.position += m_velocity * Time.fixedDeltaTime;
        
        if (Physics.Raycast(transform.position + new Vector3(0, stepOffset,0),
                Vector3.down, out hit, 2000.0f, layerMask))
        {
            if (gameObject.transform.position.y - hit.point.y <= skin)
            {
                //snap
                m_isGround = true;
                transform.position = new Vector3(transform.position.x, 
                    hit.point.y, transform.position.z);
            }
            else
            {
                m_isGround = false;
            }
        }
        
        if (m_velocity.y > 0)
        {
            m_isGround = false;
        }
        
    }
}

