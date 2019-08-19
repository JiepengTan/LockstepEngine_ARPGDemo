using UnityEngine;
using System.Collections;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public float rotateSpd = 5f;
    //旋转变量
    private float deltaX = 0f;
    private float deltaY = 0f;
    //缩放变量
    public float distance = 10f;

    public Vector3 targetCamPos;
    public Vector3 targetOffset = new Vector3(0, 0.5f, 0);
    public float maxCamDist = 10;
    public float minCamDist = 1;
    public Transform target;
    public float LerpTime = 0.1f;
    private Vector3 TargetPos { get { return target.position + targetOffset; } }
    //移动变量
    private Transform camTrans;
    void Awake()
    {
        Instance = this;
        camTrans = Camera.main.transform;
    }
    void Update()
    {
        if (target == null) { return; }
        var diffNormal = (camTrans.position - TargetPos).normalized;
        var camDist = (camTrans.position - TargetPos).magnitude;
        if (camDist < minCamDist || camDist > maxCamDist)
        {
            var rawY = targetCamPos.y;
            targetCamPos = Vector3.Lerp(targetCamPos, diffNormal.normalized * Mathf.Clamp(camDist, minCamDist, maxCamDist) + TargetPos, LerpTime);
            targetCamPos.y = rawY;
        }
        RaycastHit hitInfo;
        if (Physics.Raycast(TargetPos + diffNormal * 0.5f, diffNormal, out hitInfo, camDist))
        {
            targetCamPos = Vector3.Lerp(targetCamPos, target.position + (hitInfo.distance - 0.5f) * diffNormal, LerpTime);
        }

        camTrans.position = Vector3.Lerp(camTrans.position, targetCamPos, 0.3f);
        var _rawRotation = camTrans.rotation;
        camTrans.LookAt(TargetPos);
        var _targetRotation = camTrans.rotation;
        camTrans.rotation = Quaternion.Lerp(_rawRotation, _targetRotation, 0.05f);
        //鼠标右键点下控制相机旋转;
        //var swipe = FM.InputManager.Instance.swipe;
        if (Input.GetMouseButton(1))
        {
           // deltaX += swipe.x * rotateSpd * Time.deltaTime;
           // deltaY -= swipe.y * rotateSpd * Time.deltaTime;
            deltaX = ClampAngle(deltaX, -360, 360);
            deltaY = ClampAngle(deltaY, -70, 70);
            //camTrans.rotation = Quaternion.Euler(m_deltY, m_deltX, 0);
            targetCamPos = GetCameraTargetPosition(deltaX, deltaY, distance, TargetPos);
        }

        //鼠标中键点下场景缩放;
       // var Scale = FM.InputManager.Instance.Scale;
       // if (Scale != 0)
       // {
       //     //自由缩放方式;
       //     distance -= Scale * 10f;
       //     targetCamPos = GetCameraTargetPosition(deltaX, deltaY, distance, target.position);
       // }
    }

    public Vector3 GetCameraTargetPosition(float x, float y, float desireDistance, Vector3 targetPostion)
    {
        Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
        Vector3 disVector = new Vector3(0.0f, 0.0f, -desireDistance);
        Vector3 position = rotation * disVector + targetPostion;
        RaycastHit hitInfo;
        var _diff = (position - targetPostion).normalized;
        if (Physics.Raycast(targetPostion + _diff * 0.5f, _diff, out hitInfo, distance))
        {
            position = targetPostion + (hitInfo.distance - 0.1f) * _diff;
        }
        return position;
    }
    //规划角度;
    float ClampAngle(float angle, float minAngle, float maxAgnle)
    {
        if (angle <= -360)
            angle += 360;
        if (angle >= 360)
            angle -= 360;

        return Mathf.Clamp(angle, minAngle, maxAgnle);
    }
}
