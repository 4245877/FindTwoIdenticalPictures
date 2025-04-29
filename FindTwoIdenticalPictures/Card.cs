namespace FindTwoIdenticalPictures
{
    public class Card
    {
        public System.Drawing.Image Image { get; set; }
        public bool IsFlipped { get; set; } = false; // По умолчанию карта не перевернута
        public int Id { get; set; }
    }
}