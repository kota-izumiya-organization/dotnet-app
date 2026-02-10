using System;
using System.Globalization;

namespace TaskManager.Models
{
    /// <summary>
    /// タスク管理システムにおけるタスクを表します
    /// </summary>
    public class Task : IEquatable<Task>
    {
        /// <summary>
        /// タスクの一意識別子
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// タスクのタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// タスクの詳細説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// タスクの優先度レベル
        /// </summary>
        public TaskPriority Priority { get; set; }

        /// <summary>
        /// タスクの現在のステータス
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// タスクが作成された日時
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// タスクが最後に更新された日時
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// タスクのオプションの期限日
        /// </summary>
        public DateTimeOffset? DueDate { get; set; }

        /// <summary>
        /// タスクに割り当てられた担当者
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        /// 分類用の追加タグ
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// タスク完了までの推定時間（時間単位）
        /// </summary>
        public double? EstimatedHours { get; set; }

        /// <summary>
        /// タスクに費やした実際の時間
        /// </summary>
        public double ActualHours { get; set; }

        /// <summary>
        /// Taskクラスの新しいインスタンスを初期化します
        /// </summary>
        public Task()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
            UpdatedAt = DateTimeOffset.Now;
            Status = TaskStatus.NotStarted;
            Priority = TaskPriority.Normal;
            ActualHours = 0.0;
        }

        /// <summary>
        /// タスクのタイムスタンプを更新します
        /// </summary>
        public void Touch()
        {
            UpdatedAt = DateTimeOffset.Now;
        }

        /// <summary>
        /// タスクが期限切れかどうかを確認します
        /// </summary>
        /// <returns>タスクが期限切れの場合はtrue、それ以外の場合はfalse</returns>
        public bool IsOverdue()
        {
            if (!DueDate.HasValue || Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
            {
                return false;
            }
            
            return DateTimeOffset.Now > DueDate.Value;
        }

        /// <summary>
        /// 期限日までの日数を取得します
        /// </summary>
        /// <returns>期限日までの日数、期限日が設定されていない場合はnull</returns>
        public int? DaysUntilDue()
        {
            if (!DueDate.HasValue)
            {
                return null;
            }

            var timeSpan = DueDate.Value - DateTimeOffset.Now;
            return (int)Math.Ceiling(timeSpan.TotalDays);
        }

        /// <summary>
        /// 表示用のステータス文字列を取得します
        /// </summary>
        /// <returns>フォーマット済みのステータス文字列</returns>
        public string GetStatusDisplay()
        {
            var statusText = Status.ToString();
            
            // キャメルケースの列挙値にスペースを追加
            if (Status == TaskStatus.NotStarted)
                statusText = "Not Started";
            else if (Status == TaskStatus.InProgress)
                statusText = "In Progress";
            else if (Status == TaskStatus.OnHold)
                statusText = "On Hold";

            if (IsOverdue())
            {
                statusText += " (OVERDUE)";
            }

            return statusText;
        }

        /// <summary>
        /// タスクの文字列表現を返します
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            var dueInfo = DueDate.HasValue 
                ? $" | Due: {DueDate.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}" 
                : string.Empty;
            
            return $"[{Id.ToString().Substring(0, 8)}] {Title} | {GetStatusDisplay()} | Priority: {Priority}{dueInfo}";
        }

        /// <summary>
        /// 指定されたTaskが現在のTaskと等しいかどうかを判定します
        /// </summary>
        /// <param name="other">現在のTaskと比較するTask</param>
        /// <returns>指定されたTaskが現在のTaskと等しい場合はtrue、それ以外の場合はfalse</returns>
        public bool Equals(Task other)
        {
            if (other == null)
                return false;

            return Id.Equals(other.Id);
        }

        /// <summary>
        /// 指定されたオブジェクトが現在のTaskと等しいかどうかを判定します
        /// </summary>
        /// <param name="obj">現在のTaskと比較するオブジェクト</param>
        /// <returns>指定されたオブジェクトが現在のTaskと等しい場合はtrue、それ以外の場合はfalse</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Task);
        }

        /// <summary>
        /// デフォルトのハッシュ関数として機能します
        /// </summary>
        /// <returns>現在のTaskのハッシュコード</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
