using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class BulletTemplateConvertor : MonoBehaviour, IConvertGameObjectToEntity
{
	public int Speed = 5;
	public float LifeTime = 10;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var data = new BulletTemplateComponent
		{
			Speed = Speed,
			LifeTime = LifeTime

		};
		dstManager.AddComponentData(entity, data);
	}
}
