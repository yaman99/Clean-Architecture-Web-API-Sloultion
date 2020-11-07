using System;
using System.Collections.Generic;
using System.Text;

namespace Adsbility.Appilication.Test.Queries
{
    public class TestVm
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int count => id + 1;
    }
}
