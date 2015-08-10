using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class DragHandler : MonoBehaviour {
	public static GameObject itemBeingDragged;


	/*#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		Touch touch = Input.touches.FirstOrDefault(t => t.position.x > Screen.width/2);
		Vector3 pos;
		if (touch.position.x != 0 && touch.position.y != 0) 
		{
			pos = Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 0));
		} 
		else 
		{
			pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		}
		pos.Set (pos.x, pos.y, gameObject.transform.position.z);
		gameObject.transform.position = pos;

	}


	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		itemBeingDragged = null;
	}

	#endregion*/
}
