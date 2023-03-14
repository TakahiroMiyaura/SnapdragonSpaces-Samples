// Copyright (c) 2023 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARMeshManager))]
public class SpatialCollisonController : MonoBehaviour
{
    private static readonly object _lockObj = new();
    private readonly Dictionary<string, MeshInfo> _collisions = new();
    private ARMeshManager _arMeshManager;
    private readonly MeshFilter[] _deleteMeshFilters = Array.Empty<MeshFilter>();
    private float _timer;
    private readonly string _logsText = "";
    private string _delText = "";
    private string _addText = "";
    private string _updText = "";

    [Header("Parameters ...")]
    public float DelayStart = 5f;
    public GameObject Root;
    public Material mat;

    [Header("Debug ...")]
    public TextMeshProUGUI Logs;
    public TextMeshProUGUI Add;
    public TextMeshProUGUI Del;
    public TextMeshProUGUI Upd;


    // Start is called before the first frame update
    private void Awake()
    {
        _arMeshManager = GetComponent<ARMeshManager>();
        if (Root == null) Root = new GameObject("Spatial Objects");
    }

    private void OnEnable()
    {
        _arMeshManager.meshesChanged += ArMeshManagerOnMeshesChanged;
    }

    private void OnDisable()
    {
        _arMeshManager.meshesChanged -= ArMeshManagerOnMeshesChanged;
    }

    private void ArMeshManagerOnMeshesChanged(ARMeshesChangedEventArgs obj)
    {
        StartCoroutine(MeshUpdate(obj.added, obj.updated, obj.removed));
    }

    private IEnumerator MeshUpdate(List<MeshFilter> added, List<MeshFilter> updated, List<MeshFilter> removed)
    {
        _addText = added.Count.ToString();
        _updText = updated.Count.ToString();
        _delText = removed.Count.ToString();
        lock (_lockObj)
        {
            foreach (var addedMeshFilter in added)
            {
                var key = addedMeshFilter.gameObject.name;

                if (!_collisions.TryGetValue(key, out var mFilter))
                {
                    var colGameObject = new GameObject(key);
                    colGameObject.transform.SetParent(Root.transform, false);
                    mFilter = new MeshInfo(colGameObject.AddComponent<MeshFilter>(),
                        colGameObject.AddComponent<MeshCollider>(), colGameObject.AddComponent<MeshRenderer>());

                    _collisions[key] = mFilter;
                }

                SetMeshInfo(addedMeshFilter.transform, mFilter, addedMeshFilter.sharedMesh);
            }

            foreach (var updMeshFilter in updated)
            {
                var key = updMeshFilter.gameObject.name;

                SetMeshInfo(updMeshFilter.transform, _collisions[key], updMeshFilter.sharedMesh);
            }

            if (_collisions.Count > _deleteMeshFilters.Length)
                foreach (var deleteMeshFilter in _deleteMeshFilters)
                {
                    var key = deleteMeshFilter.gameObject.name;
                    var removeData = _collisions[key];
                    _collisions.Remove(key);
                    Destroy(removeData.GameObject);
                }
        }

        yield return null;
    }

    public void SetMaterial(Material updMaterial)
    {
        mat = updMaterial;
    }

    private void SetMeshInfo(Transform sourceTransform, MeshInfo info, Mesh mesh)
    {
        info.GameObject.transform.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
        info.GameObject.transform.localScale = sourceTransform.localScale;
        info.Filter.sharedMesh = mesh;
        info.Collider.sharedMesh = mesh;
        info.Renderer.material = mat;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_timer > DelayStart)
            _arMeshManager.enabled = true;
        else
            _timer += Time.deltaTime;

        Add.text = _addText;
        Upd.text = _updText;
        Del.text = _delText;
        Logs.text = _logsText;
    }

    internal class MeshInfo
    {
        public MeshFilter Filter { get; }

        public MeshCollider Collider { get; }
        public MeshRenderer Renderer { get; }

        public GameObject GameObject { get; }

        public MeshInfo(MeshFilter filter, MeshCollider collider, MeshRenderer renderer)
        {
            Filter = filter;
            Collider = collider;
            Renderer = renderer;
            GameObject = filter.gameObject;
        }
    }
}