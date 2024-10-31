using UnityEngine;

public class TestActionKit : MonoBehaviour
{
    private void Start()
    {
        var act = new ActionKit();

        act.AddAction(1f, new CallbackAction(() => Debug.Log("Here")));
        act.Play();
    }
}
