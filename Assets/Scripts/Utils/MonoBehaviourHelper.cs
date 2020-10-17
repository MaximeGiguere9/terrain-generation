using System.Collections;
using UnityEngine;

namespace Utils
{
	/// <inheritdoc />
	/// <summary>
	/// Serves as a bridge between Unity game loop and classes that don't inherit from MonoBehaviour
	/// </summary>
	public class MonoBehaviourHelper : MonoBehaviour
	{

		private static MonoBehaviourHelper _instance;
		public static MonoBehaviourHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameObject("MonoBehaviourHelper").AddComponent<MonoBehaviourHelper>();
					DontDestroyOnLoad(_instance);
				}
				return _instance;
			}
		}

		public static Coroutine Start(IEnumerator routine) => Instance.StartCoroutine(routine);

		public static void Stop(IEnumerator routine) => Instance.StopCoroutine(routine);

		public static void Stop(Coroutine routine) => Instance.StopCoroutine(routine);
	}
}