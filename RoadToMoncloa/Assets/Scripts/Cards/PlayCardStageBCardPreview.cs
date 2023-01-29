using UnityEngine;

public class PlayCardStageBCardPreview : PlayCardStagePreview
{
    private Vector3 _normalScale;

    private void Start()
    {
        _normalScale = gameObject.transform.localScale;
    }

    public override void Enter()
    {
        gameObject.transform.localScale = _normalScale * 1.2f;
    }

    public override void Exit()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}