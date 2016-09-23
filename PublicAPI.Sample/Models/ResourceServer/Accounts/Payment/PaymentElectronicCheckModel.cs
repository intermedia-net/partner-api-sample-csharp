// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentElectronicCheckModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Payment
{
    /// <summary>
    /// The payment electronic check model.
    /// </summary>
    internal sealed class PaymentElectronicCheckModel
    {
        /// <summary>
        /// Gets or sets the electronic check account type.
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Gets or sets the electronic check account number.
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the electronic check routing number.
        /// </summary>
        public string RoutingNumber { get; set; }
    }
}