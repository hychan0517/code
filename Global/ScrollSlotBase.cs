using UnityEngine;

public abstract class ScrollSlotBase : MonoBehaviour
{
	public abstract RectTransform GetRectTransform();
	public abstract void SettingSlot(object data);
}
