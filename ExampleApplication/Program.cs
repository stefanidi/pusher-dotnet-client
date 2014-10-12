using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ExampleApplication
{

    class Program
    {
        static Pusher _pusher = null;
        static Channel _chatChannel = null;
        static PresenceChannel _presenceChannel = null;
            
        static void Main(string[] args)
        {
            InitPusher();

            Console.ReadLine();
            _pusher.Disconnect();

            Console.ReadLine();
        }

        private static void InitPusher()
        {
            _pusher = new Pusher("de504dc5763aeef9ff52", new PusherOptions());

            _pusher.Connected += pusher_Connected;
            _pusher.ConnectionStateChanged += _pusher_ConnectionStateChanged;
            _pusher.Connect();
        }

        static void _pusher_ConnectionStateChanged(object sender, ConnectionState state)
        {
            Console.WriteLine("Connection state: " + state.ToString());
        }

        static void pusher_Connected(object sender)
        {
            _chatChannel = _pusher.Subscribe("order_book");
            _chatChannel.Subscribed += _chatChannel_Subscribed;
        }

        static void _chatChannel_Subscribed(object sender)
        {
            _chatChannel.Bind("data", (dynamic data) =>
            {
                //Skim Top:
                var topBidAr = ((Newtonsoft.Json.Linq.JArray)data.bids)[0];
                var topBid = new Tick() { Price = topBidAr.First.ToObject<decimal>(), Volume = topBidAr.Last.ToObject<decimal>() };

                var topAskAr = ((Newtonsoft.Json.Linq.JArray)data.asks)[0];
                var topAsk = new Tick() { Price = topAskAr.First.ToObject<decimal>(), Volume = topAskAr.Last.ToObject<decimal>() };

                Console.WriteLine("Top Bids: " + topBid);
                Console.WriteLine("Top Ask: " + topAsk);
                _pusher.Disconnect();
            });
        }

        public struct Tick
        {
            public decimal Price;
            public decimal Volume;

            public override string ToString()
            {
                return string.Format("{0} BTC @ {1} USD", Volume, Price);
            }
        }
    }
}
