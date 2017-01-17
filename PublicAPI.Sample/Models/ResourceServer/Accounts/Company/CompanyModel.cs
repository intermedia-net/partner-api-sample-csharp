// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Company
{
    using Address;

    /// <summary>
    /// The company model.
    /// </summary>
    internal sealed class CompanyModel
    {
        /// <summary>
        /// Gets or sets the account company name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account company phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the account company address.
        /// </summary>
        public AddressModel Address { get; set; }
    }
}