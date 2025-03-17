using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Rizing.Abstract;
using Rizing.Interface;
using UnityEngine;

public class MoveablePlatform : BaseEntity, ISaveable
{

    Vector3 startpos;
    float delta;

    readonly Dictionary<Collider, Transform> colliders = new();

    protected override void Start() {
        base.Start();

        startpos = transform.position;
    }

    public override void Process(float deltaTime) {
        delta += deltaTime * 0.1f;
        delta = Mathf.Repeat(delta, Mathf.PI * 2);

        transform.position = startpos + new Vector3(Mathf.Sin(delta) * 10, 0, 0);
    }

    void OnTriggerEnter(Collider other) {
        colliders.Add(other, other.transform.parent);
        other.transform.parent = transform;
    }

    void OnTriggerExit(Collider other) {
        other.transform.parent = colliders[other];
        colliders.Remove(other);
    }

    public object SaveState()
    {
        return new SaveData
        {
            delta = delta,
            startpos = startpos
        };
    }

    public void LoadState(object inputData)
    {
        SaveData _saveData = JObject.FromObject(inputData).ToObject<SaveData>();
        
        delta = _saveData.delta;
        startpos = _saveData.startpos;
    }

    [Serializable]
    private struct SaveData
    {
        public Vector3 startpos;
        public float delta;
    }
}
