using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class HpFill : MonoBehaviour
{
	public Image hp;
	private const float fHp = 100;
	private const float duration = 0.5f;

	private float cHp = 100;
	private float damage = 10; //테스트용
	//Fill 변경 중 추가로 Damage 들어올 때 이어서 체력을 깎기위한 변수
	private Coroutine fillCoroutine;
	private float saveDamage = 0; 
	private void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Damage();
		}
	}
	public void Damage()
	{
		if (fillCoroutine != null)
		{
			StopCoroutine(fillCoroutine);
		}
		fillCoroutine = StartCoroutine(Damage(damage, saveDamage, duration));
	}
	IEnumerator Damage(float damage,float save, float duration)
	{
		saveDamage = damage + save;
		cHp -= damage;
		float cTime = 0;
		//전체 체력대비 깍아야하는 체력의 비율
		float minus = saveDamage / fHp;
		while (cTime < duration)
		{
			cTime += Time.deltaTime;
			saveDamage -= saveDamage * (Time.deltaTime / duration);
			//현채 fill에서 추가로 깎는다 ~초 까지
			hp.fillAmount -= minus * (Time.deltaTime / duration);
			if (hp.fillAmount < cHp / fHp) break;
			yield return null;
		}
		hp.fillAmount = cHp / fHp;
		saveDamage = 0;
		fillCoroutine = null;
	}
}