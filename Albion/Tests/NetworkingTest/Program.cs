using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NetworkingTest
{
    class Program
    {
        static void Main(string[] args)
        {

            new TcpServerExample().Run(5555);
        }
    }
}
