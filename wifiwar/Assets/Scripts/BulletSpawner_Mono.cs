using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class BulletSpawner_Mono : MonoBehaviour
{
	public GameObject BulletTemplate;
	public Transform PlayerTransform;
	public int Speed = 5;
	public float LifeTime = 10;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SpawnECSBullet();
		}
	}

	private void SpawnECSBullet()
	{
		// Create entity prefab from the game object hierarchy once
		var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(BulletTemplate, World.Active);
		var entityManager = World.Active.EntityManager;
		var instance = entityManager.Instantiate(prefab);

		var position = transform.TransformPoint(0f, 0.0f, 0.0f);
		var rotation = CalculateBulletDirection();

		entityManager.SetComponentData(instance, new Rotation { Value = rotation });
		entityManager.SetComponentData(instance, new Translation { Value = position });
		entityManager.AddComponentData(instance, new ProjectileComponent
		{
			Speed = this.Speed,
			forward = transform.parent.forward,
			lifeTime = LifeTime

		});
		entityManager.AddComponentData(instance, new DeadlyTag());
	}

	private Quaternion CalculateBulletDirection()
	{
		Quaternion playerRotation = PlayerTransform.rotation;
		Vector3 euler = playerRotation.eulerAngles;
		Vector3 bulletRotation = new Vector3(0, euler.y + 90, 90);
		return Quaternion.Euler(bulletRotation);
	}
}
