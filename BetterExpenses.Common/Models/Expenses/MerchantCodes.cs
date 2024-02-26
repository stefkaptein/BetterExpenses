// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;

namespace BetterExpenses.Common.Models.Expenses;

public static class MerchantCodes
{
    /// <summary>
    /// Variety Stores
    /// </summary>
    [Display(Name = "MCC 5331", Description = "Variety Stores")]
    public const int MCC5331 = 5331;
    
    /// <summary>
    /// Grocery Stores, Supermarkets
    /// </summary>
    [Display(Name = "MCC 5411", Description = "Grocery Stores, Supermarkets")]
    public const int MCC5411 = 5411;
    
    /// <summary>
    /// Miscellaneous Food Stores - Convenience Stores, Markets, Specialty Stores
    /// </summary>
    [Display(Name = "MCC 5499", Description = "Miscellaneous Food Stores - Convenience Stores, Markets, Specialty Stores")]
    public const int MCC5499 = 5499;

    /// <summary>
    /// Other Services - not elsewhere classified
    /// </summary>
    [Display(Name = "MCC 7299", Description = "Other Services - not elsewhere classified")]
    public const int MCC7299 = 7299;
}