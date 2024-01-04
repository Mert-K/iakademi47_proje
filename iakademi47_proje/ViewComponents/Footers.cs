using iakademi47_proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace iakademi47_proje.ViewComponents
{
	public class Footers:ViewComponent
	{
		iakademi47Context context = new iakademi47Context();

		public IViewComponentResult Invoke()
		{
			List<Supplier> suppliers = context.Suppliers.ToList();
			return View(suppliers);
		}
	}
}
