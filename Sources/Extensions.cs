using UnityEngine;

namespace PlutoECL
{
	public static class Extensions
	{
		/// <summary> Short version of gameObject.GetComponent Entity (); for MonoBehaviour connections to Pluto ECL. Can return null if object is not Entity.</summary>
		public static Entity GetEntity(this GameObject gameObject) => gameObject.GetComponent<Entity>();

		/// <summary> Returns Component of specified type. Not affects Unity default components because requires T derived from MonoBehaviour - only for game logics. </summary>
		public static T Get<T>(this GameObject gameObject) where T : Part => gameObject.GetComponent<T>();
		        
		/// <summary> Returns required game Component (Part) only if it exist on Entity. </summary>
		public static T GetIfExist<T>(this GameObject gameObject) where T : Part => gameObject.GetComponent<T>();
		
		/// <summary> Returns true if game object have Component of specified type. Not affects Unity default components because requires T derived from Part - only for game logics. </summary>
		public static bool Have<T>(this GameObject gameObject) where T : Part => gameObject.Get<T>();
		
		/// <summary> Short version of gameObject.GetComponent Entity (); for MonoBehaviour connections to Pluto ECL. Can return null if object is not Entity.</summary>
		public static Entity GetEntity(this MonoBehaviour monoBeh) => monoBeh.GetComponent<Entity>();

		/// <summary> Returns Component of specified type. Not affects Unity default components because requires T derived from MonoBehaviour - only for game logics. </summary>
		public static T Get<T>(this MonoBehaviour monoBeh) where T : Part => monoBeh.GetComponent<T>();
		
		/// <summary> Returns true if game object have Component of specified type. Not affects Unity default components because requires T derived from Part - only for game logics. </summary>
		public static bool Have<T>(this MonoBehaviour monoBeh) where T : Part => monoBeh.Get<T>();
	}
}