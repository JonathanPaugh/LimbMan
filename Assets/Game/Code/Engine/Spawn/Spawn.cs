using System.Collections;
using System.Collections.Generic;
using Codice.Client.Commands;
using Jape;
using UnityEngine;


public class Spawn : Element
{
    public bool primary;

    protected override void Touch(GameObject gameObject)
    {
        if (!gameObject.HasTag(Tag.Find("Player"))) { return; }
        UpdateSpawn();
    }

    private void UpdateSpawn()
    {
        Jape.Status status = new Jape.Status
        {
            Key = "Spawn"
        };

        status.Write("Id", gameObject.Id());
        status.Write("Scene", gameObject.scene.path);

        Status.Save(status);

        Jape.Game.Save();
    }

    public Vector3 SpawnPosition()
    {
        return transform.position + (Vector3.up * 5);
    }
}
