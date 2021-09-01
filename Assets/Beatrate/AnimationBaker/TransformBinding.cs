using System;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	[Serializable]
    public class TransformBinding
	{
        public string Path
		{
            get => path;
            set => path = value;
		}
        [SerializeField]
        private string path = "";

        public TransformCurve Curve
		{
            get => curve;
            set => curve = value;
		}
        [SerializeField]
        private TransformCurve curve = null;
	}
}