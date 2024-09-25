using UnityEngine;

public class ScreenShootCreator : MonoBehaviour
{
    [SerializeField] private string displayRes;
    private int screenShootCount;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ScreenCapture.CaptureScreenshot($"/users/taha/Desktop/2048ScreenShoot_{displayRes}_{screenShootCount++}.png");
        }
    }
}
