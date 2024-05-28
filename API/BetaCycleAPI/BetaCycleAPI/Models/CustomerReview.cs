using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BetaCycleAPI.Models;

public partial class CustomerReview
{
    [Key]
    public int ReviewId { get; set; }

    [Required]
    [MaxLength(500), MinLength(4)]
    public string BodyDescription { get; set; } = null!;

    [Required]
    public byte Rating { get; set; }

    public DateTime? ReviewDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

}
