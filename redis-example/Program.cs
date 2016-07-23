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
            usingLowLevelInterface();
        }

        //IRedisNativeClient has one-to-one mapping to Redis official APIs
        static void usingLowLevelInterface()
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

            Console.ReadLine();
        }
    }
}
