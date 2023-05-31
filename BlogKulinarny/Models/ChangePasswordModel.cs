using System.ComponentModel.DataAnnotations;

namespace BlogKulinarny.Models;

public class ChangePasswordModel
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}