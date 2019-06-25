using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class Spawner_Mono : MonoBehaviour
{
    public GameObject Prefab;

    void Start()
    {
        // Create entity prefab from the game object hierarchy once
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, World.Active);
        var entityManager = World.Active.EntityManager;

        var instance = entityManager.Instantiate(prefab);

        var position = transform.TransformPoint(0.0f, 0.0f,0.0f);
        entityManager.SetComponentData(instance, new Translation { Value = position });
    }
}
