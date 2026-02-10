using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Models;
using System.Globalization;

namespace TaskManager.Services
{
    /// <summary>
    /// XMLを使用してファイルストレージ操作を処理するサービス
    /// </summary>
    public class FileStorageService : IDisposable
    {
        private readonly string _filePath;
        private bool _disposed = false;

        /// <summary>
        /// FileStorageServiceの新しいインスタンスを初期化します
        /// </summary>
        public FileStorageService()
        {
            _filePath = ConfigurationManager.AppSettings["TaskStorageFile"] ?? "tasks.xml";
        }

        /// <summary>
        /// タスクをXMLファイルに保存します
        /// </summary>
        /// <param name="tasks">保存するタスクのコレクション</param>
        /// <returns>非同期操作を表すタスク</returns>
        public async Task<bool> SaveTasksAsync(IEnumerable<Models.Task> tasks)
        {
            try
            {
                var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("Tasks",
                        tasks.Select(task => CreateTaskElement(task))
                    )
                );

                // デッドロックを避けるためにConfigureAwait(false)を使用
                await Task.Run(() => doc.Save(_filePath)).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                // 実際のアプリケーションでは、ログフレームワークを使用します
                Console.WriteLine($"Error saving tasks: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// XMLファイルからタスクを読み込みます
        /// </summary>
        /// <returns>読み込まれたタスクのコレクション</returns>
        public async Task<IEnumerable<Models.Task>> LoadTasksAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Enumerable.Empty<Models.Task>();
                }

                // デッドロックを避けるためにConfigureAwait(false)を使用
                var doc = await Task.Run(() => XDocument.Load(_filePath)).ConfigureAwait(false);
                
                var tasks = doc.Root.Elements("Task")
                    .Select(ParseTaskElement)
                    .Where(task => task != null)
                    .ToList();

                return tasks;
            }
            catch (Exception ex)
            {
                // 実際のアプリケーションでは、ログフレームワークを使用します
                Console.WriteLine($"Error loading tasks: {ex.Message}");
                return Enumerable.Empty<Models.Task>();
            }
        }

        /// <summary>
        /// タスクからXML要素を作成します
        /// </summary>
        /// <param name="task">変換するタスク</param>
        /// <returns>タスクを表すXElement</returns>
        private XElement CreateTaskElement(Models.Task task)
        {
            return new XElement("Task",
                new XElement("Id", task.Id),
                new XElement("Title", task.Title ?? string.Empty),
                new XElement("Description", task.Description ?? string.Empty),
                new XElement("Priority", (int)task.Priority),
                new XElement("Status", (int)task.Status),
                new XElement("CreatedAt", task.CreatedAt.ToString("O", CultureInfo.InvariantCulture)),
                new XElement("UpdatedAt", task.UpdatedAt.ToString("O", CultureInfo.InvariantCulture)),
                new XElement("DueDate", task.DueDate?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty),
                new XElement("AssignedTo", task.AssignedTo ?? string.Empty),
                new XElement("Tags", task.Tags ?? string.Empty),
                new XElement("EstimatedHours", task.EstimatedHours?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                new XElement("ActualHours", task.ActualHours.ToString(CultureInfo.InvariantCulture))
            );
        }

        /// <summary>
        /// XML要素を解析してタスクを作成します
        /// </summary>
        /// <param name="element">解析するXML要素</param>
        /// <returns>Taskオブジェクト、解析に失敗した場合はnull</returns>
        private Models.Task ParseTaskElement(XElement element)
        {
            try
            {
                var task = new Models.Task();

                // IDを解析
                if (Guid.TryParse(element.Element("Id")?.Value, out var id))
                {
                    task.Id = id;
                }

                // 基本プロパティを解析
                task.Title = element.Element("Title")?.Value ?? string.Empty;
                task.Description = element.Element("Description")?.Value ?? string.Empty;

                // 列挙型を解析
                if (int.TryParse(element.Element("Priority")?.Value, out var priority))
                {
                    task.Priority = (TaskPriority)priority;
                }

                if (int.TryParse(element.Element("Status")?.Value, out var status))
                {
                    task.Status = (TaskStatus)status;
                }

                // 適切なタイムゾーン処理のためにDateTimeOffsetを使用して日付を解析
                if (DateTimeOffset.TryParse(element.Element("CreatedAt")?.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var createdAt))
                {
                    task.CreatedAt = createdAt;
                }

                if (DateTimeOffset.TryParse(element.Element("UpdatedAt")?.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var updatedAt))
                {
                    task.UpdatedAt = updatedAt;
                }

                var dueDateValue = element.Element("DueDate")?.Value;
                if (!string.IsNullOrEmpty(dueDateValue) && 
                    DateTimeOffset.TryParse(dueDateValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dueDate))
                {
                    task.DueDate = dueDate;
                }

                // その他のプロパティを解析
                task.AssignedTo = element.Element("AssignedTo")?.Value ?? string.Empty;
                task.Tags = element.Element("Tags")?.Value ?? string.Empty;

                var estimatedHoursValue = element.Element("EstimatedHours")?.Value;
                if (!string.IsNullOrEmpty(estimatedHoursValue) && 
                    double.TryParse(estimatedHoursValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var estimatedHours))
                {
                    task.EstimatedHours = estimatedHours;
                }

                if (double.TryParse(element.Element("ActualHours")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var actualHours))
                {
                    task.ActualHours = actualHours;
                }

                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing task element: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// FileStorageServiceを破棄します
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
                    // マネージリソースがあればここで破棄します
                }
                _disposed = true;
            }
        }
    }
}
