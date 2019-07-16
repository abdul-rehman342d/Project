using System.ComponentModel.DataAnnotations;

namespace PowerfulPal.Neeo.NearByMeApi.Models
{
    public class UserSearchModel : BaseModel
    {
        [Required]
        public string SearchText { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public bool IsCurrentLocation { get; set; }
        //public int PageNo { get; set; } = 1;
        //public int PageSize { get; set; } = 25;
    }
}