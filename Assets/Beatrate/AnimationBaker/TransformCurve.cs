using System;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	[Serializable]
    public class TransformCurve
	{
        public AnimationCurve3 PositionCurve
		{
            get => positionCurve;
            set => positionCurve = value;
		}
        [SerializeField]
        private AnimationCurve3 positionCurve = new AnimationCurve3();

        public AnimationCurve4 RotationCurve
		{
            get => rotationCurve;
            set => rotationCurve = value;
		}
        [SerializeField]
        private AnimationCurve4 rotationCurve = new AnimationCurve4();

        public AnimationCurve3 ScaleCurve
		{
            get => scaleCurve;
            set => scaleCurve = value;
		}
        [SerializeField]
        private AnimationCurve3 scaleCurve = new AnimationCurve3();

        public bool TryEvaluatePosition(float t, out Vector3 position)
		{
            if(PositionCurve.Empty)
			{
                position = Vector3.zero;
                return false;
			}

            position = PositionCurve.Evaluate(t);
            return true;
		}

        public bool TryEvaluateRotation(float t, out Quaternion rotation)
		{
            if(RotationCurve.Empty)
			{
                rotation = Quaternion.identity;
                return false;
			}

            rotation = RotationCurve.EvaluateQuaternion(t);
            return true;
		}

        public bool TryEvaluateScale(float t, out Vector3 scale)
		{
            if(PositionCurve.Empty)
            {
                scale = Vector3.one;
                return false;
            }

            scale = ScaleCurve.Evaluate(t);
            return true;
        }
    }
}