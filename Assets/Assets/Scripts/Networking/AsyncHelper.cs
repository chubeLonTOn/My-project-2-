using System;
using System.Collections;
using System.Threading.Tasks;

namespace Assets.Scripts.Networking
{
    public static class AsyncHelper
    {
        public static IEnumerator WrapTask(Task task, Action<bool> onComplete)
        {
            while (!task.IsCompleted)
                yield return null;
            onComplete?.Invoke(!task.IsFaulted);
        }

        public static IEnumerator WrapTask<T>(Task<T> task, Action<bool, T> onComplete)
        {
            while (!task.IsCompleted)
                yield return null;
            if(task.IsCompletedSuccessfully)
                onComplete?.Invoke(true, task.Result);
            else
                onComplete?.Invoke(false, default);
        }
    }
}