using UnityEngine;

public class AnomalyGroup : MonoBehaviour
{
    [Header("Objects in this group")]
    public AnomalyObject[] groupMembers;

    public void SetAnomaly(bool isAnomaly)
    {
        foreach (AnomalyObject obj in groupMembers)
        {
            if (obj != null)
                obj.SetAnomaly(isAnomaly);
        }
    }
}