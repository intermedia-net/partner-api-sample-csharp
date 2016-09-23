// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentCreditCardModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Payment
{
    /// <summary>
    /// The payment credit card model.
    /// </summary>
    internal sealed class PaymentCreditCardModel
    {
        /// <summary>
        /// Gets or sets the credit card type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the credit card number.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit card issue number.
        /// </summary>
        public string IssueNumber { get; set; }

        /// <summary>
        /// Gets or sets the credit card start date.
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or sets the credit card expiration date.
        /// </summary>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the credit card security code.
        /// </summary>
        public string SecurityCode { get; set; }
    }
}