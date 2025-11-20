namespace HrMangmentSystem_Domain.Common
{
    public  class BaseEntity<T> 
    {

        public T Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }



    }
}
