using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;

namespace redis_example
{
    class Program
    {
        static void Main(string[] args)
        {
            //usingIRedisNativeClient();
            //usingIRedisClient();
            //usingIRedisTypedClient();
            //usingTransactions();
            //usingPublish();
            usingSubscribe();
            
            Console.ReadLine();
        }

        static void usingPublish()
        {
            using (IRedisClient client = new RedisClient())
            {
                client.PublishMessage("news", "testing");
            }
        }

        static void usingSubscribe()
        {
            using(IRedisClient client = new RedisClient() )
            {
                var sub = client.CreateSubscription();
                sub.OnMessage = (c, m) => Console.WriteLine("Got message: {0}, from channel {1}", m, c);
                sub.SubscribeToChannels("news");
            }
        }

        static void usingTransactions()
        {
            using(IRedisClient client = new RedisClient())
            {
                var transaction = client.CreateTransaction();
                transaction.QueueCommand(clinet => client.Set("abc", 1));
                transaction.QueueCommand(c => c.Increment("abc", 1));
                transaction.Commit();
                var result = client.Get<int>("abc");
                Console.WriteLine(result);
            }
        }

        static void usingIRedisTypedClient()
        {
            long lastId = 0;
            using (IRedisClient client = new RedisClient())
            {
                var customerClient = client.As<Customer>();
                var customer = new Customer()
                {
                    Id = customerClient.GetNextSequence(),
                    Address = "123 Main Street",
                    Name = "Bob",
                    Orders = new List<Order>()
                    {
                        new Order {OrderNumber="AB123" },
                        new Order {OrderNumber="AB124" }
                    }
                };
                var storedCustomer = customerClient.Store(customer);
                lastId = storedCustomer.Id;
            }
            using (IRedisClient client = new RedisClient())
            {
                var customerClient = client.As<Customer>();
                var customer = customerClient.GetById(lastId);
                Console.WriteLine("Got customer {0}, with name {1}", customer.Id, customer.Name);
            }
        }

        //a higher level of abstraction than IRedisNativeClient
        static void usingIRedisClient()
        {
            using(IRedisClient client = new RedisClient())
            {
                var customerNames = client.Lists["urn:customernames"];
                customerNames.Clear();
                customerNames.Add("Joe");
                customerNames.Add("Mary");
                customerNames.Add("Bob");
            }

            using (IRedisClient client = new RedisClient())
            {
                var customerNames = client.Lists["urn:customernames"];
                foreach(var customerName in customerNames)
                {
                    Console.WriteLine("Customer Name: {0}", customerName);
                }
            }
        }

        //IRedisNativeClient has one-to-one mapping to Redis official APIs
        static void usingIRedisNativeClient()
        {
            using (IRedisNativeClient client = new RedisClient())
            {
                client.Set("urn:messages:1", Encoding.UTF8.GetBytes("Hello C# World"));
            }
            using (IRedisNativeClient client = new RedisClient())
            {
                var result = Encoding.UTF8.GetString(client.Get("urn:messages:1"));
                Console.WriteLine("Message: {0}", result);
            }
        }
    }

    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        public string OrderNumber { get; set; }
    }
}
