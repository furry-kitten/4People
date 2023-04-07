using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4People.Database
{
    public static class Constants
    {
        public static readonly string CreatingString = ConfigurationManager.ConnectionStrings["CreatingString"].ConnectionString;
    }
}
