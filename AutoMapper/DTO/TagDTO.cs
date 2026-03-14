using System.ComponentModel.DataAnnotations;

namespace Mapper.DTO
{
    public class TagDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Can not be empty")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The length should between 1-20")]
        public string TagName { get; set; }
    }
}
