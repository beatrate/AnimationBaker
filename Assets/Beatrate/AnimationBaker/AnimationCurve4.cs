﻿using System;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	[Serializable]
    public class AnimationCurve4
    {
        public AnimationCurve X
        {
            get => x;
            set => x = value;
        }
        [SerializeField]
        private AnimationCurve x = new AnimationCurve();

        public AnimationCurve Y
        {
            get => y;
            set => y = value;
        }
        [SerializeField]
        private AnimationCurve y = new AnimationCurve();

        public AnimationCurve Z
        {
            get => z;
            set => z = value;
        }
        [SerializeField]
        private AnimationCurve z = new AnimationCurve();

        public AnimationCurve W
		{
            get => w;
            set => w = value;
		}
        [SerializeField]
        private AnimationCurve w = new AnimationCurve();

        public bool Empty
        {
            get
            {
                return X.keys.Length == 0 && Y.keys.Length == 0 && Z.keys.Length == 0 && W.keys.Length == 0;
            }
        }

        public AnimationCurve this[int axis]
        {
            get
            {
                switch(axis)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch(axis)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Vector4 Evaluate(float t)
        {
            float x = X.Evaluate(t);
            float y = Y.Evaluate(t);
            float z = Z.Evaluate(t);
            float w = W.Evaluate(t);

            return new Vector4(x, y, z, w);
        }

        public Quaternion EvaluateQuaternion(float t)
        {
            float x = X.Evaluate(t);
            float y = Y.Evaluate(t);
            float z = Z.Evaluate(t);
            float w = W.Evaluate(t);

            return new Quaternion(x, y, z, w);
        }
    }
}