using System.Collections;
using System.Collections.Generic;

namespace PlutoECL
{
	/// <summary> Filter for global systems. To handle Entities like in ECS. Actual filter version is simple, but it does its work.
	/// <para>Note that Filter doesn't know if any new entities with filtering components spawned. You need to Make new filter if you want to handle new entities (for example in Run cycle). It is not very "usable" and optimized, still work on it.</para></summary>
	public sealed class Filter : IEnumerable
	{
		readonly List<Entity> entities = new List<Entity>();
		
		public Entity this[int index] => entities[index];
		public int Count => entities.Count;
		
		public static Filter Make<T>() where T : Part
		{
			var filter = new Filter();
		
			filter.entities.AddRange(Entity.FindAllWith<T>());
			
			return filter;
		}
		
		public static Filter Make<T, T2>() where T : Part where T2 : Part 
			=> Make<T>().ExcludeNot<T2>();

		public static Filter Make<T, T2, T3>() where T : Part where T2 : Part where T3 : Part 
			=> Make<T>().ExcludeNot<T2>().ExcludeNot<T3>();

		/// <summary> Returns new Filter with excluded all entities with T component.</summary>
		public Filter Exclude<T>() where T : Part
		{
			var filter = new Filter();

			for (int i = 0; i < entities.Count; i++)
				if (!entities[i].Have<T>())
					filter.entities.Add(entities[i]);

			return filter;
		}
		
		/// <summary> Returns new Filter with excluded all entities with No T component.</summary>
		Filter ExcludeNot<T>() where T : Part
		{
			var filter = new Filter();

			for (int i = 0; i < entities.Count; i++)
				if (entities[i].Have<T>())
					filter.entities.Add(entities[i]);

			return filter;
		}

		public IEnumerator GetEnumerator() => entities.GetEnumerator();
	}
}