namespace HrMangmentSystem_Domain.Common
{
    public abstract class BaseEntity
    {

        public string Id { get; set; } = NewShortId();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

       private static string NewShortId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "")
                .Substring(0, 10);
        }

    }
}
