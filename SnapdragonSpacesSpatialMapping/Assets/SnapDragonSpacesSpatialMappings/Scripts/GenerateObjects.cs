// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using MixedReality.Toolkit;
using MixedReality.Toolkit.Subsystems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class GenerateObjects : MonoBehaviour
{
    private HandsAggregatorSubsystem _aggregator;

    private GameObject _balloonObj;

    private float _timer;

    private bool _isHandMenuVisible ;

    private Collider _leftCollider;
    private Collider _rightCollider;

    public GameObject ShootingObj;

    public GameObject BalloonObj;

    public GameObject RootObj;

    public Collider Collider;

    [Range(0, 1)]
    public float BalloonSize;

    [Range(0, 1)]
    public float ShootingRate = 0.5f;


    [Range(0, 10)]
    public float Velocity = 4f;

    [Header("Offset")]
    public Quaternion RotationOffset;

    public float PositionOffset;

    private void Awake()
    {
        _aggregator = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();
        _leftCollider = GameObject.Instantiate(Collider.gameObject).GetComponent<Collider>();
        _leftCollider.enabled = false;
        _rightCollider = GameObject.Instantiate(Collider.gameObject).GetComponent<Collider>();
        _rightCollider.enabled = false;
    }

    private void Update()
    {
        if (_isHandMenuVisible)
        {
            _leftCollider.enabled = false;
            _rightCollider.enabled = false;
            return;
        }

        
        Collider.enabled = true;
        _aggregator.TryGetPinchProgress(XRNode.LeftHand, out var isLeftReadyToPinch, out var isLeftPinching, out _);
        _aggregator.TryGetPinchProgress(XRNode.RightHand, out var isRightReadyToPinch, out var isRightPinching, out _);


        _leftCollider.enabled = isLeftReadyToPinch;
        _rightCollider.enabled = isRightReadyToPinch;

        if (isLeftReadyToPinch)
        {
            _aggregator.TryGetJoint(TrackedHandJoint.Palm, XRNode.LeftHand, out var jointPose);
            _leftCollider.transform.SetPositionAndRotation(jointPose.Position, jointPose.Rotation);
            if (isLeftPinching)
            {
                GenerateBalloonObj(jointPose);
            }
            else
            {
                _balloonObj = null;
            }
        }
        else
        {
            _balloonObj = null;
        }

        if (isRightPinching)
        {

            _aggregator.TryGetJoint(TrackedHandJoint.IndexDistal, XRNode.RightHand, out var jointPose);
            _rightCollider.transform.SetPositionAndRotation(jointPose.Position, jointPose.Rotation);
            if (isRightReadyToPinch)
            {
                GenerateGravityObj(jointPose);
            }
        }
    }

    private void GenerateGravityObj(HandJointPose handedTransform)
    {
        if (_timer > (1f - ShootingRate) * 2f)
        {
            _timer = 0f;
            var obj = Instantiate(ShootingObj);
            obj.transform.parent = RootObj.transform;
            var rigidBody = obj.GetOrAddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            obj.transform.rotation = handedTransform.Rotation;
            obj.transform.position = handedTransform.Position + Quaternion.AngleAxis(-45f, handedTransform.Right) * handedTransform.Forward * PositionOffset;
            rigidBody.AddForce(Quaternion.AngleAxis(-45f, handedTransform.Right)*handedTransform.Forward * Velocity, ForceMode.Impulse);
            rigidBody.useGravity = true;
        }

        _timer += Time.deltaTime;
    }

    private void GenerateBalloonObj(HandJointPose handedTransform)
    {
        if (_balloonObj == null)
        {
            _balloonObj = Instantiate(BalloonObj);
            _balloonObj.transform.parent = RootObj.transform;
            _balloonObj.transform.localScale = Vector3.zero;
            _balloonObj.GetOrAddComponent<Rigidbody>().useGravity = false;
        }


        _balloonObj.transform.rotation = RotationOffset * handedTransform.Rotation;
        _balloonObj.transform.position = handedTransform.Position + _balloonObj.transform.forward * PositionOffset;

        _balloonObj.transform.localScale =
            Vector3.Lerp(_balloonObj.transform.localScale, Vector3.one * BalloonSize, 0.01f);
    }

    public void IsHandMenuVisible(bool visible)
    {
        _isHandMenuVisible = visible;
    }
}