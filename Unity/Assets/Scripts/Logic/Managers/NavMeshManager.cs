using System.Collections.Generic;
using Lockstep.PathFinding;
using Lockstep.Math;
using UnityEngine;
using UnityEngine.Profiling;

public class NavMeshManager {
    static TrianglePointPath path = new TrianglePointPath();
    static TriangleNavMesh NavMesh;
    public TextAsset navData;
    static int _mapId = -1;

    public static void DoInit(string data){
        NavMesh = new TriangleNavMesh(data);
    }

    public static List<LVector3> allFindReq = new List<LVector3>();

    public static List<LVector3> FindPath(LVector3 fromPoint, LVector3 toPoint){
        //allFindReq.Add(fromPoint);
        //allFindReq.Add(toPoint);
        Profiler.BeginSample("FindPath");
        var _ret = NavMesh.FindPath(fromPoint, toPoint, path);
        Profiler.EndSample();
        return _ret;
    }

    static BspTree _bspTree => NavMesh.bspTree;
}