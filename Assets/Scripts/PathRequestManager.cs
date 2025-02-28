﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	Pathfinder pathfinding;

	bool isProcessingPath;

	void Awake()
	{
		instance = this;
		pathfinding = GetComponent<Pathfinder>();
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool, Node> callback)
	{
		PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext()
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0)
		{
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success, Node curr)
	{
		currentPathRequest.callback(path, success, curr);
		isProcessingPath = false;
		TryProcessNext();
	}

	struct PathRequest
	{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool, Node> callback;

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool, Node> _callback)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}

	}
}