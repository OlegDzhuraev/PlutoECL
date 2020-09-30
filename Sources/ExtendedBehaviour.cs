using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlutoECL
{
    /// <summary> Derive from this class all of your game logic scripts.</summary>
    [RequireComponent(typeof(Entity))]
    public class ExtendedBehaviour : MonoBehaviour
    {
        static readonly List<ExtendedBehaviour> behaviours = new List<ExtendedBehaviour>();
        static readonly List<ExtendedBehaviour> newBehaviours = new List<ExtendedBehaviour>();

        public Entity Entity
        {
            get
            {
                if (!entity)
                    entity = GetComponent<Entity>();

                return entity;
            }
        }
        
        Entity entity;

        [Tooltip("Execution priority of this behaviour Run cycle. Smaller values makes it executing early than others.")]
        [SerializeField, Range(-3, 3)] int executionOrder;

        void Awake()
        {
            newBehaviours.Add(this);

            Init();
        }

        void Start() => PostInit();

        protected virtual void Init() { }
        protected virtual void PostInit() { }
        protected virtual void Run() { }
                
        void AddToRunQueue()
        {
            for (var i = 0; i < behaviours.Count; i++)
                if (executionOrder <= behaviours[i].executionOrder)
                {
                    behaviours.Insert(i, this);
                    return;
                }

            behaviours.Add(this);
        }
        
        /// <summary> Run game update cycle. It should be done from one place in code. </summary>
        public static void RunAll()
        {
            for (var i = 0; i < newBehaviours.Count; i++)
                newBehaviours[i].AddToRunQueue();
            
            newBehaviours.Clear();

            for (var i = behaviours.Count - 1; i >= 0; i--)
                if (behaviours[i].enabled)
                    behaviours[i].Run();
        }

        void OnDestroy() => behaviours.Remove(this);
        
        /// <summary> Get (with adding if dont exist) any of game Components (Parts). If you want add Logic - do it from inspector before game run. If logic should not work from start - make Component with bool flag. Don't add Logics in runtime</summary>
        public T Get<T>() where T : Part => Entity.Get<T>();

        /// <summary> Get (with adding if dont exist) any of game Components (Parts). If you want add Logic - do it from inspector before game run. If logic should not work from start - make Component with bool flag. Don't add Logics in runtime</summary>
        public Part Get(Type partType) => Entity.Get(partType);
        
        /// <summary> Returns required game Component (Part) only if it exist on Entity. </summary>
        public T GetIfExist<T>() where T : Part => Entity.GetIfExist<T>();
        
        /// <summary> Checks, is there a specified Part on Entity. Argument is MonoBehaviour, so it can be used only for Components and Logics - doesn't affects default Unity Components, only for game logic.</summary>
        public bool Have<T>() where T : Part => Entity.Have<T>();

        /// <summary> Removes a specified Part from Entity. It can be component or Logic. Doesn't Affect default Unity Components - Only for Game Logic.</summary>
        public void Delete<T>() where T : Part => Entity.Delete<T>();

        /// <summary> Returns Entity with specified Component. Like FindObjectOfType, but faster. </summary>
        public Entity FindWith<T>() where T : Part => Entity.FindWith<T>();
        
        /// <summary> Returns all Entities with specific component. Something like FindObjectsOfType, but faster and works with framework components.</summary>
        public List<Entity> FindAllWith<T>() where T : Part => Entity.FindAllWith<T>();

        /// <summary> Returns Entity with specified Tag. Like Unity Find method. </summary>
        public Entity FindWith(IntTag tag) => Entity.FindWith(tag);
        
        /// <summary> Returns all Entities with specified Tag. Like Unity Find method. </summary>
        public List<Entity> FindAllWith(IntTag tag) => Entity.FindAllWith(tag);

        /// <summary> Finds specified Component (Part) on level. If there several Parts of this type on level, you receive only one of them - which was created first. </summary>
        public T FindPart<T>() where T : Part => Entity.FindPart<T>();
        
        /// <summary> Finds all of specified Component (Part) on level. </summary>
        public List<T> FindAllParts<T>() where T : Part => Entity.FindAllParts<T>();
    }
}