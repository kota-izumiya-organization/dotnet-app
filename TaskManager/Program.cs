using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager
{
    /// <summary>
    /// タスクマネージャーアプリケーションのメインプログラムクラス
    /// </summary>
    class Program
    {
        private static ITaskService _taskService;

        /// <summary>
        /// アプリケーションのメインエントリーポイント
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        static async Task Main(string[] args)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("   Task Manager Demo Application");
            Console.WriteLine("=================================");
            Console.WriteLine();

            // 適切な破棄を伴うタスクサービスの初期化
            using (_taskService = new TaskService())
            {
                // 既存のタスクを読み込む
                Console.WriteLine("Loading existing tasks...");
                // デッドロックを回避するためConfigureAwait(false)を使用
                var loadResult = await _taskService.LoadAsync().ConfigureAwait(false);
                
                if (loadResult)
                {
                    var allTasks = await _taskService.GetAllTasksAsync().ConfigureAwait(false);
                    Console.WriteLine($"Loaded {allTasks.Count()} existing tasks.");
                }
                else
                {
                    Console.WriteLine("No existing tasks found or error loading. Starting fresh.");
                }

                Console.WriteLine();

                // タスクが存在しない場合はサンプルデータを作成
                await CreateSampleDataIfNeeded().ConfigureAwait(false);

                // メインアプリケーションループ
                var running = true;
                while (running)
                {
                    try
                    {
                        DisplayMenu();
                        var choice = Console.ReadLine();

                        // すべての非同期操作にConfigureAwait(false)を使用
                        var continueRunning = await ProcessMenuChoice(choice).ConfigureAwait(false);
                        running = continueRunning;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }

                Console.WriteLine("\nSaving tasks before exit...");
                await _taskService.SaveAsync().ConfigureAwait(false);
                Console.WriteLine("Thank you for using Task Manager!");
            }
        }

        /// <summary>
        /// メインメニューを表示
        /// </summary>
        private static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Task Manager ===");
            Console.WriteLine("1.  View All Tasks");
            Console.WriteLine("2.  Create New Task");
            Console.WriteLine("3.  Update Task");
            Console.WriteLine("4.  Delete Task");
            Console.WriteLine("5.  View Tasks by Status");
            Console.WriteLine("6.  View Tasks by Priority");
            Console.WriteLine("7.  View Overdue Tasks");
            Console.WriteLine("8.  View Tasks by Assignee");
            Console.WriteLine("9.  View Task Statistics");
            Console.WriteLine("10. Search Tasks");
            Console.WriteLine("0.  Exit");
            Console.WriteLine();
            Console.Write("Select an option: ");
        }

        /// <summary>
        /// メニュー選択を処理
        /// </summary>
        /// <param name="choice">ユーザーのメニュー選択</param>
        /// <returns>実行を続ける場合はTrue、終了する場合はFalse</returns>
        private static async Task<bool> ProcessMenuChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    await ViewAllTasks().ConfigureAwait(false);
                    break;
                case "2":
                    await CreateNewTask().ConfigureAwait(false);
                    break;
                case "3":
                    await UpdateTask().ConfigureAwait(false);
                    break;
                case "4":
                    await DeleteTask().ConfigureAwait(false);
                    break;
                case "5":
                    await ViewTasksByStatus().ConfigureAwait(false);
                    break;
                case "6":
                    await ViewTasksByPriority().ConfigureAwait(false);
                    break;
                case "7":
                    await ViewOverdueTasks().ConfigureAwait(false);
                    break;
                case "8":
                    await ViewTasksByAssignee().ConfigureAwait(false);
                    break;
                case "9":
                    await ViewTaskStatistics().ConfigureAwait(false);
                    break;
                case "10":
                    await SearchTasks().ConfigureAwait(false);
                    break;
                case "0":
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }

            return true;
        }

        /// <summary>
        /// タスクが存在しない場合はサンプルデータを作成
        /// </summary>
        private static async Task CreateSampleDataIfNeeded()
        {
            var existingTasks = await _taskService.GetAllTasksAsync().ConfigureAwait(false);
            if (existingTasks.Any())
            {
                return; // タスクは既に存在する
            }

            Console.WriteLine("Creating sample tasks for demonstration...");

            var sampleTasks = new[]
            {
                new Models.Task
                {
                    Title = "Complete Project Documentation",
                    Description = "Write comprehensive documentation for the new project including API docs and user guides",
                    Priority = TaskPriority.High,
                    Status = TaskStatus.InProgress,
                    AssignedTo = "John Doe",
                    DueDate = DateTimeOffset.Now.AddDays(3),
                    EstimatedHours = 8.0,
                    ActualHours = 4.5,
                    Tags = "documentation, project, api"
                },
                new Models.Task
                {
                    Title = "Fix Login Bug",
                    Description = "Users are unable to login with special characters in their passwords",
                    Priority = TaskPriority.Critical,
                    Status = TaskStatus.NotStarted,
                    AssignedTo = "Jane Smith",
                    DueDate = DateTimeOffset.Now.AddDays(1),
                    EstimatedHours = 4.0,
                    ActualHours = 0.0,
                    Tags = "bug, authentication, security"
                },
                new Models.Task
                {
                    Title = "Database Cleanup",
                    Description = "Remove old records and optimize database performance",
                    Priority = TaskPriority.Normal,
                    Status = TaskStatus.Completed,
                    AssignedTo = "Bob Johnson",
                    DueDate = DateTimeOffset.Now.AddDays(-5),
                    EstimatedHours = 6.0,
                    ActualHours = 5.5,
                    Tags = "database, maintenance, performance"
                },
                new Models.Task
                {
                    Title = "Implement Dark Mode",
                    Description = "Add dark mode support to the application UI",
                    Priority = TaskPriority.Low,
                    Status = TaskStatus.OnHold,
                    AssignedTo = "Alice Wilson",
                    DueDate = DateTimeOffset.Now.AddDays(14),
                    EstimatedHours = 12.0,
                    ActualHours = 2.0,
                    Tags = "ui, feature, enhancement"
                },
                new Models.Task
                {
                    Title = "Security Audit",
                    Description = "Conduct comprehensive security audit of the application",
                    Priority = TaskPriority.High,
                    Status = TaskStatus.InProgress,
                    AssignedTo = "Charlie Brown",
                    DueDate = DateTimeOffset.Now.AddDays(-2), // 期限切れ
                    EstimatedHours = 16.0,
                    ActualHours = 10.0,
                    Tags = "security, audit, compliance"
                }
            };

            foreach (var task in sampleTasks)
            {
                await _taskService.CreateTaskAsync(task).ConfigureAwait(false);
            }

            Console.WriteLine($"Created {sampleTasks.Length} sample tasks.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// すべてのタスクを表示
        /// </summary>
        private static async Task ViewAllTasks()
        {
            Console.Clear();
            Console.WriteLine("=== All Tasks ===");
            Console.WriteLine();

            var tasks = await _taskService.GetAllTasksAsync().ConfigureAwait(false);
            
            if (!tasks.Any())
            {
                Console.WriteLine("No tasks found.");
            }
            else
            {
                DisplayTaskList(tasks);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// 新しいタスクを作成
        /// </summary>
        private static async Task CreateNewTask()
        {
            Console.Clear();
            Console.WriteLine("=== Create New Task ===");
            Console.WriteLine();

            try
            {
                var task = new Models.Task();

                Console.Write("Title: ");
                task.Title = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(task.Title))
                {
                    Console.WriteLine("Title is required!");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Description: ");
                task.Description = Console.ReadLine();

                Console.Write("Assigned to: ");
                task.AssignedTo = Console.ReadLine();

                // 優先度の選択
                Console.WriteLine("\nSelect Priority:");
                Console.WriteLine("1. Low");
                Console.WriteLine("2. Normal");
                Console.WriteLine("3. High");
                Console.WriteLine("4. Critical");
                Console.Write("Choice (1-4): ");
                
                if (int.TryParse(Console.ReadLine(), out var priorityChoice) && 
                    priorityChoice >= 1 && priorityChoice <= 4)
                {
                    task.Priority = (TaskPriority)priorityChoice;
                }

                // 期限日
                Console.Write("Due date (MM/dd/yyyy) or press Enter to skip: ");
                var dueDateInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(dueDateInput))
                {
                    if (DateTime.TryParse(dueDateInput, out var dueDate))
                    {
                        task.DueDate = new DateTimeOffset(dueDate, TimeZoneInfo.Local.GetUtcOffset(dueDate));
                    }
                }

                // 見積もり時間
                Console.Write("Estimated hours (or press Enter to skip): ");
                var estimatedHoursInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(estimatedHoursInput) && 
                    double.TryParse(estimatedHoursInput, out var estimatedHours))
                {
                    task.EstimatedHours = estimatedHours;
                }

                Console.Write("Tags (comma-separated): ");
                task.Tags = Console.ReadLine();

                await _taskService.CreateTaskAsync(task).ConfigureAwait(false);
                Console.WriteLine("\nTask created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError creating task: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// 既存のタスクを更新
        /// </summary>
        private static async Task UpdateTask()
        {
            Console.Clear();
            Console.WriteLine("=== Update Task ===");
            Console.WriteLine();

            var tasks = await _taskService.GetAllTasksAsync().ConfigureAwait(false);
            if (!tasks.Any())
            {
                Console.WriteLine("No tasks available to update.");
                Console.ReadKey();
                return;
            }

            DisplayTaskList(tasks.Take(10)); // 最初の10タスクを表示
            Console.WriteLine();
            Console.Write("Enter the task ID (first 8 characters): ");
            var taskIdInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(taskIdInput))
            {
                Console.WriteLine("Invalid input.");
                Console.ReadKey();
                return;
            }

            // 部分的なIDでタスクを検索
            var task = tasks.FirstOrDefault(t => t.Id.ToString().StartsWith(taskIdInput, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nUpdating task: {task.Title}");
            Console.WriteLine("\nSelect what to update:");
            Console.WriteLine("1. Status");
            Console.WriteLine("2. Priority");
            Console.WriteLine("3. Assigned To");
            Console.WriteLine("4. Actual Hours");
            Console.WriteLine("5. Description");
            Console.Write("Choice: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    UpdateTaskStatus(task);
                    break;
                case "2":
                    UpdateTaskPriority(task);
                    break;
                case "3":
                    Console.Write("New assignee: ");
                    task.AssignedTo = Console.ReadLine();
                    break;
                case "4":
                    Console.Write("Actual hours worked: ");
                    if (double.TryParse(Console.ReadLine(), out var hours))
                    {
                        task.ActualHours = hours;
                    }
                    break;
                case "5":
                    Console.Write("New description: ");
                    task.Description = Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                    return;
            }

            await _taskService.UpdateTaskAsync(task).ConfigureAwait(false);
            Console.WriteLine("Task updated successfully!");
            Console.ReadKey();
        }

        /// <summary>
        /// タスクステータスを更新
        /// </summary>
        /// <param name="task">更新するタスク</param>
        private static void UpdateTaskStatus(Models.Task task)
        {
            Console.WriteLine("\nSelect new status:");
            Console.WriteLine("0. Not Started");
            Console.WriteLine("1. In Progress");
            Console.WriteLine("2. Completed");
            Console.WriteLine("3. Cancelled");
            Console.WriteLine("4. On Hold");
            Console.Write("Choice (0-4): ");

            if (int.TryParse(Console.ReadLine(), out var statusChoice) && 
                statusChoice >= 0 && statusChoice <= 4)
            {
                task.Status = (TaskStatus)statusChoice;
            }
        }

        /// <summary>
        /// タスク優先度を更新
        /// </summary>
        /// <param name="task">更新するタスク</param>
        private static void UpdateTaskPriority(Models.Task task)
        {
            Console.WriteLine("\nSelect new priority:");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Normal");
            Console.WriteLine("3. High");
            Console.WriteLine("4. Critical");
            Console.Write("Choice (1-4): ");

            if (int.TryParse(Console.ReadLine(), out var priorityChoice) && 
                priorityChoice >= 1 && priorityChoice <= 4)
            {
                task.Priority = (TaskPriority)priorityChoice;
            }
        }

        /// <summary>
        /// タスクを削除
        /// </summary>
        private static async Task DeleteTask()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Task ===");
            Console.WriteLine();

            var tasks = await _taskService.GetAllTasksAsync().ConfigureAwait(false);
            if (!tasks.Any())
            {
                Console.WriteLine("No tasks available to delete.");
                Console.ReadKey();
                return;
            }

            DisplayTaskList(tasks.Take(10));
            Console.WriteLine();
            Console.Write("Enter the task ID (first 8 characters) to delete: ");
            var taskIdInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(taskIdInput))
            {
                Console.WriteLine("Invalid input.");
                Console.ReadKey();
                return;
            }

            var task = tasks.FirstOrDefault(t => t.Id.ToString().StartsWith(taskIdInput, StringComparison.OrdinalIgnoreCase));
            if (task == null)
            {
                Console.WriteLine("Task not found.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nAre you sure you want to delete: {task.Title}? (y/N)");
            var confirmation = Console.ReadLine();
            
            if (string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(confirmation, "yes", StringComparison.OrdinalIgnoreCase))
            {
                await _taskService.DeleteTaskAsync(task.Id).ConfigureAwait(false);
                Console.WriteLine("Task deleted successfully!");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }

            Console.ReadKey();
        }

        /// <summary>
        /// ステータス別にタスクを表示
        /// </summary>
        private static async Task ViewTasksByStatus()
        {
            Console.Clear();
            Console.WriteLine("=== View Tasks by Status ===");
            Console.WriteLine();
            Console.WriteLine("Select status:");
            Console.WriteLine("0. Not Started");
            Console.WriteLine("1. In Progress");
            Console.WriteLine("2. Completed");
            Console.WriteLine("3. Cancelled");
            Console.WriteLine("4. On Hold");
            Console.Write("Choice (0-4): ");

            if (int.TryParse(Console.ReadLine(), out var statusChoice) && 
                statusChoice >= 0 && statusChoice <= 4)
            {
                var status = (TaskStatus)statusChoice;
                var tasks = await _taskService.GetTasksByStatusAsync(status).ConfigureAwait(false);
                
                Console.Clear();
                Console.WriteLine($"=== Tasks with status: {status} ===");
                Console.WriteLine();
                
                if (!tasks.Any())
                {
                    Console.WriteLine("No tasks found with this status.");
                }
                else
                {
                    DisplayTaskList(tasks);
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// 優先度別にタスクを表示
        /// </summary>
        private static async Task ViewTasksByPriority()
        {
            Console.Clear();
            Console.WriteLine("=== View Tasks by Priority ===");
            Console.WriteLine();
            Console.WriteLine("Select priority:");
            Console.WriteLine("1. Low");
            Console.WriteLine("2. Normal");
            Console.WriteLine("3. High");
            Console.WriteLine("4. Critical");
            Console.Write("Choice (1-4): ");

            if (int.TryParse(Console.ReadLine(), out var priorityChoice) && 
                priorityChoice >= 1 && priorityChoice <= 4)
            {
                var priority = (TaskPriority)priorityChoice;
                var tasks = await _taskService.GetTasksByPriorityAsync(priority).ConfigureAwait(false);
                
                Console.Clear();
                Console.WriteLine($"=== Tasks with priority: {priority} ===");
                Console.WriteLine();
                
                if (!tasks.Any())
                {
                    Console.WriteLine("No tasks found with this priority.");
                }
                else
                {
                    DisplayTaskList(tasks);
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// 期限切れタスクを表示
        /// </summary>
        private static async Task ViewOverdueTasks()
        {
            Console.Clear();
            Console.WriteLine("=== Overdue Tasks ===");
            Console.WriteLine();

            var tasks = await _taskService.GetOverdueTasksAsync().ConfigureAwait(false);
            
            if (!tasks.Any())
            {
                Console.WriteLine("No overdue tasks found. Great job!");
            }
            else
            {
                Console.WriteLine("⚠️  The following tasks are overdue:");
                Console.WriteLine();
                DisplayTaskList(tasks);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// 担当者別にタスクを表示
        /// </summary>
        private static async Task ViewTasksByAssignee()
        {
            Console.Clear();
            Console.WriteLine("=== View Tasks by Assignee ===");
            Console.WriteLine();
            Console.Write("Enter assignee name: ");
            var assignee = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(assignee))
            {
                Console.WriteLine("Invalid assignee name.");
                Console.ReadKey();
                return;
            }

            try
            {
                var tasks = await _taskService.GetTasksByAssigneeAsync(assignee).ConfigureAwait(false);
                
                Console.Clear();
                Console.WriteLine($"=== Tasks assigned to: {assignee} ===");
                Console.WriteLine();
                
                if (!tasks.Any())
                {
                    Console.WriteLine("No tasks found for this assignee.");
                }
                else
                {
                    DisplayTaskList(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// タスク統計を表示
        /// </summary>
        private static async Task ViewTaskStatistics()
        {
            Console.Clear();
            Console.WriteLine("=== Task Statistics ===");
            Console.WriteLine();

            // GetTaskStatisticsメソッドにアクセスするためTaskServiceにキャスト
            if (_taskService is TaskService taskService)
            {
                var stats = taskService.GetTaskStatistics();
                
                Console.WriteLine($"Total Tasks: {stats["Total"]}");
                Console.WriteLine($"Not Started: {stats["NotStarted"]}");
                Console.WriteLine($"In Progress: {stats["InProgress"]}");
                Console.WriteLine($"Completed: {stats["Completed"]}");
                Console.WriteLine($"Cancelled: {stats["Cancelled"]}");
                Console.WriteLine($"On Hold: {stats["OnHold"]}");
                Console.WriteLine($"Overdue: {stats["Overdue"]}");
                Console.WriteLine($"High Priority: {stats["High Priority"]}");

                // 完了率を計算
                if (stats["Total"] > 0)
                {
                    var completionPercentage = (double)stats["Completed"] / stats["Total"] * 100;
                    Console.WriteLine($"Completion Rate: {completionPercentage:F1}%");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// タイトルまたは説明でタスクを検索
        /// </summary>
        private static async Task SearchTasks()
        {
            Console.Clear();
            Console.WriteLine("=== Search Tasks ===");
            Console.WriteLine();
            Console.Write("Enter search term: ");
            var searchTerm = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Invalid search term.");
                Console.ReadKey();
                return;
            }

            var allTasks = await _taskService.GetAllTasksAsync().ConfigureAwait(false);
            var matchingTasks = allTasks.Where(t => 
                (t.Title?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (t.Description?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (t.Tags?.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();

            Console.Clear();
            Console.WriteLine($"=== Search Results for: \"{searchTerm}\" ===");
            Console.WriteLine();

            if (!matchingTasks.Any())
            {
                Console.WriteLine("No matching tasks found.");
            }
            else
            {
                Console.WriteLine($"Found {matchingTasks.Count} matching task(s):");
                Console.WriteLine();
                DisplayTaskList(matchingTasks);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// フォーマットされたテーブルでタスクリストを表示
        /// </summary>
        /// <param name="tasks">表示するタスク</param>
        private static void DisplayTaskList(IEnumerable<Models.Task> tasks)
        {
            var taskList = tasks.ToList();
            if (!taskList.Any())
            {
                Console.WriteLine("No tasks to display.");
                return;
            }

            // テーブルヘッダー
            Console.WriteLine($"{"ID",-10} {"Title",-25} {"Status",-15} {"Priority",-10} {"Assignee",-15} {"Due Date",-12}");
            Console.WriteLine(new string('-', 87));

            foreach (var task in taskList)
            {
                var id = task.Id.ToString().Substring(0, 8);
                var title = task.Title.Length > 22 ? task.Title.Substring(0, 22) + "..." : task.Title;
                var status = task.GetStatusDisplay();
                var priority = task.Priority.ToString();
                var assignee = string.IsNullOrWhiteSpace(task.AssignedTo) ? "Unassigned" : task.AssignedTo;
                var dueDate = task.DueDate?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) ?? "No due date";

                // 列に収まるように長い値を切り詰める
                if (status.Length > 14) status = status.Substring(0, 14);
                if (priority.Length > 9) priority = priority.Substring(0, 9);
                if (assignee.Length > 14) assignee = assignee.Substring(0, 14);
                if (dueDate.Length > 11) dueDate = dueDate.Substring(0, 11);

                Console.WriteLine($"{id,-10} {title,-25} {status,-15} {priority,-10} {assignee,-15} {dueDate,-12}");
            }
        }
    }
}
