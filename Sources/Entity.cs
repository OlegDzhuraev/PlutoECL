using System.Collections.Generic;
using UnityEngine;

namespace PlutoECL
{
    /// <summary> Container for all components and logics of your object. Also it is MonoBehaviour, so you can use it as the connection to the Unity API.</summary>
    [DisallowMultipleComponent]
    public sealed class Entity : MonoBehaviour
    {
        static readonly List<Entity> entities = new List<Entity>();

        public Entity(Tags tags, Events events)
        {
            this.tags = tags;
            this.events = events;
        }

        public Tags Tags => tags ?? (tags = new Tags());
        public Events Events => events ?? (events = new Events());

        Tags tags;
        Events events;

        void Awake() => entities.Add(this);

        public void Destroy()
        {
            entities.Remove(this);
            
            Destroy(gameObject);
        }
        
        /// <summary> Get (with adding if dont exist) any of game Components (Parts). If you want add Logic - do it from inspector before game run. If logic should not work from start - make Component with bool flag. Don't add Logics in runtime</summary>
        public T Get<T>() where T : Part
        {
            var component = GetComponent<T>();

            if (!component)
                component = gameObject.AddComponent<T>();

            return component;
        }

        public T Get<T>(T trickedType) where T : Part => Get<T>();

        /// <summary> Checks, is there a specified Part on Entity. Argument is MonoBehaviour, so it can be used only for Components and Logics - doesn't affects default Unity Components, only for game logic.</summary>
        public bool Have<T>() where T : Part => GetComponent<T>();
        
        public bool Have<T>(T trickedType) where T : Part => Have<T>();
        
        /// <summary> Removes a specified Part from Entity. It can be component or Logic. Doesn't Affect default Unity Components - Only for Game Logic.</summary>
        public void Delete<T>() where T : Part
        {
            var component = GetComponent<T>();
            
            if (component)
                Destroy(component);
        }
        
        public void Delete<T>(T trickedType) where T : Part => Delete<T>();
        
        /// <summary> Finds specified Component (Part) on level. If there several Parts of this type on level, you receive only one of them - which was created first. </summary>
        public static T FindPart<T>() where T : Part
        {
            for (var i = 0; i < entities.Count; i++)
                if (entities[i].Have<T>())
                    return entities[i].Get<T>();

            return null;
        }       
        
        /// <summary> Finds all of specified Component (Part) on level. </summary>
        public static List<T> FindAllParts<T>() where T : Part
        {
            var list = new List<T>();
            
            for (var i = 0; i < entities.Count; i++)
                if (entities[i].Have<T>())
                    list.Add(entities[i].Get<T>());

            return list;
        }
        
        /// <summary> Returns Entity with specified Tag. Like Unity Find method. </summary>
        public static Entity FindWith(IntTag tag)
        {
            for (var i = 0; i < entities.Count; i++)
                if (entities[i].Tags.Have(tag))
                    return entities[i];

            return null;
        }
        
        /// <summary> Returns all Entities with specified Tag. Like Unity Find method. </summary>
        public static List<Entity> FindAllWith(IntTag tag)
        {
            var resultList = new List<Entity>();
            
            for (var i = 0; i < entities.Count; i++)
                if (entities[i].Tags.Have(tag))
                    resultList.Add(entities[i]);
            
            return resultList;
        }
        
        /// <summary> Returns Entity with specified Component. Like FindObjectOfType, but faster. </summary>
        public static Entity FindWith<T>() where T : Part
        {
            for (var i = 0; i < entities.Count; i++)
                if (entities[i].GetComponent<T>())
                    return entities[i];

            return null;
        }
        
        /// <summary> Returns all Entities with specific component. Something like FindObjectsOfType, but faster and works with framework components.</summary>
        public static List<Entity> FindAllWith<T>() where T : Part
        {
            var resultList = new List<Entity>();
            
            for (var i = 0; i < entities.Count; i++)
                if (entities[i].GetComponent<T>())
                    resultList.Add(entities[i]);
            
            return resultList;
        }
        
        /// <summary> Returns filter of all Entities with specific component. Useful for global systems</summary>
        public static Filter Filter<T>() where T : Part => PlutoECL.Filter.Make<T>();
        
        /// <summary> Returns filter of all Entities with specific component. Useful for global systems</summary>
        public static Filter Filter<T, T2>() where T : Part where T2 : Part 
            => PlutoECL.Filter.Make<T, T2>();
        
        /// <summary> Returns filter of all Entities with specific component. Useful for global systems</summary>
        public static Filter Filter<T, T2, T3>() where T : Part where T2 : Part where T3 : Part 
            => PlutoECL.Filter.Make<T, T2, T3>();

        /// <summary> Spawns prefab as Entity. </summary>
        public static Entity Spawn(GameObject withPrefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var entObject = Instantiate(withPrefab);

            return BuildEntity(entObject, position, rotation, parent);
        }
        
        /// <summary> Spawns empty entity with specified name. </summary>
        public static Entity Spawn(string name = "Entity", Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var entObject = new GameObject(name);
            
            return BuildEntity(entObject, position, rotation, parent);
        }

        static Entity BuildEntity(GameObject entObject, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            var entity = entObject.GetComponent<Entity>();
            
            if (!entity)
                entity = entObject.AddComponent<Entity>();
            
            entObject.transform.position = position;
            entObject.transform.rotation = rotation;
            
            if (parent)
                entObject.transform.SetParent(parent);
            
            return entity;
        }
    }
}