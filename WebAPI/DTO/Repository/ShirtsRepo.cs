namespace WebAPI.DTO.Repository
{
    public static class ShirtsRepo
    {
        public static List<Shirt> Shirts = new List<Shirt>()
        {
            new Shirt() { ID = 1, Color = "Red", Size = 10, Gender = "man", Price = 2000 },
            new Shirt() { ID = 2, Color = "Blue", Size = 8, Gender = "man", Price = 3000},
            new Shirt() { ID = 3, Color = "Green", Size = 12, Gender = "man", Price = 4000}
        };

        public static bool ShirtExists(int id)
        {
            return Shirts.Any(s => s.ID == id);
        }

        public static Shirt? GetShirtByID(int id)
        {
            return Shirts.FirstOrDefault(s => s.ID == id);
        }

        public static bool SameShirtExists(string color, string size, string gender)
        {
            var result = from s in Shirts
                         where s.Color.Equals(color, StringComparison.OrdinalIgnoreCase)
                         && s.Size.ToString().Equals(size, StringComparison.OrdinalIgnoreCase)
                         && s.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)
                         select s;

            return result.Any();
        }

        public static void AddNewShirt(Shirt shirt)
        {
            int newID = Shirts.Max(s => s.ID);
            shirt.ID = newID + 1;

            Shirts.Add(shirt);
        }

        public static void UpdateShirt(Shirt shirt)
        {
            var existingShirt = GetShirtByID(shirt.ID);

            existingShirt.Color = shirt.Color;
            existingShirt.Size = shirt.Size;
            existingShirt.Gender = shirt.Gender;
            existingShirt.Price = shirt.Price;

        }

        public static void RemoveShirt(int id)
        {
            var shirt = GetShirtByID(id);
            Shirts.Remove(shirt);
        }
    }
}
