﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeesCollection.ResponseModel.AuthModels
{
    public class AuthModel
    {
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public int AcademicYearId { get; set; }
    }

    public class UserAuthModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int AcademicYearId { get; set; }
    }
}
