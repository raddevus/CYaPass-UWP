﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYaPass
{
    interface IPersistable
    {
        Task<bool> Save();
    }
}
