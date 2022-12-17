using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Utilities
{
	public static class FilePathUtility
	{
		/// <summary>
		/// Retrieve l'absolute path for the parameter <c>path</c>.
		/// If <c>path</c> starts with ".", the full path is calculated as subfolder of the running assembly.
		/// If <c>path</c> is a full path, return the <c>path</c> itself.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetAbsolutePath(string path)
		{
			if (!Path.IsPathFullyQualified(path))
			{ 
				var runAssembly=Assembly.GetEntryAssembly();
				var pathAssembly = Path.GetDirectoryName(runAssembly.Location);
				return Path.Combine(pathAssembly, path);
			}
			return path;
		}
	}
}
