using Bunq.Sdk.Model.Core;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using MongoDB.Bson.Serialization.Attributes;

namespace BetterExpenses.Common.Models.Expenses;

public class UserExpense
{
    /// <summary>
    /// The id of the created Payment.
    /// </summary>
    [BsonId, BsonElement("_id")]
    public int Id { get; set; }
    
    /// <summary>
    /// The id of the MonetaryAccount the Payment was made to or from (depending on whether this is an incoming or
    /// outgoing Payment).
    /// </summary>
    public int MonetaryAccountId { get; set; }
    
    /// <summary>
    /// The timestamp when the Payment was done. (In utc format)
    /// </summary>
    public required DateTime Created { get; set; }
    
    /// <summary>
    /// The timestamp when the Payment was last updated (will be updated when chat messages are received).
    /// </summary>
    public required DateTime Updated { get; set; }
    
    /// <summary>
    /// The type of Payment, can be BUNQ, EBA_SCT, EBA_SDD, IDEAL, SWIFT or FIS (card). (I also saw MASTERCARD)
    /// </summary>
    public required string Type { get; set; }
    
    /// <summary>
    /// The sub-type of the Payment, can be PAYMENT, WITHDRAWAL, REVERSAL, REQUEST, BILLING, SCT, SDD or NLO.
    /// </summary>
    public required string SubType { get; set; }
    
    /// <summary>
    /// The Geolocation where the Payment was done from.
    /// </summary>
    public required Geolocation Geolocation { get; set; }
    
    /// <summary>
    /// The Amount transferred by the Payment. Will be negative for outgoing Payments and positive for incoming
    /// Payments (relative to the MonetaryAccount indicated by monetary_account_id).
    /// </summary>
    public required double Amount { get; set; }
    
    /// <summary>
    /// The LabelMonetaryAccount containing the public information of the other (counterparty) side of the Payment.
    /// </summary>
    public required MonetaryAccountReference CounterpartyAlias { get; set; }
    
    /// <summary>
    /// The description for the Payment. Maximum 140 characters for Payments to external IBANs, 9000 characters for
    /// Payments to only other bunq MonetaryAccounts.
    /// </summary>
    public required string Description { get; set; }
}