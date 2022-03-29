using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
    private Camera mainCamera;
    public float zoomSpeed = 50.0f;

    public Transform target;           // target 
    public float dist = 10.0f;           // 거리
    public float height = 5.0f;         // 높이
    public float dampRotate = 5.0f;  //회전 속도

    private Transform tr;             // 카메라 

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        Zoom();
    }

    void LateUpdate()
    {
        float cur_Y_Angle = Mathf.LerpAngle(tr.eulerAngles.y, target.eulerAngles.y, dampRotate * Time.deltaTime);
        //Mathf.LerpAngle(float s, float e, flaot t) = t시간 동안 s부터 e까지 각도를 변환하는 것.

        Quaternion rot = Quaternion.Euler(0, cur_Y_Angle, 0);

        tr.position = target.position - (rot * Vector3.forward * dist) + (Vector3.up * height);
        //타겟 위치 - 카메라위치 = 카메라가 타겟 뒤로 가야 타겟이 보이겠죠?

        tr.LookAt(target);
    }


    private void Zoom()
    {
        float distance = (float)Input.GetAxis("Mouse ScrollWheel") * -1.0f * zoomSpeed;
        if (distance != 0)
        {
            mainCamera.fieldOfView += distance;
        }
    }
}
