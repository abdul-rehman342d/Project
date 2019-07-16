using DAL;
using LibNeeo.NearByMe.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using File = System.Net.WebRequestMethods.File;

namespace LibNeeo.NearByMe
{
    public class NearByMePromotionManager
    {
        private DbManager _dbManager = new DbManager();
        private static NearByMeManager _instance = new NearByMeManager();
        //private class FileDetails
        //{
        //    public string Name { get; set; }
        //    public string Path { get; set; }
        //}
        //public async Task<bool> InsertNearByMePromotion(NearByMePromotion promotion,string rootPath)
        //{
        //    List<FileDetails> images=new List<FileDetails>();
        //    for (int i=0;i< promotion.FilesViewModel.Count;i++)
        //    {
        //        string fileName = promotion.FilesViewModel[i].GetFilename();
        //        string path = Path.Combine(
        //                rootPath, fileName
        //                );
        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            promotion.FilesViewModel[i].CopyToAsync(stream);
        //        }
        //        FileDetails currentImage=new FileDetails();
        //        currentImage.Name = fileName;
        //        currentImage.Path = path;
        //        images.Add(currentImage);

        //    }
        //    for (int i = 0; i < promotion.ImagesXml.Count; i++)
        //    {
        //        for (int j = 0; j < images.Count; j++)
        //        {
        //            if (promotion.ImagesXml[i].imagePath == images[j].Name)
        //            {
        //                promotion.ImagesXml[i].imagePath = images[j].Path;
        //            }
        //        }
        //    }


        //    StringBuilder ImagesXml = new StringBuilder("<ImageList>");
        //    foreach (NearByMePromotionImage item in promotion.ImagesXml)
        //    {
        //        ImagesXml.Append("<Image>" +
        //                "<ImageCaption>" + item.imageCaption + "</ImageCaption>" +
        //                "<ImagePath>" + item.imagePath + "</ImagePath>" +
        //                "<FeaturedImage>" + item.featuredImage + "</FeaturedImage>" +
        //             "</Image>");

        //    }
        //    ImagesXml.Append("</ImageList>");
        //    StringBuilder PromotionPackagesXml = new StringBuilder("<PromotionPackages>");
        //    foreach (NearByMeUserPromotionsPackage item in promotion.PromotionPackagesXml)
        //    {
        //        PromotionPackagesXml.Append(
        //            "<PromotionPackage>" +
        //                "<PackageId>" + item.packageId + "</PackageId>" +
        //                "<promotionId>" + item.promotionId + "</promotionId>" +
        //                "<runUntill>" + item.runUntill + "</runUntill>" +
        //             "</PromotionPackage>");
        //    }
        //    PromotionPackagesXml.Append("</PromotionPackages>");
        //    return await System.Threading.Tasks.Task.Run(() => _dbManager.InsertNearByMePromotion(promotion.username, promotion.description, promotion.status,promotion.audienceMaxAge, promotion.audienceMinAge, promotion.locations, promotion.audienceGender, promotion.audienceInterests,ImagesXml.ToString(), PromotionPackagesXml.ToString()));
        //}
        //public async Task<bool> UpsertNearByMePromotion(NearByMePromotion promotion, string rootPath)
        //{
        //    List<FileDetails> imagesName = new List<FileDetails>();
        //    for (int i = 0; i < promotion.FilesViewModel.Count; i++)
        //    {
        //        string fileName = promotion.FilesViewModel[i].GetFilename();
        //        string path = Path.Combine(
        //                rootPath, fileName
        //                );
        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            promotion.FilesViewModel[i].CopyToAsync(stream);
        //        }
        //        FileDetails currentImage = new FileDetails();
        //        currentImage.Name = fileName;
        //        currentImage.Path = path;
        //        imagesName.Add(currentImage);

        //    }
        //    for (int i = 0; i < promotion.ImagesXml.Count; i++)
        //    {
        //        for (int j = 0; j < imagesName.Count; j++)
        //        {
        //            if (promotion.ImagesXml[i].imagePath == imagesName[j].Name)
        //            {
        //                promotion.ImagesXml[i].imagePath = imagesName[j].Path;
        //            }
        //        }
        //    }
        //    string ImagesXml = "<ImageList>";
        //    string PromotionPackagesXml = "<PromotionPackages>";
        //    foreach (NearByMePromotionImage item in promotion.ImagesXml)
        //    {
        //        ImagesXml = ImagesXml +
        //            "<Image>" +
        //                "<ImageCaption>" + item.imageCaption + "</ImageCaption>" +
        //                "<ImagePath>" + item.imagePath + "</ImagePath>" +
        //                "<FeaturedImage>" + item.featuredImage + "</FeaturedImage>" +
        //             "</Image>";
        //    }
        //    ImagesXml = ImagesXml + "</ImageList>";
        //    foreach (NearByMeUserPromotionsPackage item in promotion.PromotionPackagesXml)
        //    {
        //        PromotionPackagesXml = PromotionPackagesXml +
        //            "<PromotionPackage>" +
        //                "<PackageId>" + item.packageId + "</PackageId>" +
        //                "<promotionId>" + item.promotionId + "</promotionId>" +
        //                "<runUntill>" + item.runUntill + "</runUntill>" +
        //             "</PromotionPackage>";
        //    }
        //    PromotionPackagesXml = PromotionPackagesXml + "</PromotionPackages>";
        //    return await System.Threading.Tasks.Task.Run(() => _dbManager.UpsertNearByMePromotion(promotion.promotionId,promotion.username, promotion.description, promotion.status, promotion.audienceMaxAge, promotion.audienceMinAge, promotion.locations, promotion.audienceGender, promotion.audienceInterests, ImagesXml, PromotionPackagesXml));
        //}


        //public async Task<Object> InsertNearByMePromotionImage(List<IFormFile> FilesViewModel, string rootPath)
        //{
        //    List<FileDetails> images = new List<FileDetails>();
        //    for (int i = 0; i < FilesViewModel.Count; i++)
        //    {
        //        string fileName = FilesViewModel[i].FileName;
        //        string newfileName = FilesViewModel[i].GetFilename();
        //        string path = Path.Combine(
        //                rootPath, newfileName
        //                );
        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            FilesViewModel[i].CopyToAsync(stream);
        //        }
        //        FileDetails currentImage = new FileDetails();
        //        currentImage.Name = fileName;
        //        currentImage.Path = path;
        //        images.Add(currentImage);
        //    }
        //    //StringBuilder ImagesXml = new StringBuilder("<ImageList>");
        //    //foreach (FileDetails item in images)
        //    //{
        //    //    ImagesXml.Append("<Image>" +
        //    //            "<ImageName>" + item.Name + "</ImageName>" +
        //    //            "<ImagePath>" + item.Path + "</ImagePath>" +
        //    //         "</Image>");

        //    //}
        //    //ImagesXml.Append("</ImageList>");
        //    //await System.Threading.Tasks.Task.Run(() => _dbManager.InsertNearByMePromotionImage(ImagesXml.ToString()));
        //    return await System.Threading.Tasks.Task.Run(() => images);
        //}



        public async Task<bool> InsertNearByMePromotion(NearByMePromotion promotion)
        {
            StringBuilder ImagesXml = new StringBuilder("<ImageList>");
            foreach (NearByMePromotionImage item in promotion.ImagesXml)
            {
                ImagesXml.Append("<Image>" +
                        "<ImageCaption>" + item.imageCaption + "</ImageCaption>" +
                        "<ImagePath>" + item.imagePath + "</ImagePath>" +
                        "<FeaturedImage>" + item.featuredImage + "</FeaturedImage>" +
                     "</Image>");

            }
            ImagesXml.Append("</ImageList>");

            StringBuilder PromotionPackagesXml = new StringBuilder("<PromotionPackages>");
            foreach (NearByMeUserPromotionsPackage item in promotion.PromotionPackagesXml)
            {
                PromotionPackagesXml.Append(
                    "<PromotionPackage>" +
                        "<PackageId>" + item.packageId + "</PackageId>" +
                        "<promotionId>" + item.promotionId + "</promotionId>" +
                        "<runUntill>" + item.runUntill + "</runUntill>" +
                     "</PromotionPackage>");
            }
            PromotionPackagesXml.Append("</PromotionPackages>");

            return await System.Threading.Tasks.Task.Run(() => _dbManager.InsertNearByMePromotion(promotion.username, promotion.description, promotion.status, promotion.audienceMaxAge, promotion.audienceMinAge, promotion.locations, promotion.audienceGender, promotion.audienceInterests, ImagesXml.ToString(), PromotionPackagesXml.ToString()));
        }
        public async Task<bool> UpsertNearByMePromotion(NearByMePromotion promotion)
        {
            StringBuilder ImagesXml = new StringBuilder("<ImageList>");
            foreach (NearByMePromotionImage item in promotion.ImagesXml)
            {
                ImagesXml.Append("<Image>" +
                        "<ImageCaption>" + item.imageCaption + "</ImageCaption>" +
                        "<ImagePath>" + item.imagePath + "</ImagePath>" +
                        "<FeaturedImage>" + item.featuredImage + "</FeaturedImage>" +
                     "</Image>");

            }
            ImagesXml.Append("</ImageList>");

            StringBuilder PromotionPackagesXml = new StringBuilder("<PromotionPackages>");
            foreach (NearByMeUserPromotionsPackage item in promotion.PromotionPackagesXml)
            {
                PromotionPackagesXml.Append(
                    "<PromotionPackage>" +
                        "<PackageId>" + item.packageId + "</PackageId>" +
                        "<promotionId>" + item.promotionId + "</promotionId>" +
                        "<runUntill>" + item.runUntill + "</runUntill>" +
                     "</PromotionPackage>");
            }
            PromotionPackagesXml.Append("</PromotionPackages>");

            return await System.Threading.Tasks.Task.Run(() => _dbManager.UpsertNearByMePromotion(promotion.promotionId, promotion.username, promotion.description, promotion.status, promotion.audienceMaxAge, promotion.audienceMinAge, promotion.locations, promotion.audienceGender, promotion.audienceInterests, ImagesXml.ToString(), PromotionPackagesXml.ToString()));
        }

        public async Task<bool> UpsertNearByMePromotionStatus(int promotionId, Byte status)
        {
            return await System.Threading.Tasks.Task.Run(() => _dbManager.UpsertNearByMePromotionStatus(promotionId, status));
        }
        public async Task<Object> GetNearByMePromotionById(int promotionId)
        {
            DataSet result = await System.Threading.Tasks.Task.Run(() => _dbManager.GetNearByMePromotionByID(promotionId));
            var promotion = result.Tables[0];
            var images = result.Tables[1];
            DataTable packages = result.Tables[2];
            var promotions = (from row in promotion.AsEnumerable()
                          select new
                          {
                              promotionId= Convert.ToInt32(row["promotionId"]),
                              username = Convert.ToString(row["username"]),
                              description = Convert.ToString(row["description"]),
                              status = (row["status"] != DBNull.Value) ? Convert.ToByte(row["status"]) : (Byte?)null,
                              audienceMaxAge = (row["audienceMaxAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMaxAge"]) : (Byte?)null,
                              audienceMinAge = (row["audienceMinAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMinAge"]) : (Byte?)null,
                              audienceGender = (row["audienceGender"] != DBNull.Value) ? Convert.ToByte(row["audienceGender"]) : (Byte?)null,
                              audienceInterests = Convert.ToString(row["audienceInterests"]),
                              createdDate= Convert.ToDateTime(row["createdDate"]),
                              updatedDate = Convert.ToDateTime(row["updatedDate"]),
                              Images= (from row1 in images.AsEnumerable()
                                          select new
                                          {
                                              imageId= Convert.ToInt64(row1["imageId"]),
                                              imageCaption = Convert.ToString(row1["imageCaption"]),
                                              imagePath = Convert.ToString(row1["imagePath"]),
                                              promotionId = Convert.ToInt32(row1["promotionId"]),
                                              featuredImage =(row1["featuredImage"] != DBNull.Value) ? Convert.ToBoolean(row1["featuredImage"]) : (Boolean?)null,
                                              createdDate = Convert.ToDateTime(row1["createdDate"]),
                                              updatedDate = Convert.ToDateTime(row1["updatedDate"]),
                                          }).ToList(),


                              packages = (from row2 in packages.AsEnumerable()
                                           select new
                                           {
                                               packageId = Convert.ToInt32(row2["packageId"]),
                                               locationId = Convert.ToInt32(row2["locationId"]),
                                               description = Convert.ToString(row2["description"]),
                                               runUntill = Convert.ToDateTime(row2["runUntill"])
                                           }).ToList()


                          }).FirstOrDefault();

            return promotions;
        }
        public async Task<Object> GetPersonalNearByMePromotionByUserName(string username, string advertiser)
        {
            DataTable result = await System.Threading.Tasks.Task.Run(() => _dbManager.GetPersonalNearByMePromotionByUserName(username,advertiser));
            var promotions = (from row in result.AsEnumerable()
                              select new
                              {
                                  promotionId = Convert.ToInt32(row["promotionId"]),
                                  username = Convert.ToString(row["username"]),
                                  description = Convert.ToString(row["description"]),
                                  status = (row["status"] != DBNull.Value) ? Convert.ToByte(row["status"]) : (Byte?)null,
                                  audienceMaxAge = (row["audienceMaxAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMaxAge"]) : (Byte?)null,
                                  audienceMinAge = (row["audienceMinAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMinAge"]) : (Byte?)null,
                                  audienceGender = (row["audienceGender"] != DBNull.Value) ? Convert.ToByte(row["audienceGender"]) : (Byte?)null,
                                  audienceInterests = Convert.ToString(row["audienceInterests"]),
                                  imageCaption = Convert.ToString(row["imageCaption"]),
                                  imagePath = Convert.ToString(row["imagePath"]),
                                  featuredImage = (row["featuredImage"] != DBNull.Value) ? Convert.ToBoolean(row["featuredImage"]) : (Boolean?)null,
                                  
                              }).ToList();

            return promotions;
        }
        public async Task<Object> GetAdvertisedNearByMePromotionByUserName(string username)
        {
            DataTable result = await System.Threading.Tasks.Task.Run(() => _dbManager.GetAdvertisedNearByMePromotionByUserName(username));
            var promotions = (from row in result.AsEnumerable()
                              select new
                              {
                                  promotionId = Convert.ToInt32(row["promotionId"]),
                                  username = Convert.ToString(row["username"]),
                                  description = Convert.ToString(row["description"]),
                                  status = (row["status"] != DBNull.Value) ? Convert.ToByte(row["status"]) : (Byte?)null,
                                  audienceMaxAge = (row["audienceMaxAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMaxAge"]) : (Byte?)null,
                                  audienceMinAge = (row["audienceMinAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMinAge"]) : (Byte?)null,
                                  audienceGender = (row["audienceGender"] != DBNull.Value) ? Convert.ToByte(row["audienceGender"]) : (Byte?)null,
                                  audienceInterests = Convert.ToString(row["audienceInterests"]),
                                  imageCaption = Convert.ToString(row["imageCaption"]),
                                  imagePath = Convert.ToString(row["imagePath"]),
                                  featuredImage = (row["featuredImage"] != DBNull.Value) ? Convert.ToBoolean(row["featuredImage"]) : (Boolean?)null,

                              }).ToList();
            return promotions;
        }
        public async Task<Object> GetTopNearByMePromotionByUserName(string username,int top, string advertiser)
        {
            DataTable result = await System.Threading.Tasks.Task.Run(() => _dbManager.GetTopNearByMePromotionByUserName(username,top,advertiser));
            var promotions = (from row in result.AsEnumerable()
                              select new
                              {
                                  promotionId = Convert.ToInt32(row["promotionId"]),
                                  username = Convert.ToString(row["username"]),
                                  description = Convert.ToString(row["description"]),
                                  status = (row["status"] != DBNull.Value) ? Convert.ToByte(row["status"]) : (Byte?)null,
                                  audienceMaxAge = (row["audienceMaxAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMaxAge"]) : (Byte?)null,
                                  audienceMinAge = (row["audienceMinAge"] != DBNull.Value) ? Convert.ToByte(row["audienceMinAge"]) : (Byte?)null,
                                  audienceGender = (row["audienceGender"] != DBNull.Value) ? Convert.ToByte(row["audienceGender"]) : (Byte?)null,
                                  audienceInterests = Convert.ToString(row["audienceInterests"]),
                                  imageCaption = Convert.ToString(row["imageCaption"]),
                                  imagePath = Convert.ToString(row["imagePath"]),
                                  featuredImage = (row["featuredImage"] != DBNull.Value) ? Convert.ToBoolean(row["featuredImage"]) : (Boolean?)null,

                              }).ToList();
            return promotions;
        }
        public async  Task<Object> GetAllAdvertisedAccounts()
        {

            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.GetAllAdvertisedAccounts());
            var promotions = (from row in dtNearbyMePromotion.AsEnumerable()
                              select new
                              {

                                  username = Convert.ToString(row["username"]),
                                  country = (row["country"] != DBNull.Value) ? Convert.ToInt32(row["country"]) : (int?)null,
                                  countryName = Convert.ToString(row["countryName"]),
                              }).ToList();
            return promotions;

        }
        public async Task<Object> ProcGetTopFiveAdvertisedAccounts()
        {

            DataTable dtNearbyMePromotion = await System.Threading.Tasks.Task.Run(() => _dbManager.ProcGetTopFiveAdvertisedAccounts());
            var promotions = (from row in dtNearbyMePromotion.AsEnumerable()
                              select new
                              {

                                  username = Convert.ToString(row["username"]),
                                  country = (row["country"] != DBNull.Value) ? Convert.ToInt32(row["country"]) : (int?)null,
                                  countryName = Convert.ToString(row["countryName"]),
                              }).ToList();
            return promotions;

        }
        public async Task<Object> GetAdvertisedAccountsByCountry(int countryId)
        {
            DataTable result = await System.Threading.Tasks.Task.Run(() => _dbManager.GetAdvertisedAccountsByCountry(countryId));
            var promotions = (from row in result.AsEnumerable()
                              select new
                              {

                                  username = Convert.ToString(row["username"]),
                                  countryID = (row["countryID"] != DBNull.Value) ? Convert.ToInt32(row["countryID"]) : (int?)null,
                                  countryName = Convert.ToString(row["countryName"]),
                              }).ToList();
            return promotions;
        }

    }
}
