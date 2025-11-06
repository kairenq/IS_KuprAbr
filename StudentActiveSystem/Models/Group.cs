namespace StudentActiveSystem.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public int Course { get; set; }
    }
}
