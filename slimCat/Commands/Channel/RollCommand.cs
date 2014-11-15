﻿#region Copyright

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollCommand.cs">
//     Copyright (c) 2013, Justin Kadrovach, All rights reserved.
//  
//     This source is subject to the Simplified BSD License.
//     Please see the License.txt file for more information.
//     All other rights reserved.
// 
//     THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//     KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//     IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//     PARTICULAR PURPOSE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#endregion

namespace slimCat.Services
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Utilities;

    #endregion


    public partial class UserCommandService
    {
        private void OnRollRequested(IDictionary<string, object> command)
        {
            var targetName = command.Get(Constants.Arguments.Channel);
            var target = model.CurrentChannels
                .Cast<ChannelModel>()
                .Union(model.CurrentPms)
                .FirstOrDefault(x => x.Id.Equals(targetName, StringComparison.OrdinalIgnoreCase));

            var targetType = target.Type;
            if (targetType == ChannelType.PrivateMessage)
            {
                command.Add(Constants.Arguments.Recipient, targetName);
                command.Remove(Constants.Arguments.Channel);
            }
            connection.SendMessage(command);
        }
    }

    public partial class ServerCommandService
    {
        private void RollCommand(IDictionary<string, object> command)
        {
            object channel;
            object recipient;
            command.TryGetValue(Constants.Arguments.Channel, out channel);
            command.TryGetValue(Constants.Arguments.Recipient, out recipient);
            var message = command.Get(Constants.Arguments.Message);
            var poster = command.Get(Constants.Arguments.Character);

            if (channel == null)
                channel = recipient;

            if (!CharacterManager.IsOnList(poster, ListKind.Ignored))
                manager.AddMessage(message, (string)channel, poster, MessageType.Roll);
        }
    }
}