using UnityEngine;

public static class PauseManager
{
    private static int pauseRequests = 0;

    public static void RequestPause()
    {
        pauseRequests++;
        Time.timeScale = 0f;
    }

    public static void ReleasePause()
    {
        pauseRequests = Mathf.Max(0, pauseRequests - 1);
        if (pauseRequests == 0)
            Time.timeScale = 1f;
    }
}