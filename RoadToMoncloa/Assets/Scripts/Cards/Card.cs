using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Drag and drop from https://answers.unity.com/questions/1138645/how-do-i-drag-a-sprite-around.html
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Card : MonoBehaviour
{
    [SerializeField] private int _votersWon;
    [SerializeField] private int _moneyWon;

    private Game _game;

    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isOnVotersZone;
    private bool isOnMoneyZone;
    private Vector3 originalPosition;

    private void Start()
    {
        _game = FindObjectOfType<Game>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.VotersCardDropZone))
        {
            isOnVotersZone = true;
        }
        else if (collision.CompareTag(Tags.MoneyCardDropZone))
        {
            isOnMoneyZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.VotersCardDropZone))
        {
            isOnVotersZone = false;
        }
        else if (collision.CompareTag(Tags.MoneyCardDropZone))
        {
            isOnMoneyZone = false;
        }
    }

    void OnMouseDown()
    {
        originalPosition = transform.position;
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
        if (isOnVotersZone && isOnMoneyZone)
        {
            Debug.Log("Dropped in the middle");
        }
        else if (isOnVotersZone)
        {
            Debug.Log("Dropped in voters zone");
            PlayForVoters();
        }
        else if (isOnMoneyZone)
        {
            Debug.Log("Dropped in money zone");
            PlayForMoney();
        }

        transform.position = originalPosition;
    }

    private void PlayForVoters()
    {
        _game.UpdateVotersCount(_votersWon);
        _game.DestroyCard(this);
    }

    private void PlayForMoney()
    {
        _game.UpdateMoneyCount(_moneyWon);
        _game.DestroyCard(this);
    }
}
