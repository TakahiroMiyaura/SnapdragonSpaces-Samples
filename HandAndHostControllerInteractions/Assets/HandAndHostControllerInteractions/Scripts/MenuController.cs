// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class PadReleaseEvent : UnityEvent<PadObject>
{
}

public class PadObject
{
    public Vector2 Pose { set; get; }
    public GameObject SelectedGameObject { set; get; }
}

public class MenuController : MonoBehaviour
{
    private bool _isPress;
    private float _offTimer;
    private float _onTimer;

    private Vector2 _pose;
    private RectTransform _rectTransform;

    [Range(-1, 1)]
    public float DebugX;

    [Range(-1, 1)]
    public float DebugY;

    public bool IsDebug;

    public PadReleaseEvent OnRapRelease;

    public Vector2 PadSize;

    public GameObject SelectedTarget;

    public float Threashold = 0.5f;

    public InputActionReference TouchpadInputAction;
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (TouchpadInputAction.action.triggered || IsDebug)
        {
            _onTimer += Time.deltaTime;
            if (IsDebug)
                _pose = new Vector2(DebugX, DebugY);
            else
                _pose = TouchpadInputAction.action.ReadValue<Vector2>();
            _pose = new Vector2(_pose.x * PadSize.x / 2f, _pose.y * PadSize.y / 2f);
            _rectTransform.localPosition = new Vector3(_pose.x, _pose.y, _rectTransform.localPosition.z);
        }
        else
        {
            if (_onTimer > Threashold)
            {
                _onTimer = 0f;
                _isPress = true;
            }

            if (_isPress)
            {
                if (_offTimer > Threashold)
                {
                    var padObject = new PadObject();
                    padObject.SelectedGameObject = SelectedTarget;
                    padObject.Pose = _pose;
                    OnRapRelease?.Invoke(padObject);
                    _offTimer = 0;
                    _onTimer = 0;
                }

                _offTimer += Time.deltaTime;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);
        SelectedTarget = other.gameObject;
    }
}