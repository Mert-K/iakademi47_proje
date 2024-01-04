﻿using iakademi47_proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace iakademi47_proje.ViewComponents
{
    public class Menus : ViewComponent
    {
        iakademi47Context context = new iakademi47Context();

        public IViewComponentResult Invoke()
        {
            List<Category> categories = context.Categories.ToList();
            return View(categories);
        }
    }
}
