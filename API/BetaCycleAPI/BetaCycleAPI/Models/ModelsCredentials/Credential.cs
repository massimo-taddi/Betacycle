using System;
using System.Collections.Generic;

namespace BetaCycleAPI.Models.ModelsCredentials;

public partial class Credential
{
    public long CustomerId { get; set; }

    public bool AdminPermission { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string SaltHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Credential(long customerId, bool adminPermission, string passwordHash, string saltHash, string email)
    {
        CustomerId = customerId;
        AdminPermission = adminPermission;
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        SaltHash = saltHash ?? throw new ArgumentNullException(nameof(saltHash));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}
