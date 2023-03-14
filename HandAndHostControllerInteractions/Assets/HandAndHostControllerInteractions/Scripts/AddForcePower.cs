// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using UnityEngine;

public class AddForcePower : MonoBehaviour
{
    private Rigidbody _component;
    private float _delta = 0f;
    private Transform _rotorObj;

    public PowerController Controller;

    [SerializeField]
    private bool debug;

    public float Energy = 30f;
    public float IjiForce;

    [SerializeField]
    private bool isCharge;

    [Header("For Debug")]
    [SerializeField]
    private bool isRelease;

    [Range(0, 120)]
    public float MaxEnergy = 30f;

    public float MaxForce;

    public float MaxHight = 2f;

    [Header("Parameter")]
    [Range(0, 100)]
    public float MaxRotorSpeed = 30f;

    private float _posY = 0;

    private float _randingForce = 0f;


    private void Start()
    {
        _component = GetComponent<Rigidbody>();
        _rotorObj = transform.GetChild(0);
    }

    public void Release()
    {
        isCharge = false;
        isRelease = true;
    }


    public void Charge()
    {
        isCharge = true;
    }

    public void Catch()
    {
        isCharge = false;
        isRelease = false;
    }
    
    private void Update()
    {
        _rotorObj.localRotation *= Quaternion.AngleAxis(Energy / MaxRotorSpeed, Vector3.up);
        if (isRelease)
        {
            if (Energy > 0f)
            {
                if (MaxHight > transform.position.y)
                {
                    _component.isKinematic = false;
                    _component.useGravity = true;
                    _component.AddForce(MaxForce * Vector3.up, ForceMode.Force);
                }
                else if (MaxHight + .4f > transform.position.y)
                {
                    transform.position = new Vector3(transform.position.x,
                        MaxHight + .2f + 0.1f * Mathf.Sin(Energy * Mathf.PI), transform.position.z);
                    _component.isKinematic = true;
                    _component.useGravity = false;
                }

                Energy -= Time.deltaTime;
            }
            else
            {
                Energy = 0f;
                _component.AddForce(IjiForce * Vector3.up, ForceMode.Force);
                _component.isKinematic = false;
                _component.useGravity = true;
                isRelease = false;
                isCharge = false;
            }
        }
        else
        {
            if (!isCharge) return;
            if (debug) return;
            if (Energy < MaxEnergy)
                Energy += Controller.Force;
            else
                Energy = MaxEnergy;
        }
    }
}