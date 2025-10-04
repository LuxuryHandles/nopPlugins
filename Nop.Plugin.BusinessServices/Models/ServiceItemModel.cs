using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.BusinessServices.Models
{
    public class ServiceItemModel
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool Published { get; set; } = true;
    }
}
