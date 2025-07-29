using SymphonyFrameWork;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;

public static class InitializeUtility
{
    public static async Task WaitInitialize<T>(T initialize) where T : class
    {
        if (initialize == null)
        {
            Debug.LogError("Initialize instance is null.");
            return;
        }
        if (initialize is IInitializeAsync asyncInitialize)
        {
            await WaitInitialize(asyncInitialize);
        }
        else
        {
            Debug.LogWarning($"Unsupported initialize type: {typeof(T)}");
        }
    }

    public static async Task WaitInitialize(IInitializeAsync initialize)
    {
        if (initialize == null)
        {
            Debug.LogError("Initialize instance is null.");
            return;
        }
        await SymphonyTask.WaitUntil(() => initialize.IsDone);
    }
}
