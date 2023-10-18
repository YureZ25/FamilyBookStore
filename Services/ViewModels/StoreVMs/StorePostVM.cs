using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.StoreVMs
{
    public class StorePostVM
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(1024)]
        public string Address { get; set; }
    }
}
