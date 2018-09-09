﻿using UnityEngine;

public abstract class BaseObject
{
    protected int _id = 0;
    protected string _name = "Object";
    protected int _group = 0;
    protected int _subGroup = 0;

    protected string _resourceName = "";

    protected GameObject _resource = null;

    protected bool _isInit = false;

    public int Id
    {
        get { return _id; }
    }

    public string Name
    {
        get { return _name; }
    }

    public int Group
    {
        get { return _group; }
    }

    public int SubGroup
    {
        get { return _subGroup; }
    }

    public GameObject Resource
    {
        get { return _resource; }
    }

    public void Destroy()
    {
        if (!_resource)
            return;

        GameObject.Destroy(_resource);
    }

    public BaseObject()
        : this(0, "")
    {

    }
    public BaseObject(int id)
        : this(id, "")
    {
    }

    public BaseObject(int id, string resourceName)
    {
        _id = id;
        _resourceName = resourceName;

        LoadResource();
    }

    void LoadResource()
    {
        if (!string.IsNullOrEmpty(_resourceName))
        {
            WorldManager.Instance.GetResourceTool().LoadAsync(_resourceName, OnResourceLoadFinished);
        }
    }

    void OnResourceLoadFinished(Object resource)
    {
        _resource = GameObject.Instantiate(resource, Vector3.zero, Quaternion.identity) as GameObject;

        Init();
    }

    public bool IsInit()
    {
        if (string.IsNullOrEmpty(_resourceName))
            return true;

        return _isInit;
    }

    public abstract void Init();
}
