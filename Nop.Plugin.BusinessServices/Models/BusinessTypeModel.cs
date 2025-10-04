using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.BusinessServices.Models
{
    public class BusinessTypeModel
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool Published { get; set; } = true;
    }
}
