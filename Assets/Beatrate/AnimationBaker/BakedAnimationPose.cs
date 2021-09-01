using UnityEngine;

namespace Beatrate.AnimationBaker
{
	[CreateAssetMenu]
	public class BakedAnimationPose : ScriptableObject
	{
		public BakedAnimationClip BaseClip => baseClip;
		[SerializeField]
		private BakedAnimationClip baseClip = null;

		public float Frame => frame;
		[SerializeField]
		[Min(0.0f)]
		private float frame = 0.0f;

		public float GetTime()
		{
			if(baseClip == null)
			{
				return 0.0f;
			}

			return (1.0f / baseClip.SourceClip.frameRate) * frame;
		}
	}
}