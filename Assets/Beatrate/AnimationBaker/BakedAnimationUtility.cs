using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	public static class BakedAnimationUtility
	{
		private static readonly char[] PathSeparators = new[] { '/' };

		public static string CalculateTransformPath(Transform root, Transform targetTransform)
		{
			var path = new List<string>();

			Transform currentTransform = targetTransform;

			while(currentTransform != root && currentTransform != null)
			{
				path.Add(currentTransform.name);
				currentTransform = currentTransform.parent;
			}

			var builder = new StringBuilder();
			for(int i = path.Count - 1; i >= 0; --i)
			{
				builder.Append(path[i]);
				if(i != 0)
				{
					builder.Append('/');
				}
			}

			return builder.ToString();
		}

		public static string[] ParseTransformPath(string path)
		{
			return path.Split(PathSeparators, System.StringSplitOptions.None);
		}
	}
}