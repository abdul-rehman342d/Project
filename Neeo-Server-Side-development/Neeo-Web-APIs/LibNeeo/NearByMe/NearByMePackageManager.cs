using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.NearByMe
{
    public class NearByMePackageManager
    {
        private DbManager _dbManager = new DbManager();
        public async Task<bool> InsertNearByMePacakage(NearByMePromotionPackage pacakage)
        {
            return await System.Threading.Tasks.Task.Run(() => _dbManager.InsertNearByMePromotionPackage(pacakage.locationId, pacakage.description,pacakage.price,pacakage.enabled));
        }
        public async Task<bool> UpsertNearByMePacakage(NearByMePromotionPackage pacakage)
        {
            return await System.Threading.Tasks.Task.Run(() => _dbManager.UpsertNearByMePromotionPackage(pacakage.locationId, pacakage.description, pacakage.price, pacakage.enabled,pacakage.packageId));
        }
        public async Task<List<NearByMePromotionPackage>> GetNearByMePromotionPackages()
        {
           List<NearByMePromotionPackage> pacakages = new List<NearByMePromotionPackage>(); 
            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.GetNearByMePromotionPackages());
            
            pacakages = (from row in dtNearbyMePromotion.AsEnumerable()
                          select new NearByMePromotionPackage()
                          {
                              packageId = Convert.ToInt32(row["packageId"]),
                              locationId = Convert.ToInt32(row["locationId"]),
                              description = Convert.ToString(row["description"]),
                              price=Convert.ToDecimal(row["price"]),
                              createdDate= Convert.ToDateTime(row["createdDate"]),
                              updatedDate = Convert.ToDateTime(row["updatedDate"]),
                              enabled = Convert.ToBoolean(row["enabled"]),
                              isDeleted = Convert.ToBoolean(row["isDeleted"]),
                          }).ToList();
            return pacakages;
        }
        public async Task<List<NearByMePromotionPackage>> GetNearByMePromotionPackagesById(int packageId)
        {
            List<NearByMePromotionPackage> pacakages = new List<NearByMePromotionPackage>();
            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.GetNearByMePromotionPackagesById(packageId));

            pacakages = (from row in dtNearbyMePromotion.AsEnumerable()
                         select new NearByMePromotionPackage()
                         {
                             packageId = Convert.ToInt32(row["packageId"]),
                             locationId = Convert.ToInt32(row["locationId"]),
                             description = Convert.ToString(row["description"]),
                             price = Convert.ToDecimal(row["price"]),
                             createdDate = Convert.ToDateTime(row["createdDate"]),
                             updatedDate = Convert.ToDateTime(row["updatedDate"]),
                             enabled = Convert.ToBoolean(row["enabled"]),
                             isDeleted = Convert.ToBoolean(row["isDeleted"]),
                         }).ToList();

            return pacakages;
        }
        public async Task<List<NearByMePromotionPackage>> GetNearByMePromotionPackagesByCountry(int countryId)
        {
            List <NearByMePromotionPackage> pacakages = new List<NearByMePromotionPackage>();
            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.GetNearByMePromotionPackagesByCountry(countryId));

            pacakages = (from row in dtNearbyMePromotion.AsEnumerable()
                         select new NearByMePromotionPackage()
                         {
                             packageId = Convert.ToInt32(row["packageId"]),
                             locationId = Convert.ToInt32(row["locationId"]),
                             description = Convert.ToString(row["description"]),
                             price = Convert.ToDecimal(row["price"]),
                             createdDate = Convert.ToDateTime(row["createdDate"]),
                             updatedDate = Convert.ToDateTime(row["updatedDate"]),
                             enabled = Convert.ToBoolean(row["enabled"]),
                             isDeleted = Convert.ToBoolean(row["isDeleted"]),
                         }).ToList();

            return pacakages;
        }
    }
}
