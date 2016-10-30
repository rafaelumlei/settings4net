﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using settings4net.Core.RemoteRepositories;
using settings4net.Core.RemoteRepositories.APIClient.Models;

namespace settings4net.Core.RemoteRepositories.APIClient
{
    public static partial class SettingsExtensions
    {
        /// <param name='operations'>
        /// Reference to the Settings4net.Core.RemoteRepositories.ISettings.
        /// </param>
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='setting'>
        /// Required.
        /// </param>
        /// <param name='key'>
        /// Required.
        /// </param>
        public static string AddOrUpdateSetting(this ISettings operations, string app, string env, Setting setting, string key)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISettings)s).AddOrUpdateSettingAsync(app, env, setting, key);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <param name='operations'>
        /// Reference to the Settings4net.Core.RemoteRepositories.ISettings.
        /// </param>
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='setting'>
        /// Required.
        /// </param>
        /// <param name='key'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        public static async Task<string> AddOrUpdateSettingAsync(this ISettings operations, string app, string env, Setting setting, string key, CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Microsoft.Rest.HttpOperationResponse<string> result = await operations.AddOrUpdateSettingWithOperationResponseAsync(app, env, setting, key, cancellationToken).ConfigureAwait(false);
            return result.Body;
        }
        
        /// <param name='operations'>
        /// Reference to the Settings4net.Core.RemoteRepositories.ISettings.
        /// </param>
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='setting'>
        /// Required.
        /// </param>
        public static string AddSetting(this ISettings operations, string app, string env, Setting setting)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISettings)s).AddSettingAsync(app, env, setting);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <param name='operations'>
        /// Reference to the Settings4net.Core.RemoteRepositories.ISettings.
        /// </param>
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
        public static async Task<string> AddSettingAsync(this ISettings operations, string app, string env, Setting setting, CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Microsoft.Rest.HttpOperationResponse<string> result = await operations.AddSettingWithOperationResponseAsync(app, env, setting, cancellationToken).ConfigureAwait(false);
            return result.Body;
        }
        
        /// <param name='operations'>
        /// Reference to the Settings4net.Core.RemoteRepositories.ISettings.
        /// </param>
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        public static IList<Setting> GetSettings(this ISettings operations, string app, string env)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISettings)s).GetSettingsAsync(app, env);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <param name='operations'>
        /// Reference to the Settings4net.Core.RemoteRepositories.ISettings.
        /// </param>
        /// <param name='app'>
        /// Required.
        /// </param>
        /// <param name='env'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        public static async Task<IList<Setting>> GetSettingsAsync(this ISettings operations, string app, string env, CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Microsoft.Rest.HttpOperationResponse<System.Collections.Generic.IList<settings4net.Core.RemoteRepositories.APIClient.Models.Setting>> result = await operations.GetSettingsWithOperationResponseAsync(app, env, cancellationToken).ConfigureAwait(false);
            return result.Body;
        }
    }
}
