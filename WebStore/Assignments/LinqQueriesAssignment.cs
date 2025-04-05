using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebStore.Entities;

namespace WebStore.Assignments
{
    /// Additional tutorial materials https://dotnettutorials.net/lesson/linq-to-entities-in-entity-framework-core/

    /// <summary>
    /// This class demonstrates various LINQ query tasks 
    /// to practice querying an EF Core database.
    /// 
    /// ASSIGNMENT INSTRUCTIONS:
    ///   1. For each method labeled "TODO", write the necessary
    ///      LINQ query to return or display the required data.
    ///      
    ///   2. Print meaningful output to the console (or return
    ///      collections, as needed).
    ///      
    ///   3. Test each method by calling it from your Program.cs
    ///      or test harness.
    /// </summary>
    public class LinqQueriesAssignment
    {

        private readonly WebStoreContext _dbContext;

        public LinqQueriesAssignment(WebStoreContext context)
        {
            _dbContext = context;
        }


        /// <summary>
        /// 1. List all customers in the database:
        ///    - Print each customer's full name (First + Last) and Email.
        /// </summary>
        public async Task Task01_ListAllCustomers()
        {   
            var customers = await _dbContext.Customers
               // .AsNoTracking() // optional for read-only
               .ToListAsync();

            Console.WriteLine("=== Task 01: List All Customers ===");

            if (customers == null || customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - {c.Email}");
            }
        }

        /// <summary>
        /// 2. Fetch all orders along with:
        ///    - Customer Name
        ///    - Order ID
        ///    - Order Status
        ///    - Number of items in each order (the sum of OrderItems.Quantity)
        /// </summary>
        public async Task Task02_ListOrdersWithItemCount()
        {

            Console.WriteLine("\n=== Task 02: List Orders With Item Count ===");

            int totalCount = 0;

            var orders = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            foreach (var c in orders)
            {
                int itemCount =+ c.OrderItems.Sum(i => i.Quantity);
                Console.WriteLine($"Name: {c.Customer.FirstName}, Orderstatus: {c.OrderStatus}, Quantity: {itemCount}");

                totalCount += itemCount;
            }

            Console.WriteLine("Total items ordered: " + totalCount);
        }

        /// <summary>
        /// 3. List all products (ProductName, Price),
        ///    sorted by price descending (highest first).
        /// </summary>
        public async Task Task03_ListProductsByDescendingPrice()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 03: List Products By Descending Price ===");

            var products = await _dbContext.Products
                .OrderByDescending(x => x.Price)
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (var x in products)
            {
                Console.WriteLine($"{x.ProductName}: {x.Price}");
            }
        }

        /// <summary>
        /// 4. Find all "Pending" orders (order status = "Pending")
        ///    and display:
        ///      - Customer Name
        ///      - Order ID
        ///      - Order Date
        ///      - Total price (sum of unit_price * quantity - discount) for each order
        /// </summary>
        public async Task Task04_ListPendingOrdersWithTotalPrice()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 04: List Pending Orders With Total Price ===");

            var orders = await _dbContext.Orders
                .Where(x => x.OrderStatus == "Pending")
                .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            foreach (var order in orders)
            {
                var totalPrice = order.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) - oi.Discount);

                Console.WriteLine($"Name: {order.Customer.FirstName} ID: {order.OrderId} Date: {order.OrderDate} Price: {totalPrice}");
            }
        }

        /// <summary>
        /// 5. List the total number of orders each customer has placed.
        ///    Output should show:
        ///      - Customer Full Name
        ///      - Number of Orders
        /// </summary>
        public async Task Task05_OrderCountPerCustomer()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 05: Order Count Per Customer ===");

            var customerOrderCount = await _dbContext.Orders
                .GroupBy(o => o.Customer)
                .Select(g => new
                {
                    CustomerName = $"{g.Key.FirstName} {g.Key.LastName}",
                    OrderCount = g.Count()
                })
                .ToListAsync();

            if (customerOrderCount == null || customerOrderCount.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            foreach (var item in customerOrderCount)
            {
                 Console.WriteLine($"Full name: {item.CustomerName}, Order count: {item.OrderCount}");
            }
        }

        /// <summary>
        /// 6. Show the top 3 customers who have placed the highest total order value overall.
        ///    - For each customer, calculate SUM of (OrderItems * Price).
        ///      Then pick the top 3.
        /// </summary>
        public async Task Task06_Top3CustomersByOrderValue()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 06: Top 3 Customers By Order Value ===");

            var list = await _dbContext.Orders
                .OrderByDescending(x => x.OrderItems.Sum(x => x.UnitPrice))
                .Select(g => new 
                { 
                    Name = $"{g.Customer.FirstName}", 
                    Total = g.OrderItems.Sum(x => x.UnitPrice)
                })
                .Take(3)
                .ToListAsync();

            if (list == null || list.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }

            foreach (var item in list)
            {
                Console.WriteLine($"Name: {item.Name} Total: {item.Total}");
            }
        }

        /// <summary>
        /// 7. Show all orders placed in the last 30 days (relative to now).
        ///    - Display order ID, date, and customer name.
        /// </summary>
        public async Task Task07_RecentOrders()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 07: Recent Orders ===");

            var orders = await _dbContext.Orders
                .Where(x => x.OrderDate >= DateTime.Now.AddDays(-30))
                .ToListAsync();

            if (orders.Count == 0){
                Console.WriteLine("No orders found from past 30 days.");
                return;
            }

            foreach (var item in orders)
            {
                Console.WriteLine($"ID: {item.OrderId}, Date: {item.OrderDate}, Name: {item.Customer.FirstName} {item.Customer.LastName}");
            }
        }

        /// <summary>
        /// 8. For each product, display how many total items have been sold
        ///    across all orders.
        ///    - Product name, total sold quantity.
        ///    - Sort by total sold descending.
        /// </summary>
        public async Task Task08_TotalSoldPerProduct()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 08: Total Sold Per Product ===");

            var products = await _dbContext.Products
                .OrderByDescending(x => x.OrderItems.Sum(x => x.Quantity))
                .ToListAsync();

            if (products == null || products.Count == 0)
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (var item in products)
            {
                int productTotal = item.OrderItems.Sum(x => x.Quantity);
                Console.WriteLine($"Name: {item.ProductName} Total sold: {productTotal}");
            }
        }

        /// <summary>
        /// 9. List any orders that have at least one OrderItem with a Discount > 0.
        ///    - Show Order ID, Customer name, and which products were discounted.
        /// </summary>
        public async Task Task09_DiscountedOrders()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 09: Discounted Orders ===");

            var discountedOrders = await _dbContext.Orders
                .Where(x => x.OrderItems.Any(z => z.Discount > 0))
                .ToListAsync();

            if (discountedOrders == null || discountedOrders.Count == 0)
            {
                Console.WriteLine("No discounted orders found.");
                return;
            }

            foreach (var order in discountedOrders)
            {
                var discountedItems = order.OrderItems.Where(oi => oi.Discount > 0).ToList();

                foreach (var item in discountedItems)
                {
                    Console.WriteLine($"ID: {order.OrderId}, Name: {order.Customer.FirstName} {order.Customer.LastName}, Product name: {item.Product.ProductName}, Discount: -{item.Discount}€");
                }
            }
        }

        /// <summary>
        /// 10. (Open-ended) Combine multiple joins or navigation properties
        ///     to retrieve a more complex set of data. For example:
        ///     - All orders that contain products in a certain category
        ///       (e.g., "Electronics"), including the store where each product
        ///       is stocked most. (Requires `Stocks`, `Store`, `ProductCategory`, etc.)
        ///     - Or any custom scenario that spans multiple tables.
        /// </summary>
        public async Task Task10_AdvancedQueryExample()
        {
            Console.WriteLine(" ");
            Console.WriteLine("=== Task 10: Advanced Query Example ===");

            var electronics = await _dbContext.Products
                .Where(x => x.Categories
                .Any(x => x.CategoryName == "Electronics"))
                .ToListAsync();

            foreach (var item in electronics)
            {   
                var ordersWithElectronics = await _dbContext.Orders
                    .Where(x => x.OrderItems
                    .Any(x => x.ProductId == item.ProductId))
                    .ToListAsync();

                var storeWithBiggestStock = await _dbContext.Stocks
                    .Include(x => x.Store)
                    .Where(x => x.ProductId == item.ProductId)
                    .OrderByDescending(x => x.QuantityInStock)
                    .FirstOrDefaultAsync();

                if (storeWithBiggestStock != null)
                    Console.WriteLine($"{item.ProductName} has highest stock in {storeWithBiggestStock.Store.StoreName}");


                foreach (var order in ordersWithElectronics)
                {

                    Console.WriteLine($"Order ID: {order.OrderId} containts: {item.ProductName}");
                }
            }
        }
    }
}
