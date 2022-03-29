using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splineMgr : MonoBehaviour
{
	
	//Has to be at least 4 points
	public Transform[] controlPointsList;

	//Are we making a line or a loop?
	public bool isLooping = true;

	//The spline's resolution
	//Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
	public float resolution = 0.02f; // 0.02 = 1/50 , 즉 50 개임, 최저 11개 이상 

	public GameObject hellicopter;

	float act = 0;

	int index = 0;
	List<Vector3> points = new List<Vector3>();


	// Start is called before the first frame update
	void Start()
    {
		collectAllPoinstAroundControlPoints();
	}

	void collectAllPoinstAroundControlPoints()
    {
		for (int i = 0; i < controlPointsList.Length; i++)
		{
			// isLooping 이 false 일 경우 처음과 끝 부분 제외함
			if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isLooping)
			{
				continue;
			}

			gatherPoints_CatmullRomSpline(i);
		}
	}

	// Update is called once per frame
	void Update()
	{
		movHelicopter();
	}

	void movHelicopter()
    {
		act += Time.deltaTime;

		if (points.Count > 0 && act > 0.05f)
		{
			act = 0;
			index = index % points.Count;
			hellicopter.transform.position = points[index];

			
			/* 다음 지점 바라보게함 */
			int indexNext;
			if (index + 1 < points.Count)
				indexNext = index + 1;
			else
				indexNext = (index + 1) % points.Count;

			hellicopter.transform.LookAt(points[indexNext]);// 다음 지점을 바라보게


			/*
			 *  헬리콥터가 좌우 롤링 할 수 있게 함
			 *    진행방향을 회전축으로 +,- 약간 돌려준다.
			 *    회전하는 정도는 vector 2개를 cross (외적) 하여 그 결과 벡터 x,y,z 중에 y 만 사용한다.
			 *    vector 2 개는 
			 *    v1 = 현재 위치 - 10 포인트 이전 점 
			 *    v2 = 이후 10 포인트 지점 - 현재 위치
			 *    로 얼마나 이전 대비 커브가 휘어지느냐를 판단한다.
			 *    즉 , 좌로 휘어지나 우로 휘어지나 그리고 휘어진다면 얼마나 많이 휘어지나 등을 외적으로 판단한다.
			 */

			// 현재 진행 방향 , 5칸 
			Vector3 curDir = Vector3.zero;
			if (index + 5 < points.Count) // 경계선이 아닐 경우 
				curDir = points[index + 5] - points[index];
			else
				curDir = points[(index + 5) % points.Count] - points[index];

			// pre : 10 칸 전
			int preIndex;
			if (index < 10) // 경계선 부위일 경우 index 
				preIndex = (points.Count) - (10 - index);
			else
				preIndex = index - 10;

			Vector3 pre = points[preIndex];

			// next : 10 칸 후 
			int nextIndex;
			if (index >= points.Count - 10) // 경계선 부위일 경우 index 
				nextIndex = (index + 10) - (points.Count);
			else
				nextIndex = index + 10;

			Vector3 next = points[nextIndex];


			// 헬리콥터가 진행 방향 대비 좌우로 기울어지는 Roll 구현 
			Vector3 cur = points[index];
			Vector3 dir1 = cur - pre; // 과거부터 현재 진행방향 
			Vector3 dir2 = next - cur; // 현재부터 미래 진행방향 

			// 과거 진행 대비 미래 진행 방향 체크,우로 꺽이는지 좌로 꺽이는지
			Vector3 cross = Vector3.Cross(dir1.normalized, dir2.normalized);

			//Debug.LogFormat("norm {0} index {1} cross {2}", curDir.normalized, index, cross.y * 90.0f);

			Quaternion axisRot = Quaternion.AngleAxis(-cross.y * 90.0f, curDir.normalized); // 진행방향 축을 기준으로 회전 
			hellicopter.transform.rotation = axisRot * hellicopter.transform.rotation; // 진행 축 회전 후 원래 회전(위에서 LookAt)

			index++;
		}
	}

	//Display without having to press play
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;

		//Draw the Catmull-Rom spline between the points
		for (int i = 0; i < controlPointsList.Length; i++)
		{
			//Cant draw between the endpoints
			//Neither do we need to draw from the second to the last endpoint
			//...if we are not making a looping line
			if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isLooping)
			{
				continue;
			}

			DisplayCatmullRomSpline(i);
		}
	}

	//Display a spline between 2 points derived with the Catmull-Rom spline algorithm
	void DisplayCatmullRomSpline(int pos)
	{
		//The 4 points we need to form a spline between p1 and p2
		Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].position;
		Vector3 p1 = controlPointsList[pos].position;
		Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].position;
		Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].position;

		//The start position of the line
		Vector3 lastPos = p1;

		//How many times should we loop?
		int loops = Mathf.FloorToInt(1f / resolution);

		for (int i = 1; i <= loops; i++)
		{
			//Which t position are we at?
			float t = i * resolution;

			//Find the coordinate between the end points with a Catmull-Rom spline
			Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

			//Draw this line segment
			Gizmos.DrawLine(lastPos, newPos);


			//Save this pos so we can draw the next line segment
			lastPos = newPos;
		}
	}

	void gatherPoints_CatmullRomSpline(int pos)
	{
		//The 4 points we need to form a spline between p1 and p2
		Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].position;
		Vector3 p1 = controlPointsList[pos].position;
		Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].position;
		Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].position;

		//The start position of the line
		Vector3 lastPos = p1;

		//How many times should we loop?
		int loops = Mathf.FloorToInt(1f / resolution);

		for (int i = 1; i <= loops; i++)
		{
			//Which t position are we at?
			float t = i * resolution;

			//Find the coordinate between the end points with a Catmull-Rom spline
			Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

			// 점들을 수집한다.
			points.Add(newPos);

			//Save this pos so we can draw the next line segment
			lastPos = newPos;
		}
	}


	//Clamp the list positions to allow looping
	int ClampListPos(int pos)
	{
		if (pos < 0)
		{
			pos = controlPointsList.Length - 1;
		}

		if (pos > controlPointsList.Length)
		{
			pos = 1;
		}
		else if (pos > controlPointsList.Length - 1)
		{
			pos = 0;
		}

		return pos;
	}

	//Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
	//http://www.iquilezles.org/www/articles/minispline/minispline.htm
	Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

		//The cubic polynomial: a + b * t + c * t^2 + d * t^3
		Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}
}
