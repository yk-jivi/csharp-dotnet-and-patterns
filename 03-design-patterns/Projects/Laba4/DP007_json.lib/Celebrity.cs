namespace DP007_json.lib
{
    public class Celebrity
    {
        public int Id { get; set; } //уникальный идентификатор знаменитости
        public string Firstname { get; set; } = ""; //имя знаменитости
        public string Surname { get; set; } = ""; //фамилия знаменитости
        public string PhotoPath { get; set; } = ""; //путь к jpg-файлу фотографии

        public Celebrity() { }

        public Celebrity(int id, string firstname, string surname, string photopath)
        {
            Id = id;
            Firstname = firstname;
            Surname = surname;
            PhotoPath = photopath;
        }
    }
}