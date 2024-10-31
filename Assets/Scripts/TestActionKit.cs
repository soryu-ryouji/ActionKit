using UnityEngine;

public class TestActionKit : MonoBehaviour
{
    private void Start()
    {
        var act = new ActionKit();
        Timeline timeline = new Timeline();
        timeline.AddAction(1f, new CallbackAction(() => Debug.Log("Here")));
        act.Play(timeline);

        Sequence sequence = new Sequence();
        sequence.AddAction(new CallbackAction(() => Debug.Log("Seq 1")));
        sequence.AddAction(new CallbackAction(() => Debug.Log("Seq 2")));
        sequence.AddAction(new CallbackAction(() => Debug.Log("Seq 3")));
        act.Play(sequence);
    }
}
