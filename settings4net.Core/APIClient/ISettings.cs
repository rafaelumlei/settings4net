﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using settings4net.Core.APIClient.Models;

namespace settings4net.Core.APIClient
{
    public partial interface ISettings
    {
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='setting'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> AddSettingWithOperationResponseAsync(Setting setting, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='fullpath'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> DeleteSettingWithOperationResponseAsync(string id, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<Setting>>> GetSettingsWithOperationResponseAsync(string app, string env, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='fullpath'>
        /// Required.
        /// </param>
        /// <param name='setting'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<string>> UpdateSettingWithOperationResponseAsync(string app, string env, string fullpath, Setting setting, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    }
}
