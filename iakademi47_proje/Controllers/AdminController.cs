using Microsoft.AspNetCore.Mvc;
using iakademi47_proje.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace iakademi47_proje.Controllers
{
	public class AdminController : Controller
	{
		Cls_User u = new Cls_User();
		Cls_Category c = new Cls_Category();
		Cls_Supplier s = new Cls_Supplier();
		Cls_Status st = new Cls_Status();
		Cls_Product p = new Cls_Product();
		iakademi47Context context = new iakademi47Context();


		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login([Bind("Email,Password,NameSurname")] User user)
		{
			if (ModelState.IsValid)
			{
				User? usr = await u.loginControl(user);
				if (usr != null)
				{
					return RedirectToAction("Index");
				}
			}
			return View();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> CategoryIndex()
		{
			List<Category> categories = await c.CategorySelect();
			return View(categories);
		}


		[HttpGet]
		public IActionResult CategoryCreate()
		{
			CategoryFill();
			return View();
		}

		void CategoryFill()
		{
			List<Category> categories = c.CategorySelectMain();
			ViewData["categoryList"] = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });
		}

		[HttpPost]
		public IActionResult CategoryCreate(Category category)
		{
			string answer = Cls_Category.CategoryInsert(category);

			if (answer == "başarılı")
			{
				TempData["Message"] = category.CategoryName + " Kategori Eklendi.";
			}
			else if (answer == "zaten var")
			{
				TempData["Message"] = "Bu kategori daha önceden eklenmiş.";
			}
			else
			{
				TempData["Message"] = "HATA!!!!! Kategori Eklenemedi.";

			}
			return RedirectToAction("CategoryCreate"); //[HttpGet]
													   //return View(); [HttpPost]
		}


		public async Task<IActionResult> CategoryEdit(int? id)
		{
			CategoryFill();
			if (id == null || context.Categories == null)
			{
				return NotFound();
			}

			var category = await c.CategoryDetails(id);
			return View(category);
		}

		[HttpPost]
		public IActionResult CategoryEdit(Category category)
		{
			bool answer = Cls_Category.CategoryUpdate(category);

			if (answer)
			{
				TempData["Message"] = category.CategoryName + " Kategorisi Güncellendi.";
				return RedirectToAction("CategoryIndex");
			}
			else
			{
				TempData["Message"] = "HATA!!!!! Kategori Güncellenemedi.";
				return RedirectToAction(nameof(CategoryEdit));
			}
			return View();
		}

		public static int global_categoryid = 0;
		public async Task<IActionResult> CategoryDetails(int id)
		{
			if (id != 0)
			{
				global_categoryid = id;
			}

			if (id == 0)
			{
				id = global_categoryid;
			}
			var category = await c.CategoryDetails(id);

			//menüden tekrar tıklanamaz ise çözüm bu
			ViewBag.categoryname = category?.CategoryName;

			if (category.ParentID > 0)
			{
				ViewBag.categoryname2 = context.Categories.FirstOrDefault(c => c.CategoryID == category.ParentID).CategoryName;
			}
			else
			{
				ViewBag.categoryname2 = "Ana Kategori";
			}


			return View(category);

		}

		[HttpGet]
		public async Task<IActionResult> CategoryDelete(int? id)
		{
			if (id == 0 || context.Categories == null)
			{
				return NotFound();
			}

			var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);

			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}


		[HttpPost, ActionName("CategoryDelete")]
		public async Task<IActionResult> CategoryDeleteConfirmed(int id)
		{
			bool result = Cls_Category.CategoryDelete(id);
			if (result)
			{
				TempData["Message"] = "Silindi";
				return RedirectToAction("CategoryIndex");
			}
			else
			{
				TempData["Message"] = "HATA Silinemedi";
				return RedirectToAction(nameof(CategoryDelete));
			}
		}


		/*******************************************************/


		public async Task<IActionResult> SupplierIndex()
		{
			List<Supplier> suppliers = await s.SupplierSelect();
			return View(suppliers);
		}

		[HttpGet]
		public IActionResult SupplierCreate()
		{
			return View();
		}

		async void SupplierFill()
		{
			List<Supplier> suppliers = await s.SupplierSelect();
			ViewData["supplierList"] = suppliers.Select(c => new SelectListItem { Text = c.BrandName, Value = c.SupplierID.ToString() });
		}

		[HttpPost]
		public IActionResult SupplierCreate(Supplier supplier)
		{
			string answer = Cls_Supplier.SupplierInsert(supplier);

			if (answer == "başarılı")
			{
				TempData["Message"] = supplier.BrandName + " Supplier Eklendi.";
			}
			else if (answer == "zaten var")
			{
				TempData["Message"] = "Bu supplier daha önceden eklenmiş.";
			}
			else
			{
				TempData["Message"] = "HATA!!!!! Supplier Eklenemedi.";

			}
			return RedirectToAction("SupplierCreate"); //[HttpGet]
													   //return View(); [HttpPost]
		}

		public async Task<IActionResult> SupplierEdit(int? id)
		{

			if (id == null || context.Suppliers == null)
			{
				return NotFound();
			}

			var supplier = await s.SupplierDetails(id);
			return View(supplier);
		}

		[HttpPost]
		public IActionResult SupplierEdit(Supplier supplier)
		{
			if (supplier.PhotoPath == null)
			{
				supplier.PhotoPath = context.Suppliers.FirstOrDefault(s => s.SupplierID == supplier.SupplierID).PhotoPath;
			}

			bool answer = Cls_Supplier.SupplierUpdate(supplier);

			if (answer)
			{
				TempData["Message"] = supplier.BrandName + " Markası Güncellendi.";
				return RedirectToAction("SupplierIndex");
			}
			else
			{
				TempData["Message"] = "HATA!!!!! Marka Güncellenemedi.";
				return RedirectToAction(nameof(SupplierEdit));
			}
		}


		public static int global_supplierid;
		public async Task<IActionResult> SupplierDetails(int id)
		{
			if (id != 0)
			{
				global_supplierid = id;
			}

			if (id == 0)
			{
				id = global_supplierid;
			}
			var supplier = await s.SupplierDetails(id);

			//menüden tekrar tıklanamaz ise çözüm bu
			ViewBag.BrandName = supplier?.BrandName;
			ViewData["Title"] = supplier.BrandName;

			return View(supplier);

		}

		[HttpGet]
		public async Task<IActionResult> SupplierDelete(int? id)
		{
			if (id == 0 || context.Suppliers == null)
			{
				return NotFound();
			}

			var supplier = await context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);

			if (supplier == null)
			{
				return NotFound();
			}
			return View(supplier);
		}


		[HttpPost, ActionName("SupplierDelete")]
		public async Task<IActionResult> SupplierDeleteConfirmed(int id)
		{
			bool result = Cls_Supplier.SupplierDelete(id);
			if (result)
			{
				TempData["Message"] = "Silindi";
				return RedirectToAction("SupplierIndex");
			}
			else
			{
				TempData["Message"] = "HATA Silinemedi";
				return RedirectToAction(nameof(SupplierDelete));
			}
		}


		/*************************    STATUS   **************************/


		public async Task<IActionResult> StatusIndex()
		{
			List<Status> statuses = await st.StatusSelect();
			return View(statuses);
		}

		[HttpGet]
		public IActionResult StatusCreate()
		{
			return View();
		}

		[HttpPost]
		public IActionResult StatusCreate(Status status)
		{
			string answer = Cls_Status.StatusInsert(status);

			if (answer == "başarılı")
			{
				TempData["Message"] = status.StatusName + " Supplier Eklendi.";
			}
			else if (answer == "zaten var")
			{
				TempData["Message"] = "Bu statü daha önceden eklenmiş.";
			}
			else
			{
				TempData["Message"] = "HATA!!!!! Statü Eklenemedi.";

			}
			return RedirectToAction("StatusCreate"); //[HttpGet]
													 //return View(); [HttpPost]
		}

		[HttpGet]
		public async Task<IActionResult> StatusEdit(int? id)
		{

			if (id == null || context.Statuses == null)
			{
				return NotFound();
			}

			var status = await st.StatusDetails(id);
			return View(status);
		}

		[HttpPost]
		public IActionResult StatusEdit(Status status)
		{

			bool answer = Cls_Status.StatusUpdate(status);

			if (answer)
			{
				TempData["Message"] = status.StatusName + " Statüsü Güncellendi.";
				return RedirectToAction("StatusIndex");
			}
			else
			{
				TempData["Message"] = "HATA!!!!! Statü Güncellenemedi.";
				return RedirectToAction(nameof(StatusEdit));
			}
		}


		public static int global_statusid;
		public async Task<IActionResult> StatusDetails(int id)
		{
			if (id != 0)
			{
				global_statusid = id;
			}

			if (id == 0)
			{
				id = global_statusid;
			}
			var status = await st.StatusDetails(id);

			//menüden tekrar tıklanamaz ise çözüm bu
			ViewBag.statusname = status?.StatusName;
			ViewData["Title"] = status.StatusName;

			return View(status);
		}

		[HttpGet]
		public async Task<IActionResult> StatusDelete(int? id)
		{
			if (id == 0 || context.Statuses == null)
			{
				return NotFound();
			}

			var status = await context.Statuses.FirstOrDefaultAsync(c => c.StatusID == id);

			if (status == null)
			{
				return NotFound();
			}
			return View(status);
		}


		[HttpPost, ActionName("StatusDelete")]
		public async Task<IActionResult> StatusDeleteConfirmed(int id)
		{
			bool result = Cls_Status.StatusDelete(id);
			if (result)
			{
				TempData["Message"] = "Silindi";
				return RedirectToAction("StatusIndex");
			}
			else
			{
				TempData["Message"] = "HATA Silinemedi";
				return RedirectToAction(nameof(StatusDelete));
			}
		}



        /****************************** PRODUCT **********************/

        public async Task<IActionResult> ProductIndex()
        {
            List<Product> products = await p.ProductSelect();
            return View(products);
        }

        async void StatusFill()
        {
            List<Status> statuses = await st.StatusSelect();
            ViewData["statusList"] = statuses.Select(c => new SelectListItem { Text = c.StatusName, Value = c.StatusID.ToString() });
        }

        async void CategoryFillAll()
        {
			List<Category> categories = await c.CategorySelect();
            ViewData["categoryList"] = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> ProductCreate()
        {
            List<Category> categories = await c.CategorySelect();
            ViewData["categoryList"] = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });



            List<Supplier> suppliers = await s.SupplierSelect();
            ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });



            List<Status> statuses = await st.StatusSelect();
            ViewData["StatusList"] = statuses.Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusID.ToString() });



            return View();
        }

        [HttpPost]
        public IActionResult ProductCreate(Product product)
        {
			
            string answer = Cls_Product.ProductInsert(product);



            if (answer == "başarılı")
            {
                TempData["Message"] = product.ProductName + " Ürünü Eklendi";
            }
            else if (answer == "Bu ürün Zaten Var!!")
            {
                TempData["Message"] = product.ProductName + " Ürünü Daha Önceden Eklenmiştir!.";
            }
            else
            {
                TempData["Message"] = product.ProductName + " Ürünü Eklenemedi.";
            }
            return RedirectToAction("ProductCreate"); //[HttpGet]
        }

		[HttpGet]
        public async Task<IActionResult> ProductEdit(int? id)
        {
            CategoryFill();
            SupplierFill();
            StatusFill();

            if (id == null || context.Products == null)
            {
                return NotFound();
            }
            var product = await p.ProductDetails(id);
            return View(product);
        }

		[HttpPost]
		public IActionResult ProductEdit(Product product)
		{
			//veritabanından kaydını getirdim
			Product? prd = context.Products.FirstOrDefault(s => s.ProductID == product.ProductID);
			//formdan gelmeyen , bazı kolonları null yerine , eski bilgilerini bastım
			product.AddDate = prd.AddDate;
			product.Highlighted = prd.Highlighted;
			product.TopSeller = prd.TopSeller;

			if (product.PhotoPath == null)
			{
				product.PhotoPath = prd.PhotoPath;
			}

			bool answer = Cls_Product.ProductUpdate(product);
			if (answer == true)
			{
				TempData["Message"] = "Güncellendi";
				return RedirectToAction("ProductIndex");
			}
			else
			{
				TempData["Message"] = "HATA";
				return RedirectToAction(nameof(ProductEdit));
			}
		}

		public async Task<IActionResult> ProductDetails(int? id)
        {
            var product = await context.Products.FirstOrDefaultAsync(c => c.ProductID == id);
            ViewBag.productname = product?.ProductName;
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDelete(int? id)
        {
            if (id == null || context.Products == null)
            {
                return NotFound();
            }

            var product = await context.Products.FirstOrDefaultAsync(c => c.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("ProductDelete")]
        public async Task<IActionResult> ProductDeleteConfirmed(int id)
        {
            bool answer = Cls_Product.ProductDelete(id);
            if (answer == true)
            {
                TempData["Message"] = "Silindi";
                return RedirectToAction("ProductIndex");
            }
            else
            {
                TempData["Message"] = "HATA";
                return RedirectToAction(nameof(ProductDelete));
            }
        }


    }

}
