// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using UnityEngine;
using UnityEngine.Events;

public class LastJointObj : MonoBehaviour
{
    public UnityEvent OnCollisionSpatialMeshEvent;
    
    private void Start()
    {
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) OnCollisionSpatialMeshEvent?.Invoke();
    }
}