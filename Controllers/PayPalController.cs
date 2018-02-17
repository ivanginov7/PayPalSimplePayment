using PayPal.Api;
using PayPal.Models;
using PayPal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayPal.Controllers
{
    public class PayPalController : Controller
    {
        // POST: PayPal
        [HttpPost]
        public ActionResult Checkout()
        {
            //SETUP
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);


            var context = new PayPalContext();
            var products = context.Products.AsEnumerable();
            var book = products.Where(p => p.Name == "book").FirstOrDefault();
            var album = products.Where(p => p.Name == "album").FirstOrDefault();
            var orderitem1 = new OrderItem { Product = book, Count = 2, Id = 1, ProductId = book.Id, ProductPrice = book.Price };
            var orderitem2 = new OrderItem { Product = album, Count = 3, Id = 2, ProductId = album.Id, ProductPrice = album.Price};
            var order = new Models.Order { OrderItems = new List<OrderItem> { orderitem1, orderitem2 }, TotalCost = Math.Round(orderitem1.Cost + orderitem2.Cost,2)};

            var subtotal = Math.Round(order.TotalCost,2);
            var tax = Math.Round(subtotal * 20 / 100, 2);
            var shipping = Math.Round(subtotal * 10 / 100, 2);
            var total = Math.Round(subtotal + tax + shipping, 2);
            
            context.Orders.Add(order);
            context.SaveChanges();
            var orderId = order.Id;
            //SETUP

            var payer = new Payer() { payment_method = "paypal" };
            var guid = Convert.ToString(new Random().Next(100000));
            var redirUrls = new RedirectUrls()
            {
                cancel_url = "http://localhost:49334/paypal/cancel",
                return_url = "http://localhost:49334/paypal/process"
            };
            var itemList = new ItemList()
            {
                items = new List<Item>()
                {
                    new Item()
                    {
                        name = order.OrderItems.Where(i=>i.Product.Name == "book").FirstOrDefault().Product.Name,
                        currency = "EUR",
                        price = order.OrderItems.Where(i=>i.Product.Name == "book").FirstOrDefault().Product.Price.ToString(),
                        quantity = order.OrderItems.Where(i=>i.Product.Name == "book").FirstOrDefault().Count.ToString(),
                        sku = "sku"
                    },
                    new Item()
                    {
                        name = order.OrderItems.Where(i=>i.Product.Name == "album").FirstOrDefault().Product.Name,
                        currency = "EUR",
                        price = order.OrderItems.Where(i=>i.Product.Name == "album").FirstOrDefault().Product.Price.ToString(),
                        quantity = order.OrderItems.Where(i=>i.Product.Name == "album").FirstOrDefault().Count.ToString(),
                        sku = "sku"
                    }
                }
            };
            
            var details = new Details()
            {
                tax = Convert.ToString(tax),
                shipping = Convert.ToString(shipping),
                subtotal = Convert.ToString(subtotal)
            };
            
            var amount = new Amount()
            {
                currency = "EUR",
                total = Convert.ToString(total),
                details = details
            };
            
            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "transaction with dummy data",
                invoice_number = Common.GetRandomInvoiceNumber(),
                amount = amount,
                item_list = itemList
            });
            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                redirect_urls = redirUrls,
                transactions = transactionList
            };
            
            //Initialize the payment and redirect the user
            try
            {
       
                var createdPayment = payment.Create(apiContext);
               
                return Redirect(createdPayment.GetApprovalUrl());
            }
            catch (PayPal.PaymentsException e)
            {
                return Content(e.Details.ConvertToJson());
            }
            
        }
        public ActionResult Cancel()
        {
            return View();
        }
        public ActionResult Process(string paymentId, string PayerID)
        {
            //SETUP
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            var paymentExecution = new PaymentExecution() { payer_id = PayerID };
            var payment = new Payment() { id = paymentId };

            //execute payment
            var executedPayment = payment.Execute(apiContext, paymentExecution);
            var model = executedPayment.state;
            return Content(model);
        }
    }
}