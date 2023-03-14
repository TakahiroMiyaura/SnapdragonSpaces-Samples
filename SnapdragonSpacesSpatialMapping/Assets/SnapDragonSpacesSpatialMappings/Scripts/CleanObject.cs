// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using UnityEngine;

public class CleanObject : MonoBehaviour
{
    private bool _isDestory;

    public float Threshold = 8f;

    // Update is called once per frame
    private void Update()
    {
        if (Camera.main.transform.position.y - transform.position.y > Threshold)
            if (!_isDestory)
            {
                _isDestory = true;
                gameObject.transform.SetParent(null);
                Destroy(gameObject);
            }
    }
}