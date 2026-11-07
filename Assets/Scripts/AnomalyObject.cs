using UnityEngine;

public class AnomalyObject : MonoBehaviour
{
    public enum AnomalyType
    {
        Position,       // Objek berpindah
        Rotation,       // Objek berputar
        Scale,          // Objek membesar/mengecil
        Visibility,     // Objek menghilang/muncul
        Combined        // Posisi + rotasi sekaligus
    }

    [Header("Anomaly Type")]
    public AnomalyType anomalyType = AnomalyType.Position;

    [Header("Position")]
    public Vector3 normalPosition;
    public Vector3 anomalyPosition;

    [Header("Rotation")]
    public Vector3 normalRotation;
    public Vector3 anomalyRotation;

    [Header("Scale")]
    public Vector3 normalScale;
    public Vector3 anomalyScale;

    [Header("Visibility")]
    public bool hideOnAnomaly = true;

    void Awake()
    {
        // Auto-grab nilai awal dari transform
        if (normalPosition == Vector3.zero)
            normalPosition = transform.localPosition;

        if (normalRotation == Vector3.zero)
            normalRotation = transform.localEulerAngles;

        if (normalScale == Vector3.zero)
            normalScale = transform.localScale;
    }

    public void SetAnomaly(bool isAnomaly)
    {
        switch (anomalyType)
        {
            case AnomalyType.Position:
                transform.localPosition = isAnomaly ? anomalyPosition : normalPosition;
                break;

            case AnomalyType.Rotation:
                transform.localEulerAngles = isAnomaly ? anomalyRotation : normalRotation;
                break;

            case AnomalyType.Scale:
                transform.localScale = isAnomaly ? anomalyScale : normalScale;
                break;

            case AnomalyType.Visibility:
                gameObject.SetActive(hideOnAnomaly ? !isAnomaly : isAnomaly);
                break;

            case AnomalyType.Combined:
                transform.localPosition = isAnomaly ? anomalyPosition : normalPosition;
                transform.localEulerAngles = isAnomaly ? anomalyRotation : normalRotation;
                break;
        }
    }

    public ReportSystem.AnomalyType GetAnomalyType()
    {
        switch (anomalyType)
        {
            case AnomalyType.Visibility:
                return hideOnAnomaly ?
                    ReportSystem.AnomalyType.MissingObject :
                    ReportSystem.AnomalyType.ExtraObject;
            case AnomalyType.Position:
                return ReportSystem.AnomalyType.PositionChanged;
            case AnomalyType.Rotation:
                return ReportSystem.AnomalyType.RotationChanged;
            case AnomalyType.Scale:
                return ReportSystem.AnomalyType.ScaleChanged;
            case AnomalyType.Combined:
                return ReportSystem.AnomalyType.PositionChanged;
            default:
                return ReportSystem.AnomalyType.None;
        }
    }
}