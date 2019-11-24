using System.Collections;
using UnityEngine;
using DG.Tweening;
public class CameraMove : MonoBehaviour
{
	private const float endTime = 0.1f;
	private float delayTime;
	private const float speed = 0.1f;
	//카메라 초기위치
	private Vector3 startPos;
	//진동 세기
	private const float randomRange = 0.3f;
	//진동으로 움직일 목표 지점
	private Vector3 goalPos;
	//재동작 방지 불린
	private bool isReady = true;
	//목표지점 도달시 다음 목표는 초기 카메라 위치
	private bool isreturn;
	private Vector3 dir;
	public void OnStart()
	{
		if (isReady == false) return;
		isReady = false;
		isreturn = false;
		startPos = gameObject.transform.position;
		delayTime = 0;
		//최초 목표 위치 설정
		SetNextPosition();
		StartCoroutine(StartCameraMove());
	}
	IEnumerator StartCameraMove()
	{
		while (true)
		{
			delayTime += Time.deltaTime;
			GoToGoal();
			if (CheckEndTime() == true) yield break;
			else yield return null;
		}
	}
	private void GoToGoal()
	{
		Move();
		if (CheckDistance()) SetNextPosition();
	}
	private void SetNextPosition()
	{
		//목표지점 도달시 다음 목표는 최초카메라 위치
		if (isreturn)
		{
			//목표 위치 설정 - 최초위치 + 랜덤 x,y값
			isreturn = false;
			float fX = Random.Range(-randomRange, randomRange);
			float fY = Random.Range(-randomRange, randomRange);
			goalPos = new Vector3(startPos.x + fX, startPos.y + fY, startPos.z);
			dir = (goalPos - gameObject.transform.position).normalized;
		}
		else
		{
			isreturn = true;
			goalPos = startPos;
			dir = (goalPos - gameObject.transform.position).normalized;
		}
	}
	private bool CheckEndTime()
	{
		if (delayTime >= endTime)
		{
			OnEnd();
			return true;
		}
		return false;
	}
	private void OnEnd()
	{
		isReady = true;
		gameObject.transform.position = startPos;
		delayTime = endTime;
	}
	private bool CheckDistance()
	{
		if ((goalPos - gameObject.transform.position).magnitude <= speed)
		{
			return true;
		}
		return false;
	}
	private void Move()
	{
		gameObject.transform.position += new Vector3(dir.x * speed, dir.y * speed, 0);
	}
}
