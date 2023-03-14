// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System.Collections.Generic;
using UnityEngine;

public class ObjectResetter : MonoBehaviour
{
    private readonly List<float> InitialPosY = new();
    private GameObject root;
    public GameObject Table;

    // Start is called before the first frame update
    private void Start()
    {
        for (var i = 0; i < transform.childCount; i++) InitialPosY.Add(transform.GetChild(i).transform.position.y);

        root = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        if (root.gameObject.transform.position.y < -4f)
            for (var i = 0; i < transform.childCount; i++)
                transform.GetChild(i).transform.position = new Vector3(Table.transform.position.x, InitialPosY[i],
                    Table.transform.position.z);
    }
}