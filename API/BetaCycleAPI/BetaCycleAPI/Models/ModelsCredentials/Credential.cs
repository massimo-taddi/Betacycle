using System;
using System.Collections.Generic;

namespace BetaCycleAPI.Models.ModelsCredentials;

public partial class Credential
{
    public long CustomerId { get; set; }

    public bool AdminPermission { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string SaltHash { get; set; } = null!;

    public string EmailHash256 { get; set; } = null!;

    public Credential(long customerId, bool adminPermission, string passwordHash, string saltHash, string emailHash256)
    {
        CustomerId = customerId;
        AdminPermission = adminPermission;
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        SaltHash = saltHash ?? throw new ArgumentNullException(nameof(saltHash));
        EmailHash256 = emailHash256 ?? throw new ArgumentNullException(nameof(emailHash256));
    }
}
