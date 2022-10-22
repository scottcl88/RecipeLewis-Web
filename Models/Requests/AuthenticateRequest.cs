﻿using System.ComponentModel.DataAnnotations;

namespace RecipeLewis.Models.Requests;

public class AuthenticateRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}