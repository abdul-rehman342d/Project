using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LibNeeo.NearByMe
{
    public class NearByMeCountriesManager
    {
        private DbManager _dbManager = new DbManager();
        public async Task<List<Country>> GetAllCountries()
        {
            List<Country> countries = new List<Country>();
            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.GetAllCountries());

            countries = (from row in dtNearbyMePromotion.AsEnumerable()
                         select new Country()
                         {
                             countryId = Convert.ToInt32(row["countryId"]),
                             countryName = Convert.ToString(row["countryName"])
                         }).ToList();

            return countries;
        }

        public async Task<Country> GetCountryByCode(string countryCode,string prePath)
        {
            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.GetCountryByCode(countryCode));
            Country country = (from row in dtNearbyMePromotion.AsEnumerable()
                         select new Country()
                         {

                             countryId = Convert.ToInt32(row["countryId"]),
                             countryName = Convert.ToString(row["countryName"]),
                             countryCode = Convert.ToString(row["countryCode"]),
                             flagImageURL = prePath + Convert.ToString(row["flagImageURL"])
                         }).FirstOrDefault();

            return country;
        }
    }
}
