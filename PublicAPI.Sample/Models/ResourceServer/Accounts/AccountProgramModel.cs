// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountProgramModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.ResourceServer.Accounts
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The account program model.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountProgramModel
    {
        /// <summary>
        /// The partner account.
        /// </summary>
        [EnumMember(Value = "partner")]
        Partner,

        /// <summary>
        /// The end-user account.
        /// </summary>
        [EnumMember(Value = "account")]
        Account
    }
}