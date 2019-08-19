using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Lockstep.Math;
using Lockstep.PathFinding;
using UnityEngine.Profiling;


public class TestNavMesh : MonoBehaviour {
    public Transform srcPoint;
    public Transform dstPoint;

    public LineRenderer lineRenderer;

    public List<LVector3> pathPoints = new List<LVector3>();
    public bool isFindPath = false;

    public TriangleNavMesh NavMesh;


    public bool isShowSingleTriangleConnection;
    public bool isShowTriangleArea;
    public bool isShowAllTriangles;
    public bool isShowPathResult;
    public bool isShowPathProgress;

    public TextAsset navData;
    void CheckInit(){
        if (NavMesh != null) {
            return;
        }

        //var _mapId = int.Parse(SceneManager.GetActiveScene().name.Replace("map", ""));
        //var txt = Resources.Load<TextAsset>("Maps/" + _mapId + ".navmesh");
        var txt = navData;
        NavMesh = new TriangleNavMesh(txt.text);
        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();
        resultMesh = CreateMeshGo("PathMesh", 1.0f);
        progressMesh = CreateMeshGo("ProgressMesh", 0.5f);
        allMesh = CreateMeshGo("AllMesh", 0.2f);
    }

    private MeshFilter CreateMeshGo(string name, float high){
        var mesh = new Mesh();
        mesh.MarkDynamic();
        var go = new GameObject(name);
        go.AddComponent<MeshRenderer>().material = debugMeshMat;
        go.transform.position = Vector3.up * high;
        return go.AddComponent<MeshFilter>();
    }

    private void Update(){
        CheckInit();
        DrawLine();
    }

    public TrianglePointPath path = new TrianglePointPath();
    public float widthMultiplier = 2;
    private MeshFilter resultMesh;
    private MeshFilter progressMesh;
    private MeshFilter allMesh;
    public Material debugMeshMat;
    public float useTime;

    void DrawLine(){
        if (isFindPath) {
            Profiler.BeginSample(" FindPath");
            var time = DateTime.Now;
            //pathPoints = NavMesh.FindPath(srcPoint.position.ToLVector3(), dstPoint.position.ToLVector3(), path);
            pathPoints = NavMeshManager.FindPath(srcPoint.position.ToLVector3(), dstPoint.position.ToLVector3());
            useTime = (float) (DateTime.Now - time).TotalMilliseconds;
            Profiler.EndSample();
            //isDrawLine = false;
        }

        var graph = NavMesh.navMeshGraphPath;
        if (graph != null) {
            if (isShowPathResult) ShowResult(graph);
            if (isShowPathProgress) ShowProgress(graph);
            if (isShowAllTriangles) ShowAllTriangles(graph);
            if (isShowTriangleArea) ShowAllConnections(graph);
            if (isShowSingleTriangleConnection) ShowOneTriangleConnections(graph);
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.widthMultiplier = widthMultiplier;
        lineRenderer.SetPositions(pathPoints.ToArray().ToVecArray());
    }


    public int curTriIdx = 0;

    private void ShowOneTriangleConnections(TriangleGraphPath graph){
        var pathTris = new List<Triangle>();
        var triangle = NavMesh._graph.GetTriangle(dstPoint.position.ToLVector3());
        curTriIdx = triangle.index;
        foreach (var conn in triangle.connections) {
            var borderTri = conn.GetToNode();
            if (borderTri != null) {
                pathTris.Add(borderTri);
            }
        }

        ShowTriangles(allMesh, pathTris, Color.magenta);
    }

    private void ShowAllConnections(TriangleGraphPath graph){
        var pathTris = new List<Triangle>();
        var triangle = NavMesh._graph.GetTriangle(dstPoint.position.ToLVector3());
        HashSet<int> allTris = new HashSet<int>();
        pathTris.Add(triangle);
        allTris.Add(triangle.index);
        Queue<Triangle> queue = new Queue<Triangle>();
        queue.Enqueue(triangle);
        while (queue.Count > 0) {
            var tri = queue.Dequeue();
            foreach (var conn in tri.connections) {
                var borderTri = conn.GetToNode();
                if (borderTri != null && allTris.Add(borderTri.index)) {
                    queue.Enqueue(borderTri);
                    pathTris.Add(borderTri);
                }
            }
        }

        ShowTriangles(allMesh, pathTris, Color.magenta);
    }

    private void ShowAllTriangles(TriangleGraphPath graph){
        var pathTris = new List<Triangle>();
        pathTris.Clear();
        var nodes = NavMesh._graph._triangles;
        foreach (var node in nodes) {
            if (node != null
                //&& node.category == IndexedAStarPathFinder<Triangle>.CLOSED
            ) {
                pathTris.Add(node);
            }
        }

        ShowTriangles(progressMesh, pathTris, Color.yellow);
    }

    private void ShowProgress(TriangleGraphPath graph){
        var pathTris = new List<Triangle>();
        pathTris.Clear();
        var nodes = NavMesh._pathFinder._nodeRecords;
        foreach (var node in nodes) {
            if (node != null
                //&& node.category == IndexedAStarPathFinder<Triangle>.CLOSED
            ) {
                pathTris.Add(node.node);
            }
        }

        ShowTriangles(progressMesh, pathTris, Color.yellow);
    }

    private void ShowResult(TriangleGraphPath graph){
        var pathTris = new List<Triangle>();
        pathTris.Clear();
        foreach (var node in graph.nodes) {
            pathTris.Add(node.GetFromNode());
        }

        pathTris.Add(graph.GetEndTriangle());
        ShowTriangles(resultMesh, pathTris, Color.green);
    }

    private void ShowTriangles(MeshFilter filter, List<Triangle> tris, Color rawColor){
        var triCount = tris.Count;
        Mesh mesh = new Mesh();
        if (tris.Count <= 1 || tris[0] == null) {
            triCount = 0;
            mesh.vertices = new Vector3[0];
            mesh.colors = new Color[0];
            mesh.triangles = new int[0];
            mesh.RecalculateBounds();
        }
        else {
            var colors = new Color[triCount * 3];
            var vecs = new Vector3[triCount * 3];
            var idxs = new int[triCount * 3];
            for (int i = 0; i < triCount; i++) {
                var tri = tris[i];
                vecs[i * 3 + 0] = tri.a.ToVector3();
                vecs[i * 3 + 1] = tri.b.ToVector3();
                vecs[i * 3 + 2] = tri.c.ToVector3();
                idxs[i * 3 + 0] = i * 3 + 0;
                idxs[i * 3 + 1] = i * 3 + 1;
                idxs[i * 3 + 2] = i * 3 + 2;
                var color = rawColor * (i * 1.0f / triCount);
                colors[i * 3 + 0] = color;
                colors[i * 3 + 1] = color;
                colors[i * 3 + 2] = color;
            }

            mesh.vertices = vecs;
            mesh.colors = colors;
            mesh.triangles = idxs;
            mesh.RecalculateBounds();
        }

        filter.mesh = mesh;
    }
}