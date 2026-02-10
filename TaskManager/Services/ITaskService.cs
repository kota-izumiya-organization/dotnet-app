using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services
{
    /// <summary>
    /// タスク管理操作のインターフェース
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// 新しいタスクを作成します
        /// </summary>
        /// <param name="task">作成するタスク</param>
        /// <returns>非同期操作を表すタスク</returns>
        Task<bool> CreateTaskAsync(Models.Task task);

        /// <summary>
        /// 既存のタスクを更新します
        /// </summary>
        /// <param name="task">更新するタスク</param>
        /// <returns>非同期操作を表すタスク</returns>
        Task<bool> UpdateTaskAsync(Models.Task task);

        /// <summary>
        /// IDでタスクを削除します
        /// </summary>
        /// <param name="taskId">削除するタスクのID</param>
        /// <returns>非同期操作を表すタスク</returns>
        Task<bool> DeleteTaskAsync(Guid taskId);

        /// <summary>
        /// IDでタスクを取得します
        /// </summary>
        /// <param name="taskId">タスクのID</param>
        /// <returns>見つかった場合はタスク、それ以外はnull</returns>
        Task<Models.Task> GetTaskByIdAsync(Guid taskId);

        /// <summary>
        /// すべてのタスクを取得します
        /// </summary>
        /// <returns>すべてのタスクのコレクション</returns>
        Task<IEnumerable<Models.Task>> GetAllTasksAsync();

        /// <summary>
        /// ステータスでタスクを取得します
        /// </summary>
        /// <param name="status">フィルターするステータス</param>
        /// <returns>指定されたステータスを持つタスクのコレクション</returns>
        Task<IEnumerable<Models.Task>> GetTasksByStatusAsync(TaskStatus status);

        /// <summary>
        /// 優先度でタスクを取得します
        /// </summary>
        /// <param name="priority">フィルターする優先度</param>
        /// <returns>指定された優先度を持つタスクのコレクション</returns>
        Task<IEnumerable<Models.Task>> GetTasksByPriorityAsync(TaskPriority priority);

        /// <summary>
        /// 期限超過のタスクを取得します
        /// </summary>
        /// <returns>期限超過のタスクのコレクション</returns>
        Task<IEnumerable<Models.Task>> GetOverdueTasksAsync();

        /// <summary>
        /// 特定の担当者に割り当てられたタスクを取得します
        /// </summary>
        /// <param name="assignee">タスクが割り当てられている担当者</param>
        /// <returns>担当者に割り当てられたタスクのコレクション</returns>
        Task<IEnumerable<Models.Task>> GetTasksByAssigneeAsync(string assignee);

        /// <summary>
        /// すべてのタスクを永続的なストレージに保存します
        /// </summary>
        /// <returns>非同期操作を表すタスク</returns>
        Task<bool> SaveAsync();

        /// <summary>
        /// 永続的なストレージからすべてのタスクを読み込みます
        /// </summary>
        /// <returns>非同期操作を表すタスク</returns>
        Task<bool> LoadAsync();
    }
}
