using iakademi47_proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace iakademi47_proje.ViewComponents
{
	public class Telephone:ViewComponent
	{
		iakademi47Context context = new iakademi47Context();

		public string Invoke()
		{
			string telephone = context.Settings.FirstOrDefault(s => s.SettingID == 1).Telephone;
			return $"{telephone}";
		}
	}
}
