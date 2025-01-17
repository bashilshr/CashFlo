﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CashFlo.Model
{
    public class User
    {
        public string? Username { get; set; }
        public Currency Currency { get; set; }  // Currency is an enum
        public string? Password { get; set; }
        public decimal PrimaryBalance { get; set; }
        public string? Salt { get; set; }
    }
}
    