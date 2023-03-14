// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MenuCommands : MonoBehaviour
{
    public GameObject RootObj;

    public void ExitCommand()
    {
        Application.Quit();
    }

    public void ResetCommand()
    {
        var childTransforms = RootObj.GetComponentsInChildren<Transform>();
        foreach (var childTransform in childTransforms)
        {
            if(!childTransform.gameObject.Equals(RootObj))
                Destroy(childTransform.gameObject);
        }
    }
}