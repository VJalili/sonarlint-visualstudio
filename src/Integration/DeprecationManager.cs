﻿/*
 * SonarLint for Visual Studio
 * Copyright (C) 2016-2018 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using SonarLint.VisualStudio.Integration.InfoBar;

namespace SonarLint.VisualStudio.Integration
{
    public sealed class DeprecationManager : IDisposable
    {
        private const string VisualStudio2015Update3VersionString = "14.0.25420.00";
        private static readonly Version VisualStudio2015Update3Version = Version.Parse(VisualStudio2015Update3VersionString);
        internal /* for testing purpose */ static readonly Guid DeprecationBarGuid = new Guid(ToolWindowGuids80.ErrorList);

        private readonly IInfoBarManager infoBarManager;
        private readonly ISonarLintOutput sonarLintOutput;
        private IInfoBar deprecationBar;

        public DeprecationManager(IInfoBarManager infoBarManager, ISonarLintOutput sonarLintOutput)
        {
            if (infoBarManager == null)
            {
                throw new ArgumentNullException(nameof(infoBarManager));
            }
            if (sonarLintOutput == null)
            {
                throw new ArgumentNullException(nameof(sonarLintOutput));
            }

            this.infoBarManager = infoBarManager;
            this.sonarLintOutput = sonarLintOutput;
        }

        public void Initialize(string visualStudioVersion)
        {
            Version vsVersion;
            if (Version.TryParse(visualStudioVersion, out vsVersion) &&
                vsVersion < VisualStudio2015Update3Version)
            {
                WriteMessageToOutput();
                ShowDeprecationBar();
            }
        }

        private void WriteMessageToOutput()
        {
            const string message =
                "*****************************************************************************************\r\n" +
                "***   Newer versions of SonarLint will not work with this version of Visual Studio.   ***\r\n" +
                "***   Please update to Visual Studio 2015 Update 3 or Visual Studio 2017 to benefit   ***\r\n" +
                "***   from new features.                                                              ***\r\n" +
                "*****************************************************************************************";

            sonarLintOutput.Write(message);
        }

        private void ShowDeprecationBar()
        {
            const string message = "Newer versions of SonarLint will not work with this version of Visual Studio. Please " +
                "update to Visual Studio 2015 Update 3 or Visual Studio 2017 to benefit from new features.";
            deprecationBar = infoBarManager.AttachInfoBar(DeprecationBarGuid, message, default(ImageMoniker));
        }

        public void Dispose()
        {
            deprecationBar?.Close();
            deprecationBar = null;
        }
    }
}
