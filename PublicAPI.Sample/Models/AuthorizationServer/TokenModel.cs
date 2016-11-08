// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenModel.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample.Models.AuthorizationServer
{
    using Newtonsoft.Json;

    /// <summary>
    /// The token model.
    /// </summary>
    internal sealed class TokenModel
    {
        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        /// <remarks>
        /// Currently, 'Bearer' is the only supported token type.
        /// </remarks>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the token expiration in seconds.
        /// </summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the access token value.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}