using System;

namespace TaskManager.Models
{
    /// <summary>
    /// タスクの優先度レベルを表します
    /// </summary>
    public enum TaskPriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Critical = 4
    }
}
