using System;

namespace TaskManager.Models
{
    /// <summary>
    /// タスクのステータスを表します
    /// </summary>
    public enum TaskStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        OnHold = 4
    }
}
