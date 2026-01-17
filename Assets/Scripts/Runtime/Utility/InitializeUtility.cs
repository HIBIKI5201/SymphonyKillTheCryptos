using SymphonyFrameWork;
using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine;

public static class InitializeUtility
{
    public static async ValueTask WaitInitialize(IInitializeAsync initialize)
    {
        if (initialize == null)
        {
            Debug.LogError("Initialize instance is null.");
            return;
        }

        await SymphonyTask.WaitUntil(() => initialize.IsDone);
    }
}
