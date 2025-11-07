// MIT License
// Copyright (c) 2025 AceRust
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("Random Seed Rotator", "AceRust", "1.0.0")]
    [Description("Automatically rotates or randomizes Rust map seeds on wipe or restart, keeping the server world fresh.")]
    public class RandomSeedRotator : CovalencePlugin
    {
        #region Configuration

        private const int WorldSize = 4500; // Fixed world size
        private readonly List<int> SeedList = new List<int>
        {
            2083415640, 488159760, 372604177, 330716258, 1167635348, 143115178,
            221406951, 733943327, 347396879, 164236192, 277121595, 717509877,
            546489524, 469431581, 432339362, 442956213, 364490646, 154690547
        };

        private int _lastSeed = -1;

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["SeedSelected"] = "[RandomSeedRotator] Selected map seed: {0} | World Size: {1}",
                ["SeedRepeat"] = "[RandomSeedRotator] Seed repeated from last wipe: {0}"
            }, this);
        }

        #endregion

        private void OnServerInitialized()
        {
            if (SeedList.Count == 0)
            {
                PrintWarning("Seed list is empty — please add some seeds to the plugin file.");
                return;
            }

            int randomIndex = UnityEngine.Random.Range(0, SeedList.Count);
            int selectedSeed = SeedList[randomIndex];

            // Prevent same seed from being selected twice in a row
            if (selectedSeed == _lastSeed && SeedList.Count > 1)
            {
                randomIndex = (randomIndex + 1) % SeedList.Count;
                selectedSeed = SeedList[randomIndex];
            }

            ConVar.Server.level = "Procedural Map";
            ConVar.Server.worldsize = WorldSize;
            ConVar.Server.seed = selectedSeed;

            _lastSeed = selectedSeed;

            Puts(lang.GetMessage("SeedSelected", this, null), selectedSeed, WorldSize);
        }

        private void Unload()
        {
            Puts("[RandomSeedRotator] Plugin unloaded — seed rotation halted.");
        }
    }
}
