using Unity.Entities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	static GameManager instance = null;

	[SerializeField] GameObject explosionPrefab = null;

	EntityManager entityManager;
	World world = null;
	BlobAssetStore blobAssetStore = null;

	Entity explosionEntityPrefab = Entity.Null;

	public static Entity ExplosionPrefab => instance.explosionEntityPrefab;

	private void Awake()
	{
		if (instance != null)
			Destroy(gameObject);
		instance = this;

		DontDestroyOnLoad(gameObject);

		world = World.DefaultGameObjectInjectionWorld;
		entityManager = world.EntityManager;
		blobAssetStore = new BlobAssetStore();

		var settings = GameObjectConversionSettings.FromWorld(world, blobAssetStore);

		explosionEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(explosionPrefab, settings);
		entityManager.SetName(explosionEntityPrefab, "Explosion prefab");
	}

	private void OnDestroy()
	{
		if (blobAssetStore != null) blobAssetStore.Dispose();
	}
}
