
using System;
using UnityEngine;

public class DollyZoom : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _target;
    public float time;
    public float speed;
    public float frustumHeight;
    public float distance;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        distance = Vector3.Distance(_camera.transform.position, _target.position);
        frustumHeight = 2.0f * distance * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    private void Update()
    {
        
        distance = Vector3.Distance(_camera.transform.position, _target.position);
        _camera.fieldOfView = 2.0f * Mathf.Atan(frustumHeight * 0.5f / distance) * Mathf.Rad2Deg; 
    }
}
