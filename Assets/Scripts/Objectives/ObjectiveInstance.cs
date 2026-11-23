using UnityEngine;

public class ObjectiveInstance
{
    public Objective objective;
    public ObjectiveState state;

    public int progress;

    public ObjectiveInstance(Objective objective)
    {
        this.objective = objective;
        state = ObjectiveState.Locked;
        progress = 0;
    }
}
