namespace StudentPortal.Models.Entities
{
    public class Student : BaseEntity
    {
        public string StudentId { get; set; }

        public string? Firstname { get; set; }

        public string? Surname { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }

        public ICollection<StudentEnrollment> StudentEnrollments { get; set; }

    }
}
