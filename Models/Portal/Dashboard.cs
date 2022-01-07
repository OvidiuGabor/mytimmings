using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mytimmings.Models.Portal
{
    public class Dashboard
    {

        public Models.Security.User user { get; }


        public Dashboard(Security.User user)
        {
            this.user = user;
        }

    }
}