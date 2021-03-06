﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem.Utilities;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ProjectSystem.VS
{
    [Export(typeof(IProjectSystemOptions))]
    internal class ProjectSystemOptions : IProjectSystemOptions
    {
        [Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")]
        private class SVsSettingsPersistenceManager { }

        private const string FastUpToDateEnabledSettingKey = @"ManagedProjectSystem\FastUpToDateCheckEnabled";
        private const string VerboseFastUpToDateLoggingSettingKey = @"ManagedProjectSystem\VerboseFastUpToDateLogging";

        private readonly IVsOptionalService<SVsSettingsPersistenceManager, ISettingsManager> _settingsManager;
        private readonly IEnvironmentHelper _environment;

        [ImportingConstructor]
        private ProjectSystemOptions(IEnvironmentHelper environment, IVsOptionalService<SVsSettingsPersistenceManager, ISettingsManager> settingsManager)
        {
            Requires.NotNull(environment, nameof(environment));

            _environment = environment;
            _settingsManager = settingsManager;
        }

        public bool IsProjectOutputPaneEnabled
        {
            get
            {
#if DEBUG
                return true;
#else
                return IsEnabled("PROJECTSYSTEM_PROJECTOUTPUTPANEENABLED");
#endif
            }
        }

        public async Task<bool> GetIsFastUpToDateCheckEnabledAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return _settingsManager.Value?.GetValueOrDefault(FastUpToDateEnabledSettingKey, true) ?? true;
        }

        public async Task<bool> GetVerboseFastUpToDateLoggingAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return _settingsManager.Value?.GetValueOrDefault(VerboseFastUpToDateLoggingSettingKey, false) ?? false;
        }

        private bool IsEnabled(string variable)
        {
            string value = _environment.GetEnvironmentVariable(variable);

            return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase);
        }
    }
}
