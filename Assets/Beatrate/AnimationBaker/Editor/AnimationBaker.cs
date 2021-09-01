using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Beatrate.AnimationBaker.EditorSide
{
	public class EditorProgressBarScope : IDisposable
	{
		private string title;
		private string info;
		private float progress;

		public EditorProgressBarScope(string title, string info)
		{
			this.title = title;
			this.info = info;
			progress = 0.0f;
			SyncProgressBar();
		}

		public void UpdateProgress(string info, float progress)
		{
			this.info = info;
			this.progress = progress;
			SyncProgressBar();
		}

		public void UpdateProgress(string info)
		{
			this.info = info;
			SyncProgressBar();
		}

		public void UpdateProgress(float progress)
		{
			this.progress = progress;
			SyncProgressBar();
		}

		private void SyncProgressBar()
		{
			EditorUtility.DisplayProgressBar(title, info, progress);
		}

		public void Dispose()
		{
			EditorUtility.ClearProgressBar();
		}
	}

	public class AnimationBaker
    {
		private class BindingIdentifiers
		{
			public const string LocalPosition = "m_LocalPosition";
			public const string LocalRotation = "m_LocalRotation";
			public const string LocalScale = "m_LocalScale";
		}

        public static void Bake(BakedAnimationClip bakedClip, SerializedBakedAnimationClip serializedClip)
		{
			using(var progressBar = new EditorProgressBarScope("Baking animation clips", ""))
			{
				EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(bakedClip.SourceClip);
				var transformBindings = new Dictionary<string, TransformBinding>();

				for(int i = 0; i < curveBindings.Length; ++i)
				{
					EditorCurveBinding curveBinding = curveBindings[i];
					
					AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(bakedClip.SourceClip, curveBinding);
					string path = curveBinding.path;
					string propertyName = curveBinding.propertyName;
					
					if(curveBinding.type == typeof(UnityEngine.Transform))
					{
						if(!transformBindings.TryGetValue(curveBinding.path, out TransformBinding transformBinding))
						{
							transformBinding = new TransformBinding
							{
								Curve = new TransformCurve(),
								Path = curveBinding.path
							};
							transformBindings.Add(curveBinding.path, transformBinding);
						}

						if(propertyName.StartsWith(BindingIdentifiers.LocalPosition))
						{
							int axis = ExtractAxisFromProperty(propertyName);
							transformBinding.Curve.PositionCurve[axis] = animationCurve.DeepCopy();
						}
						else if(propertyName.StartsWith(BindingIdentifiers.LocalRotation))
						{
							int axis = ExtractAxisFromProperty(propertyName);
							transformBinding.Curve.RotationCurve[axis] = animationCurve.DeepCopy();
						}
						else if(propertyName.StartsWith(BindingIdentifiers.LocalScale))
						{
							int axis = ExtractAxisFromProperty(propertyName);
							transformBinding.Curve.ScaleCurve[axis] = animationCurve.DeepCopy();
						}
					}

					progressBar.UpdateProgress(i / (curveBindings.Length - 1));
				}

				bakedClip.TransformBindings.Clear();
				bakedClip.TransformBindings.AddRange(transformBindings.Values);

				EditorUtility.SetDirty(bakedClip);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		private static int ExtractAxisFromProperty(string propertyName)
		{
			switch(propertyName[propertyName.Length - 1])
			{
				case 'x':
					return 0;
				case 'y':
					return 1;
				case 'z':
					return 2;
				case 'w':
					return 3;
				default:
					throw new InvalidOperationException();
			}
		}
    }
}
