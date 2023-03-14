// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PowerController : MonoBehaviour
{
    private float _angle;

    private float _debugangle;

    private Vector2 _prev;

    private float _timer;

    public bool DebugMode = false;

    public float Force;

    [Range(0, 100)]
    public float speed = 1f;

    public TextMeshProUGUI Text;

    public InputActionReference TouchpadInputAction;
    
    private void Start()
    {
    }

    public void ActionStart()
    {
        _prev = Vector2.up;
    }

    public void SetData(PadObject pose)
    {
        Text.text = pose.SelectedGameObject.name;
    }

    private void Update()
    {
        if (_timer > 0f) Force = 0f;
        _timer += Time.deltaTime;
        var next = Vector2.zero;
        if (DebugMode)
        {
            if (_debugangle > 2 * Mathf.PI) _debugangle -= 2 * Mathf.PI;

            next = new Vector2(Mathf.Cos(_debugangle), Mathf.Sin(_debugangle));

            _debugangle += 2 * Mathf.PI * Time.deltaTime;
        }
        else
        {
            next = TouchpadInputAction.action.ReadValue<Vector2>().normalized;
        }


        var angle = Vector3.Angle(_prev, next);
        _angle += angle;
        _prev = next;
        if (_angle >= 360)
        {
            _angle = 0f;
            Force = speed / _timer;
            _timer = 0f;
        }
    }
}