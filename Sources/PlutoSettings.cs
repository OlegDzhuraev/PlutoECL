using System;
using UnityEngine;

namespace PlutoECL
{
    [CreateAssetMenu(fileName = "PlutoSettings", menuName = "Pluto ECL/Settings Asset")]
    public class PlutoSettings : ScriptableObject
    {
	    public static PlutoSettings Instance
        {
	        get
	        {
		        if (!instance)
			        instance = Resources.Load("PlutoSettings") as PlutoSettings;

		        return instance;
	        }
        }
        
        static PlutoSettings instance;
        
        public Type TagsEnum;

        [Tooltip("Adds Quck Entity add button and shows null and empty fields.")]
        public bool UseCustomInspectorHeader = true;
        
        [Tooltip("Highlights in hierarchy objects with Components which have null and empty fields.")]
        public bool UseCustomHierarchy = true;
    }
}