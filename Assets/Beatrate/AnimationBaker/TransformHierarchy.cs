using System.Collections.Generic;
using UnityEngine;

namespace Beatrate.AnimationBaker
{
	public class TransformHierarchy
	{
		private class PathPiece
		{
			public Transform Transform { get; set; }
			public string Path { get; set; }
			public PathPiece ParentPiece { get; set; }
		}

		private Dictionary<string, Transform> pathToTransform = new Dictionary<string, Transform>();

		public Transform Root { get; private set; } = null;

		public void Build(Transform root)
		{
			Root = root;
			pathToTransform.Clear();

			var rootPiece = new PathPiece
			{
				Transform = Root,
				Path = "",
				ParentPiece = null
			};
			var pieces = new List<PathPiece>();
			pieces.Add(rootPiece);

			var queue = new Queue<PathPiece>();
			queue.Enqueue(rootPiece);

			while(queue.Count != 0)
			{
				PathPiece piece = queue.Dequeue();
				for(int i = 0; i < piece.Transform.childCount; ++i)
				{
					Transform child = piece.Transform.GetChild(i);
					var childPiece = new PathPiece
					{
						Transform = child,
						Path = piece.Path.Length == 0 ? child.name : (piece.Path + "/" + child.name),
						ParentPiece = piece
					};

					pieces.Add(childPiece);
					queue.Enqueue(childPiece);
				}
			}

			for(int i = 0; i < pieces.Count; ++i)
			{
				PathPiece piece = pieces[i];
				pathToTransform[piece.Path] = piece.Transform;
			}
		}

		public bool TryGetTransform(string path, out Transform transform)
		{
			if(pathToTransform.TryGetValue(path, out transform))
			{
				return true;
			}

			transform = null;
			return false;
		}
	}
}