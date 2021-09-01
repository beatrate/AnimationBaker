using Beatrate.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	public class SnappyAnimator : MonoBehaviour
	{
		private class BoundBakedAnimationClip
		{
			public BakedAnimationClip Clip { get; set; }
			public List<Transform> ResolvedTransforms { get; set; }
		}

		private class BoundBakedAnimationPose
		{
			public BakedAnimationPose Pose { get; set; }
			public List<Transform> ResolvedTransforms { get; set; }
		}

		private class TransformCache
		{
			public List<Transform> ResolvedTransforms { get; set; }
		}

		private struct TransformInterpolation
		{
			public Transform Transform;
			public Vector3 StartPosition;
			public Vector3 EndPosition;
			public Quaternion StartRotation;
			public Quaternion EndRotation;
			public Vector3 StartScale;
			public Vector3 EndScale;
		}

		[SerializeField]
		private BakedAnimationPose start = null;
		[SerializeField]
		private BakedAnimationPose end = null;

		[SerializeField]
		private float t = 0.0f;
		[SerializeField]
		private bool updateT = false;
		[SerializeField]
		[Min(0.0f)]
		private float cycleDuration = 1.0f;

		private Dictionary<int, TransformCache> transformCaches = new Dictionary<int, TransformCache>();
		private TransformHierarchy transformHierarchy = new TransformHierarchy();

		public void Awake()
		{
			transformHierarchy.Build(transform);

			Bind(start);
			Bind(end);
		}

		public void Update()
		{
			t = Mathf.Clamp(t, 0.0f, cycleDuration);

			BakedAnimationPose source = null;
			BakedAnimationPose target = null;

			if(t / cycleDuration < 0.5f)
			{
				source = start;
				target = end;
			}
			else
			{
				source = end;
				target = start;
			}
			Blend(source, target, t / cycleDuration);

			if(updateT)
			{
				t = Mathf.Clamp(t, 0.0f, cycleDuration);

				t += Time.deltaTime;
				if(t > cycleDuration)
				{
					t -= cycleDuration;
					t = Mathf.Clamp(t, 0.0f, cycleDuration);
				}
			}
		}

		private void Blend(BakedAnimationPose first, BakedAnimationPose second, float t)
		{
			TransformCache firstCache = GetTransformCache(first);
			TransformCache secondCache = GetTransformCache(second);

			var interpolationMap = BeDictionaryPool<int, TransformInterpolation>.Get();

			for(int i = 0; i < first.BaseClip.TransformBindings.Count; ++i)
			{
				var binding = first.BaseClip.TransformBindings[i];
				var resolvedTransform = firstCache.ResolvedTransforms[i];
				if(resolvedTransform != null)
				{
					if(!interpolationMap.TryGetValue(resolvedTransform.GetInstanceID(), out TransformInterpolation interpolation))
					{
						interpolation = new TransformInterpolation
						{
							Transform = resolvedTransform,
							StartPosition = Vector3.zero,
							EndPosition = Vector3.zero,
							StartRotation = Quaternion.identity,
							EndRotation = Quaternion.identity,
							StartScale = Vector3.one,
							EndScale = Vector3.one,
						};
						
						interpolationMap.Add(resolvedTransform.GetInstanceID(), interpolation);
					}

					if(binding.Curve.TryEvaluatePosition(first.GetTime(), out Vector3 position))
					{
						interpolation.StartPosition = position;
					}

					if(binding.Curve.TryEvaluateRotation(first.GetTime(), out Quaternion rotation))
					{
						interpolation.StartRotation = rotation;
					}

					if(binding.Curve.TryEvaluateScale(first.GetTime(), out Vector3 scale))
					{
						interpolation.StartScale = scale;
					}

					interpolationMap[resolvedTransform.GetInstanceID()] = interpolation;
				}
			}

			for(int i = 0; i < second.BaseClip.TransformBindings.Count; ++i)
			{
				var binding = second.BaseClip.TransformBindings[i];
				var resolvedTransform = secondCache.ResolvedTransforms[i];
				if(resolvedTransform != null)
				{
					if(!interpolationMap.TryGetValue(resolvedTransform.GetInstanceID(), out TransformInterpolation interpolation))
					{
						interpolation = new TransformInterpolation
						{
							Transform = resolvedTransform,
							StartPosition = Vector3.zero,
							EndPosition = Vector3.zero,
							StartRotation = Quaternion.identity,
							EndRotation = Quaternion.identity,
							StartScale = Vector3.one,
							EndScale = Vector3.one,
						};

						interpolationMap.Add(resolvedTransform.GetInstanceID(), interpolation);
					}

					if(binding.Curve.TryEvaluatePosition(second.GetTime(), out Vector3 position))
					{
						interpolation.EndPosition = position;
					}

					if(binding.Curve.TryEvaluateRotation(second.GetTime(), out Quaternion rotation))
					{
						interpolation.EndRotation = rotation;
					}

					if(binding.Curve.TryEvaluateScale(second.GetTime(), out Vector3 scale))
					{
						interpolation.EndScale = scale;
					}

					interpolationMap[resolvedTransform.GetInstanceID()] = interpolation;
				}
			}

			var interpolations = BeListPool<TransformInterpolation>.Get();
			interpolations.AddRange(interpolationMap.Values);
			BeDictionaryPool<int, TransformInterpolation>.Return(interpolationMap);

			for(int i = 0; i < interpolations.Count; ++i)
			{
				TransformInterpolation interpolation = interpolations[i];
				interpolation.Transform.localPosition = Vector3.Lerp(interpolation.StartPosition, interpolation.EndPosition, t);
				interpolation.Transform.localRotation = Quaternion.Slerp(interpolation.StartRotation, interpolation.EndRotation, t);
			}

			BeListPool<TransformInterpolation>.Return(interpolations);
		}

		private void Bind(BakedAnimationPose pose)
		{
			var cache = new TransformCache
			{
				ResolvedTransforms = new List<Transform>()
			};

			for(int i = 0; i < pose.BaseClip.TransformBindings.Count; ++i)
			{
				TransformBinding binding = pose.BaseClip.TransformBindings[i];
				transformHierarchy.TryGetTransform(binding.Path, out Transform resolvedTransform);
				cache.ResolvedTransforms.Add(resolvedTransform);
			}

			transformCaches.Add(pose.GetInstanceID(), cache);
		}

		private TransformCache GetTransformCache(BakedAnimationPose pose)
		{
			return transformCaches[pose.GetInstanceID()];
		}
	}
}