using System;
using System.Collections;
using System.Collections.Generic;
using Jape;
using UnityEngine;


public class ToolWallBlock : Element
{
    [Space(10)]
    [SerializeField] private MeshRenderer Mesh = default;

    [HideInInspector] public List<GameObject> Targets = new List<GameObject>();

    private void Start()
    {
        Mesh.enabled = false;
    }

    protected override void Touch(GameObject gameObject)
    {
        if (!gameObject.HasTag(Tag.Find("Player"))) { return; }

        if (!Targets.Contains(gameObject)) {
            if (gameObject.TryGetComponent(out Movement movement))
            {
                movement.Grounded.Restrict(GetType());
                movement.Walled.Restrict(GetType());
                Targets.Add(gameObject);
            }
        }
    }

    protected override void Leave(GameObject gameObject)
    {
        if (!gameObject.HasTag(Tag.Find("Player"))) { return; }

        if (Targets.Contains(gameObject)) {
            if (gameObject.TryGetComponent(out Movement movement))
            {
                movement.Grounded.Unrestrict(GetType());
                movement.Walled.Unrestrict(GetType());
                Targets.Remove(gameObject);
            }
        }
    }
}
