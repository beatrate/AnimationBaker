using System;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	public static class AnimationCurveExtensions
	{
        public static AnimationCurve DeepCopy(this AnimationCurve original)
		{
            AnimationCurve copied = new AnimationCurve();
            copied.preWrapMode = original.preWrapMode;
            copied.postWrapMode = original.postWrapMode;

            var keys = new Keyframe[original.keys.Length];
            Array.Copy(original.keys, keys, original.keys.Length);
            copied.keys = keys;

            return copied;
		}
	}
}