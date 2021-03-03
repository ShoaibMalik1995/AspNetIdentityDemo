using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AspNetIdentityDemo.Shared.Enums
{
    public enum Permission
    {
        ADMIN = 0,
        CREATE = 1,
        UPDATE = 2,
        DELETE = 3,
        VIEW = 4
    }

    public static class Module
    {
        //USER_MANAGEMENT,
        //INVENTORY_MANAGEMENT,
        //SALES_REP,
        //REPORTS,
        //PRODUCTION_PIPELINE,
        //POS,

        public const string USER_MANAGEMENT = "User Management";
        public const string INVENTORY_MANAGEMENT = "Inventory ";

    }
}
