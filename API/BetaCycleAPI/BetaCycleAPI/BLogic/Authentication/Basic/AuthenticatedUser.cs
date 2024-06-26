﻿using System.Security.Principal;

namespace BetaCycleAPI.BLogic.Authentication.Basic
{
    public class AuthenticatedUser : IIdentity
    {
        public AuthenticatedUser(string authType, bool isAuthenticated, string name)
        {
            AuthenticationType = authType;
            IsAuthenticated = isAuthenticated;
            Name = name;
        }
        public string? AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string? Name { get; set; }

        public bool IsAdmin { get; set; }
    }
}
