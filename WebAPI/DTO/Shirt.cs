using System.ComponentModel.DataAnnotations;
using WebAPI.Validations;


namespace WebAPI.DTO
{
    public class Shirt
    {
        public int ID { get; set; }

        [Required]
        public string? Color { get; set; }

        [Required]
        [ShirtSizeValidatorAtribute]
        public int Size { get; set; }

        [Required]
        public string? Gender { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
