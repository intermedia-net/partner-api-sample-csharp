// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddressModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Address
{
    /// <summary>
    /// The account address model.
    /// </summary>
    internal sealed class AddressModel
    {
        /// <summary>
        /// Gets or sets the address country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the address state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the address city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the address street.
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the address ZIP.
        /// </summary>
        public string Zip { get; set; }
    }
}