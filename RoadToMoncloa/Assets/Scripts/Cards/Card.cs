using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Drag and drop from https://answers.unity.com/questions/1138645/how-do-i-drag-a-sprite-around.html
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse up");
    }
}
