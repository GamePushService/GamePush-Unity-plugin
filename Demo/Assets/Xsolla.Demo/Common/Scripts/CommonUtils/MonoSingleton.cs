using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private const string PATH_TO_PREFABS = "";
		private static T _instance;
		private bool _isInitialized;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = InstantiateThis();
					_instance?.OneTimeInit();
				}

				return _instance;
			}
		}

		public static bool IsExist => _instance != null;

		private void Awake()
		{
			if (_instance == null)
			{
				DontDestroyOnLoad(gameObject);
				_instance = this as T;
				_instance.OneTimeInit();
			}
			else if (_instance != this)
			{
				Destroy(gameObject);
			}
		}

		private static T InstantiateThis()
		{
			var componentName = typeof(T).Name;

			//Try finding pre-existing object on the scene
#if UNITY_6000
			T result = FindAnyObjectByType(typeof(T)) as T;
#else
			T result = FindObjectOfType(typeof(T)) as T;
#endif

			if (result != null)
				return result;

			//Try loading from resources
			var prefab = Resources.Load($"{PATH_TO_PREFABS}{typeof(T).Name}");
			if (prefab != null)
			{
				var instance = Instantiate(prefab) as GameObject;
				if (instance != null)
					instance.name = typeof(T).Name;
				result = instance?.GetComponent<T>();
			}

			if (result != null)
				return result;

			//Create new object
			var newInstance = new GameObject($"Temp Instance of {typeof(T)}");
			result = newInstance.AddComponent<T>();
			if (result != null)
				return result;

			XDebug.LogError($"Could not instantiate {componentName}");
			return result;
		}

		public virtual void Init() { }

		protected virtual void OnDestroy()
		{
			_instance = null;
		}

		private void OneTimeInit()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				Init();
			}
		}

		private void OnApplicationQuit()
		{
			Destroy(gameObject);
		}
	}
}