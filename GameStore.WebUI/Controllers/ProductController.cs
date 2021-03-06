using GameStore.Domain.Infrastructure;
using GameStore.Domain.Model;
using GameStore.WebUI.Areas.Admin.Models.DTO;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameStore.WebUI.Controllers
{    
    public class ProductController : Controller
    {
        private List<ProductDTO> GetProductsByCategory(int categoryid)
        {
            List<ProductDTO> list = new List<ProductDTO>();
            if (System.Web.HttpContext.Current.Cache["ProductList" + categoryid] != null)
            {
                list = (List<ProductDTO>)System.Web.HttpContext.Current.Cache["ProductList" + categoryid];
            }
            else
            {
                using (GameStoreDBContext context = new GameStoreDBContext())
                {
                    var query = from product in context.Products
                                where product.CategoryId == categoryid
                                join category in context.Categories
                                  on product.CategoryId equals category.CategoryId
                                select new ProductDTO { ProductId = product.ProductId, ProductName = product.ProductName, CategoryId = product.CategoryId, CategoryName = category.CategoryName, /*Price = product.Price,*/ Image = product.Image, Condition = product.Condition, /*Discount = product.Discount,*/ UserId = product.UserId, OpenUrl = product.OpenUrl, DownloadName = product.DownloadName };
                    list = query.ToList();
                    System.Web.HttpContext.Current.Cache["ProductList" + categoryid] = list;
                }
            }

            return list;
        }

        public ActionResult App()
        {
            List<ProductDTO> list = GetProductsByCategory(1);
            ViewBag.Title = "Application";
            return View("List", list);
        }

        public ActionResult Game()
        {
            List<ProductDTO> list = GetProductsByCategory(2);
            ViewBag.Title = "Game";
            return View("List", list);
        }

        public ActionResult Search(string productname)
        {
            List<ProductDTO> list = new List<ProductDTO>();

            using (GameStoreDBContext context = new GameStoreDBContext())
            {
                if (String.IsNullOrEmpty(productname))
                {
                    var query = from product in context.Products
                                join category in context.Categories
                                  on product.CategoryId equals category.CategoryId
                                select new ProductDTO { ProductId = product.ProductId, ProductName = product.ProductName, CategoryId = product.CategoryId, CategoryName = category.CategoryName, /*Price = product.Price,*/ Image = product.Image, Condition = product.Condition, /*Discount = product.Discount,*/ UserId = product.UserId, OpenUrl = product.OpenUrl, DownloadName = product.DownloadName };
                    list = query.ToList();
                }
                else
                {
                    var query = from product in context.Products
                                where product.ProductName.ToLower().Contains(productname.ToLower())
                                join category in context.Categories
                                  on product.CategoryId equals category.CategoryId
                                select new ProductDTO { ProductId = product.ProductId, ProductName = product.ProductName, CategoryId = product.CategoryId, CategoryName = category.CategoryName, /*Price = product.Price,*/ Image = product.Image, Condition = product.Condition, /*Discount = product.Discount,*/ UserId = product.UserId, OpenUrl = product.OpenUrl, DownloadName = product.DownloadName };
                    list = query.ToList();
                }
            }
            ViewBag.Title = "Search Result";
            return View("List", list);
        }

        public ActionResult Detail(int id)
        {
            ProductDTO model = null;
            using (GameStoreDBContext context = new GameStoreDBContext())
            {
                var query = from product in context.Products
                            where product.ProductId == id
                            join category in context.Categories
                              on product.CategoryId equals category.CategoryId
                            select new ProductDTO { ProductId = product.ProductId, ProductName = product.ProductName, CategoryId = product.CategoryId, CategoryName = category.CategoryName, /*Price = product.Price,*/ Image = product.Image, Condition = product.Condition, /*Discount = product.Discount,*/ UserId = product.UserId, OpenUrl = product.OpenUrl, DownloadName = product.DownloadName };
                model = query.FirstOrDefault();
            }
            return View(model);
        }

    }
}