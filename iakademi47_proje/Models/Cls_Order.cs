using Humanizer;

namespace iakademi47_proje.Models
{
    public class Cls_Order
    {
        iakademi47Context context = new iakademi47Context();

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public string? MyCart { get; set; } //10=1&20=1&30=1  MyCart = Cookie'nin value'sunun depolandığı prop

        public decimal UnitPrice { get; set; }

        public string? ProductName { get; set; }

        public int Kdv { get; set; }

        public string? PhotoPath { get; set; }
        

        public bool AddToMyCart(string id) //cookie'ye value atama metodu
        {
            bool exists = false;

            if (MyCart == null) //MyCart = cookie'nin value'sunun depolandığı property
            {
                MyCart = id + "=1";
            }
            else
            {
                string[] MyCartArray = MyCart.Split('&');
                //10=1 MyCartArray[0]
                //20=1 MyCartArray[1]
                //30=1 MyCartArray[2]
                for (int i = 0; i < MyCartArray.Length; i++)
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split('=');
                    //10
                    //1
                    if (MyCartArrayLoop[0] == id)
                    {
                        //bu ürün zaten sepette var
                        exists = true;
                    }
                }
                if (exists == false)
                {
                    MyCart = MyCart + "&" + id.ToString() + "=1";
                }
            }
            return exists;
        }

        public void DeleteFromMyCart(string scid) //sepetten silme metodu
        {
            string[] MyCartArray = MyCart.Split('&');
            string NewMyCart = "";
            int count = 1;

            for (int i = 0; i < MyCartArray.Length; i++)
            {
                string[] MyCartArrayLoop = MyCartArray[i].Split('=');
                if (count == 1)
                {
                    //yeni sepetin icine,silinmeyecek ürünleri koyacağım
                    //yeni sepete ilk ürünü koyuyorum
                    if (MyCartArrayLoop[0] != scid) //yani sepette 10 , 20 ,30 id'li ürünler varsa ve 10 silinmek isteniyorsa NewMyCart'a 20 ile 30 id'li ürün konulmaktadır.
                    {
                        NewMyCart += MyCartArrayLoop[0] + "=" + Convert.ToInt32(MyCartArrayLoop[1]);
                        count++;
                    }
                }
                else
                {
                    //count 1 den büyükse,yeni sepette en az 1 ürün var
                    if (MyCartArrayLoop[0] != scid)
                    {
                        NewMyCart += "&" + MyCartArrayLoop[0] + "=" + Convert.ToInt32(MyCartArrayLoop[1]);
                        //count++;
                    }
                }
            }

            MyCart = NewMyCart;
        }

        //sağ üst köşedeki sepet sayfasına git tıklanınca
        public List<Cls_Order> SelectMyCart()
        {
            List<Cls_Order> list = new List<Cls_Order>();
            string[] MyCartArray = MyCart.Split('&');

            if (MyCartArray[0] != "")
            {
                for (int i = 0; i < MyCartArray.Length; i++)
                {
                    string[] MyCartArrayLoop = MyCartArray[i].Split('=');
                    int sepetid = Convert.ToInt32(MyCartArrayLoop[0]);

                    Product? product = context.Products.FirstOrDefault(p => p.ProductID == sepetid);

                    Cls_Order pr = new Cls_Order();
                    pr.ProductID = product.ProductID;
                    pr.Quantity = Convert.ToInt32(MyCartArrayLoop[1]);
                    pr.UnitPrice = product.UnitPrice;
                    pr.ProductName = product.ProductName;
                    pr.Kdv = product.Kdv;
                    pr.PhotoPath = product.PhotoPath;
                    list.Add(pr); //her bir ürünün bütün kolon bilgileri listeye ekleniyor
                }
            }

            return list;
        }

        public string WriteToOrderTable(string Email)
        {
            string OrderGroupGUID = DateTime.Now.ToString().Replace(":", "").Replace(".", "").Replace(" ", "").Replace(",", "");

            DateTime OrderDate = DateTime.Now;
            try
            {
                List<Cls_Order> orders = SelectMyCart();
                foreach (var item in orders)
                {
                    Order order = new Order();
                    order.OrderDate = OrderDate;
                    order.OrderGroupGUID = OrderGroupGUID;
                    order.UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;
                    order.ProductID = item.ProductID;
                    order.Quantity = item.Quantity;

                    context.Orders.Add(order);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                OrderGroupGUID = "Error";
            }
            
            return OrderGroupGUID;
        }

        public List<vw_MyOrders> SelectMyOrders(string Email)
        {
            int UserID = context.Users.FirstOrDefault(u => u.Email == Email).UserID;

            List<vw_MyOrders> myOrders = context.vw_MyOrders.Where(o => o.UserID == UserID).ToList();

            return myOrders;
        }

    }
}
