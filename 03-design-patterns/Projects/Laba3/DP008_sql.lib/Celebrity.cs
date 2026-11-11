using System.ComponentModel.DataAnnotations;

namespace DP008_sql.lib
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