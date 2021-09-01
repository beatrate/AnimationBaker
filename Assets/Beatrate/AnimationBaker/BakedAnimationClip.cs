using System.Collections.Generic;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	[CreateAssetMenu]
	public class BakedAnimationClip : ScriptableObject
	{
		public AnimationClip SourceClip => sourceClip;
		[SerializeField]
		private AnimationClip sourceClip = null;

		public float Length => sourceClip == null ? 0.0f : sourceClip.length;

		public List<TransformBinding> TransformBindings => transformBindings;
		[SerializeField]
		private List<TransformBinding> transformBindings = null;
	}
}