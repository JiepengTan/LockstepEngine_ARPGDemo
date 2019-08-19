using System;
using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using UnityEngine;

[Serializable]
public class CNavMesh {
    [Header("Steering")] public LFloat Speed;
    public LFloat AngularSpeed;
    public LFloat Acceleration;
    public LFloat StoppingDistance;
    public bool AutoBraking;

    [Header("Obstacle Avoidance")] public LFloat Radius;
    public LFloat Height;
    public LFloat Priority;

    [Header("PathFinding ")] public bool AutoTraverseOffMeshLink;
    public bool AutoRepath;

    //[Header("Logic")] 
    //public LFloat UpdateInterval = new LFloat(1);
    //public LFloat NearDist = new LFloat(2);


    public CTransform2D transform2D;

    private LFloat _minSqrReseachDist => (StoppingDistance * StoppingDistance) /4;
    private LVector3 _targetPos;

    public LVector3 TargetPos {
        set => SetDestination(value);
    }


    private LFloat _curSegLen;
    private LFloat _distInCurSeg;
    private int _pointCount;
    private int _nextPointIdx;
    private LVector3[] _path;

    private LVector3 _nextPoint;
    private LVector3 _prePoint;
    public LVector3 curNormal;

    public Action FuncOnReachTargetPos;
    public bool enable = true;
    private bool isFirst = true;

    public void SetDestination(LVector3 targetPos){
        var preTargetPos = _targetPos;
        _targetPos = targetPos;
        if (isFirst || (_targetPos - preTargetPos).sqrMagnitude > _minSqrReseachDist) {
            //需要更加细致的判定 减少寻路需求
            FindPath(false);
        }

        if (isFirst) {
            isFirst = false;
        }
    }

    public void DoUpdate(LFloat deltaTime){
        if (!enable) {
            return;
        }

        if (_path != null) {
            var dist = Speed * deltaTime;
            _distInCurSeg = dist + _distInCurSeg;
            while (_distInCurSeg > _curSegLen) {
                if (_nextPointIdx >= _pointCount - 1) {
                    OnFinishPath();
                    return;
                }

                _distInCurSeg -= _curSegLen;
                NextPoint();
            }

            transform2D.Pos3 = _prePoint + curNormal * _distInCurSeg;
            if ((transform2D.Pos3 - _targetPos).magnitude < StoppingDistance) {
                OnFinishPath();
            }
        }
    }

    void NextPoint(){
        try {
            _nextPointIdx++;
            _nextPoint = _path[_nextPointIdx];
            _prePoint = _path[_nextPointIdx - 1];
            curNormal = (_nextPoint - _prePoint).normalized;
            _curSegLen = (_nextPoint - _prePoint).magnitude;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    void OnFinishPath(){
        _path = null;
        _pointCount = 0;
        FuncOnReachTargetPos?.Invoke();
    }


    void FindPath(bool forceSet = false){
        var tPath = NavMeshManager.FindPath(transform2D.Pos3, _targetPos);
        if (tPath == null || tPath.Count < 2 || tPath[0] == _targetPos) {
            return;
        }

        if ((tPath[0] - transform2D.Pos3).magnitude > 4) {
            int ii = 0;
            tPath = NavMeshManager.FindPath(transform2D.Pos3, _targetPos);
        }

        _path = tPath.ToArray();
        _pointCount = tPath.Count;
        //reset path status
        _distInCurSeg = LFloat.zero;
        _nextPointIdx = 0;
        NextPoint();
    }

    public void OnDrawGizmos(){
        if (_path == null) return;
        var prePoint = transform2D.Pos3;
        for (int i = _nextPointIdx; i < _pointCount; i++) {
            var nextPoint = _path[_nextPointIdx];
            Gizmos.DrawLine(prePoint.ToVector3(), nextPoint.ToVector3());
            prePoint = nextPoint;
        }
    }
}