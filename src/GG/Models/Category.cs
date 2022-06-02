namespace GG.Models
{
    public abstract class Category //En superklass för kategorier. Just nu finns det bara Decade som använder den, men är kvar för ev. utveckling
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}