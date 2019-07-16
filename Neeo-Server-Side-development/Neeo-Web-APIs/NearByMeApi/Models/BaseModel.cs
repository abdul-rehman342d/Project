using System.ComponentModel.DataAnnotations;

namespace PowerfulPal.Neeo.NearByMeApi.Models
{
    public class BaseModel
    {
        [Required]
        [RegularExpression("^([0-9]+)(\\s)*$")]
        public string UId { get; set; }
    }
}