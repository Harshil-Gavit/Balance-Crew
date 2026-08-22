using UnityEngine;

public class BalanceDebug : MonoBehaviour
{
    [SerializeField] private BalanceDetector detector;

    private void OnGUI()
    {
        GUI.Label(
            new Rect(20, 20, 500, 200),
            "STATE: " + detector.state +
            "\nDIRECTION: " + detector.direction +
            "\nTILT: " + detector.tiltAngle.ToString("F1") +
            "\nBALANCE X: " + detector.balanceX.ToString("F2") +
            "\nBALANCE Z: " + detector.balanceZ.ToString("F2") +
            "\nFALL AMOUNT: " + detector.fallAmount.ToString("F2")
        );
    }
}