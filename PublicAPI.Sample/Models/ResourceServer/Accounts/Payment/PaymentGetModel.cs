// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentGetModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Payment
{
    /// <summary>
    /// The payment get model.
    /// </summary>
    internal class PaymentGetModel
    {
        /// <summary>
        /// Gets or sets the account payment type.
        /// </summary>
        public PaymentTypeModel Type { get; set; }
    }
}