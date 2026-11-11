using System.ComponentModel.DataAnnotations;

namespace ServiceLocator.Interfaces
{
    public class Celebrity
    {
        [Key] // Указывает, что это первичный ключ
        public int Id { get; set; }
        public string Firstname { get; set; } = "";
        public string Surname { get; set; } = "";
        public string PhotoPath { get; set; } = "";
    }
}