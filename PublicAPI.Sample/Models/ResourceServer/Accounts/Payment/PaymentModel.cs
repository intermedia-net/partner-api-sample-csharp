// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Payment
{
    using Address;

    /// <summary>
    /// The payment model.
    /// </summary>
    internal sealed class PaymentModel : PaymentGetModel
    {
        /// <summary>
        /// Gets or sets the account payment name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account payment phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the account payment address.
        /// </summary>
        public AddressModel Address { get; set; }

        /// <summary>
        /// Gets or sets the account credit card payment.
        /// </summary>
        public PaymentCreditCardModel CreditCard { get; set; }

        /// <summary>
        /// Gets or sets the account electronic check payment.
        /// </summary>
        public PaymentElectronicCheckModel ElectronicCheck { get; set; }
    }
}