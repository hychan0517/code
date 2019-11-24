using UnityEngine;

public class ThreeDemensionsCamera : MonoBehaviour
{
	public Transform playerTransform;
	public float playerHeight = 1.7f;
	public float standardDistance = 5.0f;
	public float offsetCol = 0.1f;
	public float maxDistance = 20;
	public float minDistance = 0.6f;
	public int minYDegree = -40;
	public int maxYDegree = 80;
	public float rotateSpeed = 0.4f;
	public int zoomSpeed = 200;
	public float lerpAcc = 5.0f;
	public LayerMask m_layerCol = -1;

	private float xDegree = 0.0f;
	private float yDregee = 0.0f;
	private float currentMoveVec;
	private float zoomDistance;
	private float endPosition;
	private Quaternion rotation;

	void Awake()
	{
		Vector3 angle = transform.eulerAngles;
		xDegree = angle.x;
		yDregee = angle.y;
		currentMoveVec = standardDistance;
		zoomDistance = standardDistance;
		endPosition = standardDistance;
	}
	void LateUpdate()
	{
		ChangeRotate();
		//줌인아웃입력 가속도, 속도제한
		zoomDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs(zoomDistance);
		zoomDistance = Mathf.Clamp(zoomDistance, minDistance, maxDistance);
		//충돌이나 입력이 없을시 현재 zoom값의 카메라위치로 돌아오기 위해 이외의 계산은 다른 변수 사용
		endPosition = zoomDistance;
		Vector3 tartgetOffset;
		
		tartgetOffset = new Vector3(0, -playerHeight, 0);
		//보간이 끝난 카메라 위치
		Vector3 position = playerTransform.position - (rotation * Vector3.forward * zoomDistance + tartgetOffset);
		Vector3 trueTargetPosition = playerTransform.position - tartgetOffset;
		//ray 충돌이 있다면 충돌 지점 앞 카메라 위치, 없으면 줌 목표지점까지 선형보간
		RaycastHit hit;
		if (Physics.Linecast(trueTargetPosition, position, out hit, m_layerCol.value))
		{
			endPosition = Vector3.Distance(trueTargetPosition, hit.point) - offsetCol;
			currentMoveVec = endPosition;
		}
		else currentMoveVec = Mathf.Lerp(currentMoveVec, endPosition, Time.deltaTime * lerpAcc);
		currentMoveVec = Mathf.Clamp(currentMoveVec, minDistance, maxDistance);
		transform.position = playerTransform.position - (rotation * Vector3.forward * currentMoveVec + tartgetOffset);
	}
	private void ChangeRotate()
	{
		//상하좌우 회전
		if (Input.GetMouseButton(0))
		{
			xDegree += Input.GetAxis("Mouse X") * rotateSpeed;
			yDregee -= Input.GetAxis("Mouse Y") * rotateSpeed;
		}
		//상하 회전값 제한
		yDregee = ClampAngle(yDregee, minYDegree, maxYDegree);
		rotation = Quaternion.Euler(yDregee, xDegree, 0);
		transform.rotation = rotation;
	}
	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp(angle, min, max);
	}
}