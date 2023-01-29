using UnityEngine;

public class PlayCardStageNormalCardPreview : PlayCardStagePreview
{
    public override void Enter()
    {
        gameObject.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        MoveCardUp();
    }

    public override void Exit()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void MoveCardUp()
    {
        var position = gameObject.transform.position;
        position.y += .6f;
        gameObject.transform.position = position;
    }
}