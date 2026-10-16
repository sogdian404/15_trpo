using _15_trpo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15_trpo.SingleToneService
{
    class DBService
    {
        private _15trpoContext context;
        public _15trpoContext Context => context;
        private static DBService? instance;
        public static DBService Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBService();
                return instance;
            }
        }
        private DBService()
        {
            context = new _15trpoContext();
        }
    }
}
