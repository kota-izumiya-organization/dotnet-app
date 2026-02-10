using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services
{
    /// <summary>
    /// タスク管理のためのITaskServiceの実装
    /// </summary>
    public class TaskService : ITaskService, IDisposable
    {
        private readonly List<Models.Task> _tasks;
        private readonly FileStorageService _storageService;
        private readonly int _maxTasks;
        private readonly bool _autoSaveEnabled;
        private bool _disposed = false;

        /// <summary>
        /// TaskServiceの新しいインスタンスを初期化します
        /// </summary>
        public TaskService()
        {
            _tasks = new List<Models.Task>();
            _storageService = new FileStorageService();
            
            // 構成設定を読み込みます
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxTasksPerUser"], out _maxTasks))
            {
                _maxTasks = 100; // デフォルト値
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings["AutoSaveEnabled"], out _autoSaveEnabled))
            {
                _autoSaveEnabled = true; // デフォルト値
            }
        }

        /// <summary>
        /// 新しいタスクを作成します
        /// </summary>
        /// <param name="task">作成するタスク</param>
        /// <returns>非同期操作を表すタスク</returns>
        public async Task<bool> CreateTaskAsync(Models.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Task title cannot be empty", nameof(task));

            if (_tasks.Count >= _maxTasks)
                throw new InvalidOperationException($"Maximum number of tasks ({_maxTasks}) reached");

            // タスクがまだ存在しないことを確認します
            if (_tasks.Any(t => t.Id == task.Id))
                throw new ArgumentException("Task with the same ID already exists", nameof(task));

            _tasks.Add(task);

            if (_autoSaveEnabled)
            {
                // Use ConfigureAwait(false) to avoid deadlocks
                await SaveAsync().ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>
        /// 既存のタスクを更新します
        /// </summary>
        /// <param name="task">更新するタスク</param>
        /// <returns>非同期操作を表すタスク</returns>
        public async Task<bool> UpdateTaskAsync(Models.Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var existingTaskIndex = _tasks.FindIndex(t => t.Id == task.Id);
            if (existingTaskIndex == -1)
                return false;

            task.Touch(); // タイムスタンプを更新
            _tasks[existingTaskIndex] = task;

            if (_autoSaveEnabled)
            {
                // デッドロックを避けるためにConfigureAwait(false)を使用
                await SaveAsync().ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>
        /// IDでタスクを削除します
        /// </summary>
        /// <param name="taskId">削除するタスクのID</param>
        /// <returns>非同期操作を表すタスク</returns>
        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            var taskToRemove = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (taskToRemove == null)
                return false;

            _tasks.Remove(taskToRemove);

            if (_autoSaveEnabled)
            {
                // デッドロックを避けるためにConfigureAwait(false)を使用
                await SaveAsync().ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>
        /// IDでタスクを取得します
        /// </summary>
        /// <param name="taskId">タスクのID</param>
        /// <returns>見つかった場合はタスク、それ以外はnull</returns>
        public async Task<Models.Task> GetTaskByIdAsync(Guid taskId)
        {
            // Simulate async operation for consistency
            await Task.Yield();
            return _tasks.FirstOrDefault(t => t.Id == taskId);
        }

        /// <summary>
        /// すべてのタスクを取得します
        /// </summary>
        /// <returns>すべてのタスクのコレクション</returns>
        public async Task<IEnumerable<Models.Task>> GetAllTasksAsync()
        {
            // Simulate async operation for consistency
            await Task.Yield();
            return _tasks.ToList(); // 外部からの変更を防ぐためにコピーを返します
        }

        /// <summary>
        /// ステータスでタスクを取得します
        /// </summary>
        /// <param name="status">フィルターするステータス</param>
        /// <returns>指定されたステータスを持つタスクのコレクション</returns>
        public async Task<IEnumerable<Models.Task>> GetTasksByStatusAsync(TaskStatus status)
        {
            // Simulate async operation for consistency
            await Task.Yield();
            return _tasks.Where(t => t.Status == status).ToList();
        }

        /// <summary>
        /// 優先度でタスクを取得します
        /// </summary>
        /// <param name="priority">フィルターする優先度</param>
        /// <returns>指定された優先度を持つタスクのコレクション</returns>
        public async Task<IEnumerable<Models.Task>> GetTasksByPriorityAsync(TaskPriority priority)
        {
            // Simulate async operation for consistency
            await Task.Yield();
            return _tasks.Where(t => t.Priority == priority).ToList();
        }

        /// <summary>
        /// 期限超過のタスクを取得します
        /// </summary>
        /// <returns>期限超過のタスクのコレクション</returns>
        public async Task<IEnumerable<Models.Task>> GetOverdueTasksAsync()
        {
            // Simulate async operation for consistency
            await Task.Yield();
            return _tasks.Where(t => t.IsOverdue()).ToList();
        }

        /// <summary>
        /// 特定の担当者に割り当てられたタスクを取得します
        /// </summary>
        /// <param name="assignee">タスクが割り当てられている担当者</param>
        /// <returns>担当者に割り当てられたタスクのコレクション</returns>
        public async Task<IEnumerable<Models.Task>> GetTasksByAssigneeAsync(string assignee)
        {
            if (string.IsNullOrWhiteSpace(assignee))
                throw new ArgumentException("Assignee cannot be null or empty", nameof(assignee));

            // Simulate async operation for consistency
            await Task.Yield();
            return _tasks.Where(t => string.Equals(t.AssignedTo, assignee, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// すべてのタスクを永続的なストレージに保存します
        /// </summary>
        /// <returns>非同期操作を表すタスク</returns>
        public async Task<bool> SaveAsync()
        {
            // デッドロックを避けるためにConfigureAwait(false)を使用
            return await _storageService.SaveTasksAsync(_tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// 永続的なストレージからすべてのタスクを読み込みます
        /// </summary>
        /// <returns>非同期操作を表すタスク</returns>
        public async Task<bool> LoadAsync()
        {
            try
            {
                // デッドロックを避けるためにConfigureAwait(false)を使用
                var loadedTasks = await _storageService.LoadTasksAsync().ConfigureAwait(false);
                
                _tasks.Clear();
                _tasks.AddRange(loadedTasks);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// タスク統計を取得します
        /// </summary>
        /// <returns>タスク統計を含む辞書</returns>
        public Dictionary<string, int> GetTaskStatistics()
        {
            var stats = new Dictionary<string, int>
            {
                ["Total"] = _tasks.Count,
                ["NotStarted"] = _tasks.Count(t => t.Status == TaskStatus.NotStarted),
                ["InProgress"] = _tasks.Count(t => t.Status == TaskStatus.InProgress),
                ["Completed"] = _tasks.Count(t => t.Status == TaskStatus.Completed),
                ["Cancelled"] = _tasks.Count(t => t.Status == TaskStatus.Cancelled),
                ["OnHold"] = _tasks.Count(t => t.Status == TaskStatus.OnHold),
                ["Overdue"] = _tasks.Count(t => t.IsOverdue()),
                ["High Priority"] = _tasks.Count(t => t.Priority == TaskPriority.High || t.Priority == TaskPriority.Critical)
            };

            return stats;
        }

        /// <summary>
        /// TaskServiceを破棄します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 保護されたdisposeメソッド
        /// </summary>
        /// <param name="disposing">マネージリソースを破棄する場合はtrue</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _storageService?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
