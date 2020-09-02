using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PlutoECL
{
	public static class Updater
	{
		[MenuItem("Tools/Pluto ECL/Update")]
		static void Init() => RunUpdate();

		static void RunUpdate()
		{
			var packagesLockPath = Path.GetDirectoryName(Application.dataPath) + @"\Packages\packages-lock.json";
			
			if (!File.Exists(packagesLockPath)) 
				return;

			var text = String.Empty;
			using (var sr = new StreamReader(packagesLockPath, System.Text.Encoding.Default))
				text = sr.ReadToEnd();

			var lines = text.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
			
			var lockFileLength = lines.Count;
			var plutoBlockStartIndex = -1;
			var plutoBlockEndIndex = -1;

			for (int i = 0; i < lockFileLength; i++)
			{
				var line = lines[i];

				if (line.Contains("\"com.insaneone.plutoecl\": {"))
					plutoBlockStartIndex = i;

				if (plutoBlockStartIndex != -1 && line.Contains("},"))
					plutoBlockEndIndex = i;
				
				if (plutoBlockStartIndex != -1 && i == plutoBlockEndIndex + 1)
					break;
			}
			
			lines.RemoveRange(plutoBlockStartIndex, plutoBlockEndIndex + 1);
      
			lockFileLength = lines.Count;
			using (var sr = new StreamWriter(packagesLockPath))
				for (var i = 0; i < lockFileLength; i++)
					sr.WriteLine(lines[i]);

			Debug.Log("Pluto updated successfully.");

			AssetDatabase.Refresh(ImportAssetOptions.Default);
		}
	}
}