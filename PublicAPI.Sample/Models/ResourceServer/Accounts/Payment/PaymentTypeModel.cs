// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PaymentTypeModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts.Payment
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The account payment type.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentTypeModel
    {
        /// <summary>
        /// The credit card payment type.
        /// </summary>
        [EnumMember(Value = "creditCard")]
        CreditCard,

        /// <summary>
        /// The electronic check payment type.
        /// </summary>
        [EnumMember(Value = "electronicCheck")]
        ElectronicCheck,

        /// <summary>
        /// The paper check payment type.
        /// </summary>
        [EnumMember(Value = "paperCheck")]
        PaperCheck
    }
}