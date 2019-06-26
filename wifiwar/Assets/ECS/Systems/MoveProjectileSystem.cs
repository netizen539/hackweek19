using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class MoveProjectileSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		/*
		Entities.ForEach((ref ProjectileComponent projectile, ref Translation translation, ref Rotation rotation) =>
		{
			var deltaTime = Time.deltaTime;
			Vector3 newTranslation = new Vector3(translation.Value.x, translation.Value.y, translation.Value.z);
			newTranslation += projectile.forward.normalized * projectile.Speed * Time.deltaTime;

			translation = new Translation { Value = newTranslation };
		});
		*/
	}
}
