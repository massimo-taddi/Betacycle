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
}
