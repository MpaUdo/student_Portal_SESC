namespace StudentPortal.Models.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Fee { get; set; }
        public ICollection<StudentEnrollment> StudentEnrollments { get; set; }
    }
}
