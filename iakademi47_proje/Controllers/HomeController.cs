using iakademi47_proje.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PagedList.Core; //NuGet Package Manager'dan PagedList.Core.Mvc package'i yüklenerek eklendi bu namespace. Rakamlı sayfalama için, arabam.com'da yapıldığı gibi.
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace iakademi47_proje.Controllers
{
    public class HomeController : Controller
    {
        //public HomeController()
        //{
        //    this.mainpageCount = context.Settings.FirstOrDefault(s => s.SettingID == 1).MainPageCount;
        //}

        //int mainpageCount = 0;
        Cls_Product p = new Cls_Product();
        MainPageModel mpm = new MainPageModel();
        iakademi47Context context = new iakademi47Context();
        Cls_Order cls_order = new Cls_Order();

        public IActionResult Index()
        {
            //new=anasayfa , ""=alt sayfa için parametre , 0 = ajax için parametre
            mpm.SliderProducts = p.ProductSelect("slider", "", 0);
            mpm.NewProducts = p.ProductSelect("new", "", 0);
            mpm.Productofday = p.ProductDetails();
            mpm.SpecialProducts = p.ProductSelect("Special", "", 0);
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "", 0);
            mpm.HighlightedProducts = p.ProductSelect("Highlighted", "", 0);
            mpm.TopsellerProducts = p.ProductSelect("Topseller", "", 0);
            mpm.StarProducts = p.ProductSelect("Star", "0", 0);
            mpm.FeaturedProducts = p.ProductSelect("Featured", "", 0);
            mpm.NotableProducts = p.ProductSelect("Notable", "", 0);
            return View(mpm);
        }

        public IActionResult NewProducts()
        {
            mpm.NewProducts = p.ProductSelect("new", "new", 0);
            return View(mpm);
        }

        public PartialViewResult _partialNewProducts(int nextpagenumber)
        {
            mpm.NewProducts = p.ProductSelect("new", "new", nextpagenumber);
            return PartialView(mpm);
        }

        public IActionResult SpecialProducts()
        {
            mpm.SpecialProducts = p.ProductSelect("Special", "Special", 0);
            return View(mpm);
        }

        public PartialViewResult _partialSpecialProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.SpecialProducts = p.ProductSelect("Special", "Special", pagenumber);
            return PartialView(mpm);
        }

        public IActionResult DiscountedProducts()
        {
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "Discounted", 0);
            return View(mpm);
        }

        public PartialViewResult _partialDiscountedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.DiscountedProducts = p.ProductSelect("Discounted", "Discounted", pagenumber);
            return PartialView(mpm);
        }

        public IActionResult HighlightedProducts()
        {
            mpm.HighlightedProducts = p.ProductSelect("Highlighted", "Highlighted", 0);
            return View(mpm);
        }

        public PartialViewResult _partialHighlightedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.HighlightedProducts = p.ProductSelect("Highlighted", "Highlighted", pagenumber);
            return PartialView(mpm);
        }

        public IActionResult TopsellerProducts(int page = 1, int pageSize = 8)
        {
            // Home/TopsellerProducts 'a ilk istek atıldığında page = 1 olacak , eğer ki page'e metot içinde default değer atamassak ilk istekte page değişkeninin default değeri olan 0 olur ve alt satırda exception fırlatır. Bundan dolayı değişkene atama yapıyoruz. 2. sayfaya tıkladığımızda ise query string vasıtasıyla /Home/TopsellerProducts?page=2 şeklinde istek atılıyor. Dolayısıyla yukarıdaki parametre adı page olmalıdır. Başka bir ad verirsek Model Binding yapılamaz ve değer atanamaz.

            //Aşağıdaki class'ın tanımlı olduğu Package = PagedList.Core.Mvc . NuGet Package Manager'dan yükleniyor ve PagedList.Core namespace'i içinde bulunuyor aşağıdaki class. Bu paketin içinde tag helper'da var ve _ViewImports dosyasına @addTagHelper *, PagedList.Core.Mvc şeklinde eklendi View sayfasında tag helper'ı kullanabilmek için.

            PagedList<Product> model = new PagedList<Product>(context.Products.OrderByDescending(p => p.TopSeller), page, pageSize);

            return View("TopsellerProducts", model);

        }

        public IActionResult CartProcess(int id) //SEPETE EKLE
        {

            //cookie adı sepetim(key),aşağıdakilerde değerleri(value) 
            //10=1&      10 id'li üründen 1 adet demek , &(ampersand) işareti parametreleri ayırmak için
            //20=1&
            //30=4

            Cls_Product.Highlighted_Increase(id); //ürün sepete eklenince ürünün Highlighted değeri 1 arttırılacak.
            cls_order.ProductID = id;
            cls_order.Quantity = 1;
            string? cookie = Request.Cookies["sepetim"]; //tarayıcıdan sepetim(key) adında cookie okuma. Sepetim isimli key değeri var mı yok mu diye. Eğer yoksa null döner. Var ise sepetim isimli key'in value'sunu döner. Örneğin value olarak 36=1 şeklinde string dönüyor. 36=1 şeklinde yapmayı biz tanımladık ilk cookie'yi oluştururken(aşağıdaki if else koşulunda). Eğer sepetim(key) adında cookie var ama value yoksa empty string döner. Bu seneryo daha önce cookie oluşturulmuş ama sonradan sepetin içindeki ürünlerin silinmesinden sonra gerçekleşir.
            var cookieOptions = new CookieOptions(); //Cookie oluştururken kullanmak için CookieOptions nesnesi üretme.
            if (cookie == null || cookie == string.Empty) //cookie = cookie'nin value'su
            {
                cookieOptions.Expires = DateTime.Now.AddDays(7); //7 günlük çerez süresi tanımladık
                cls_order.AddToMyCart(id.ToString()); //cls_order.MyCart property'sine atama yapmak için bu metodu yazdık.Çünkü bir alt satırda lazım olacak.
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                //HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                TempData["Message"] = "Ürün Sepetinize Eklendi";
            }
            else
            {
                cls_order.MyCart = cookie; //Cookie'nin değerini(ki MyCart) MyCart property'sine koyduk.
                if (cls_order.AddToMyCart(id.ToString()) == false)
                {
                    //sepet dolu,aynı ürün değilse
                    Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                    //HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                    TempData["Message"] = "Ürün Sepetinize Eklendi";
                }
                else
                {
                    //HttpContext.Session.SetString("Message", "Ürün Sepetinizde Zaten Var");
                    TempData["Message"] = "Ürün Sepetinizde Zaten Var";
                }
            }
            string url = Request.Headers["Referer"];
            return Redirect(url);
        }

        public IActionResult Cart() //Sepetim
        {
            List<Cls_Order> MyCart;
            //silme butonu ile gelince
            if (HttpContext.Request.Query["scid"].ToString() != "")
            {
                int scid = Convert.ToInt32(HttpContext.Request.Query["scid"].ToString());
                cls_order.MyCart = Request.Cookies["sepetim"];
                cls_order.DeleteFromMyCart(scid.ToString());

                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                TempData["Message"] = "Ürün Sepetinizden Silindi";
                MyCart = cls_order.SelectMyCart();
                ViewBag.MyCart = MyCart;
                ViewBag.MyCart_Table_Details = MyCart;

            }
            else
            {
                //sag üst köşede sepet sayfama git butonuyla gelince
                var cookie = Request.Cookies["sepetim"];
                if (cookie == null || cookie == string.Empty)
                {
                    //sepette hiç ürün olmayabilir
                    var cookieOptions = new CookieOptions();
                    cls_order.MyCart = "";
                    MyCart = cls_order.SelectMyCart();
                    ViewBag.MyCart = MyCart;
                    ViewBag.MyCart_Table_Details = MyCart;
                }
                else
                {
                    //sepette ürün var
                    var cookieOptions = new CookieOptions();
                    cls_order.MyCart = Request.Cookies["sepetim"];
                    MyCart = cls_order.SelectMyCart();
                    ViewBag.MyCart = MyCart;
                    ViewBag.MyCart_Table_Details = MyCart;
                }
            }

            if (MyCart.Count == 0)
            {
                ViewBag.MyCart = null;
            }

            return View();
        }


        [HttpGet]
        public IActionResult Order() //Sepete ekleme sonrası , sonraki adıma geç metodu
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                User? user = Cls_User.SelectMemberInfo(HttpContext.Session.GetString("Email").ToString());
                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult Order(IFormCollection frm)
        {
            //string? kredikartno = Request.Form["kredikartno"]; //IFormCollection olmadan yakalama
            string kredikartno = frm["kredikartno"].ToString(); //IFormCollection zorunlu
            string kredikartay = frm["kredikartay"].ToString();
            string kredikartyil = frm["kredikartyil"].ToString();
            string kredikartcvs = frm["kredikartcvs"].ToString();

            //bankaya git,eğer true gelirse(onay gelirse),
            //Order tablosuna kayıt atacağız.
            //digital-planet e (e-fatura) bilgilerini gönder

            //payu --100 --1.90 --perşembe 
            //iyzico --100 --1 --40günde bir ödeme yapar

            string txt_tckimlikno = frm["txt_tckimlikno"].ToString();
            string txt_vergino = frm["txt_vergino"].ToString();

            if (txt_tckimlikno != "")
            {
                WebServiceController.tckimlikno = txt_tckimlikno;
                //fatura bilgilerini digital-planet şirketine gönderirsiniz (xml formatında)
                //sizin e-faturanızı oluşturacak
            }
            else
            {
                WebServiceController.vergino = txt_vergino;
            }

            //.Net , payu , WindowsForm ile yazılmış bir proje gönderiyorlar
            NameValueCollection data = new NameValueCollection();
            string url = "https://www.sedattefci.com/backref"; //payu'dan gelecek bilginin gideceği url

            data.Add("BACK_REF", url);
            data.Add("CC_CVV", kredikartcvs);
            data.Add("CC_NUMBER", kredikartno);
            data.Add("EXP_MONTH", kredikartay);
            data.Add("EXP_YEAR", kredikartyil);

            var deger = "";
            foreach (var item in data) //burada itemlar sırasıyla= BACK_REF,CC_CVV,CC_NUMBER,EXP_MONTH,EXP_YEAR
            {
                var value = item as string; //item string ise value'ya ata,değilse null ata. Bizim bütün item'lar string olduğu için null olmaz.
                var byteCount = Encoding.UTF8.GetByteCount(data.Get(value));
                deger += byteCount + data.Get(value);
            }

            var signatureKey = "payu üyeliğinde size verilen SECRET_KEY burada olacak";
            var hash = HashWithSignature(deger, signatureKey);

            data.Add("ORDER_HASH", hash);

            var x = POSTFormPAYU("https://secure.payu.com.tr/order/.......", data);

            //sanal kart
            if (x.Contains("<STATUS>SUCCESS</STATUS>") && x.Contains("<RETURN_CODE>3DS_ENROLLED</RETURN_CODE>"))
            {
                //sanal kredi kartı OK
            }
            else
            {
                //gerçek kredi kartı
            }

            return RedirectToAction("backref");
        }


        public static string POSTFormPAYU(string url, NameValueCollection data)
        {
            return "";
        }

        public static string HashWithSignature(string deger, string signatureKey)
        {
            return "";
        }

        public IActionResult backref()
        {
            Confirm_Order();
            return RedirectToAction("Confirm");
        }

        /*
         1 aaaaaaa 1909202319334510-26345445654
         2 bbbbbbb 1909202319334510-
         3 ccccccc 1909202319334510-
        75 eeeeeee 19022023222445124-7777777
        76 fffffff 19022023222445124
         */

        public static string OrderGroupGUID = "";

        public IActionResult Confirm_Order()
        {
            //siparis tablosuna kaydet
            //cookie sepetini sileceğiz
            //e-fatura oluşturacağız, e-fatura oluşturan xml metodu çağıracağız
            var cookie = Request.Cookies["sepetim"];
            if (cookie != null)
            {
                cls_order.MyCart = cookie;
                OrderGroupGUID = cls_order.WriteToOrderTable(HttpContext.Session.GetString("Email"));
                Response.Cookies.Delete("sepetim");

                bool result = Cls_User.SendSms(OrderGroupGUID);
                if (result == false)
                {
                    //Orders tablosunda sms kolonuna false değeri basılır
                    //Orders tablosunda sms kolonu = false olan siparişleri getir
                }

                //Cls_User.SendEMail(OrderGroupGUID);

                //1)sedattefci.com sitesinde müsteriden kredi kart bilgileri alınır
                //2)bu bilgiler payu yada iyzico (bu 2 site banka ile haberlesir) sitesine gönderilir
                //3) kredi kart bilgileri bankaya geldiğinde, banka kullanıcıya sms onayı gönderir
                //4) banka backref metoduna geri dönüş yapar, banka kredi karta okey vermişse, siz bir sms firmasıyla (netgsm) anlaştınız.SendSms metodu müşteriye siparişiniz onaylandı sms'i gönderir.Biz sms içeriklerini sms firmasına göndereceğiz. O firma sms gönderme işlemi yapacak.
                //5) digitalplanet müşteriye e-fatura gönderir.Ben digital planet'e siparişin içeriğini xml formatında gönderirim.
            }
            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            ViewBag.OrderGroupGUID = OrderGroupGUID;
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (Cls_User.loginEmailControl(user) == false)
            {
                bool answer = Cls_User.AddUser(user);
                if (answer)
                {
                    TempData["Message"] = "Kaydedildi.";
                    return RedirectToAction("Login");
                }
                TempData["Message"] = "Hata.Tekrar deneyiniz.";
            }

            else
            {
                TempData["Message"] = "Bu Email Zaten mevcut.Başka Deneyiniz.";
            }
            return View();

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            string answer = Cls_User.MemberControl(user);

            if (answer == "error")
            {
                TempData["Message"] = "Hata.Email ve/veya Şifre yanlış. Tekrar deneyin.";
            }
            else if (answer == "admin")
            {
                //email ve şifre dogru, admin
                HttpContext.Session.SetString("Admin", "Admin");
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Index", "Admin");

            }
            else
            {
                //email ve sifre dogru , normal kullanıcı
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Cart", "Home");
            }

            return View();
        }

        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                List<vw_MyOrders> orders = cls_order.SelectMyOrders(HttpContext.Session.GetString("Email").ToString());
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult DetailedSearch()
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Suppliers = context.Suppliers.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult DProducts(int CategoryID, string[] SupplierID, string price, string IsInStock)
        {
            price = price.Replace(" ", "");
            string[] PriceArray = price.Split('-');
            string startprice = PriceArray[0];
            string endprice = PriceArray[1];
            string sign = ">";
            if (IsInStock == "0")
            {
                sign = ">=";
            }

            int count = 0;
            string suppliervalue = ""; //1,2,4
            for (int i = 0; i < SupplierID.Length; i++)
            {
                if (count == 0)
                {
                    suppliervalue = "SupplierID =" + SupplierID[i];
                    count++;
                }
                else
                {
                    suppliervalue += " or SupplierID =" + SupplierID[i];
                }
            }

            string query = "select * from Products where  CategoryID = " + CategoryID + " and (" + suppliervalue + ") and (UnitPrice > " + startprice + " and UnitPrice < " + endprice + ") and Stock " + sign + " 0 order by ProductName";

            ViewBag.Products = p.SelectProductsByDetails(query);
            return View();
        }


        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Index");
        }

        public IActionResult CategoryPage(int id)
        {
            List<Product> products = p.ProductSelectWithCategoryID(id);
            return View(products);
        }

        public IActionResult SupplierPage(int id)
        {
            List<Product> products = p.ProductSelectWithSupplierID(id);
            return View(products);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            //LINQ Method Syntax
            //mpm.ProductDetails = context.Products.FirstOrDefault(p => p.ProductID == id);

            //  ado.net , dapper
            //select * from Products where ProductID = id

            //LINQ Query Syntax  - 4 nolu ürünün bütün kolon (sütün) bilgileri elimde
            mpm.ProductDetails = (from p in context.Products where p.ProductID == id select p).FirstOrDefault();

            //linq
            mpm.CategoryName = (from p in context.Products
                                join c in context.Categories
                              on p.CategoryID equals c.CategoryID
                                where p.ProductID == id
                                select c.CategoryName).FirstOrDefault();

            //linq
            mpm.BrandName = (from p in context.Products
                             join s in context.Suppliers
                           on p.SupplierID equals s.SupplierID
                             where p.ProductID == id
                             select s.BrandName).FirstOrDefault();

            //select * from Products where Related = 2 and ProductID != 4
            mpm.RelatedProducts = context.Products.Where(p => p.Related == mpm.ProductDetails!.Related && p.ProductID != id).ToList();

            Cls_Product.Highlighted_Increase(id);
            return View(mpm);
        }

        public PartialViewResult gettingProducts(string id)
        {
            id = id.ToUpper(new CultureInfo("tr-TR"));
            List<sp_arama> ulist = Cls_Product.gettingSearchProducts(id); //stored procedure'den                                                                    yapılan aramanın                                                                        sonuçlarını dönüyor.

            string json = JsonConvert.SerializeObject(ulist); //JSON string'e çevirme.
            var response = JsonConvert.DeserializeObject<List<Search>>(json); //JSON string'i                                                                               List<Search>'e                                                                          çevirme.
            return PartialView(response);
        }

        public IActionResult PharmacyOnDuty()
        {
            /*
                https://openfiles.izmir.bel.tr/100104/docs/cbs-WebServisKullanimDokumani-1.1.pdf

                https://openfiles.izmir.bel.tr/111324/docs/ibbapi-WebServisKullanimDokumani_1.0.pdf
            */


            string json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/nobetcieczaneler");
            var pharmacy = JsonConvert.DeserializeObject<List<Pharmacy>>(json);
            return View(pharmacy);
        }

        public IActionResult ArtAndCulture()
        {
            string json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/kultursanat/etkinlikler");
            var activite = JsonConvert.DeserializeObject<List<Activite>>(json);
            return View(activite);
        }

        public IActionResult WirelessIzmirLocations()
        {
            string? json = new WebClient().DownloadString("https://openapi.izmir.bel.tr/api/ibb/cbs/wizmirnetnoktalari");


            //WirelessIzmir? wirelessIzmir = JsonConvert.DeserializeObject<WirelessIzmir>(json);
            //VEYA AŞAĞIDAKİ GİBİ DESERIALIZE EDİLEBİLİR.


            WirelessIzmir? wirelessIzmir = JsonSerializer.Deserialize<WirelessIzmir>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return View(wirelessIzmir);
        }


    }
}
