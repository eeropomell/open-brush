﻿// Copyright 2020 The Tilt Brush Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine.Localization;
using UnityEngine;

namespace TiltBrush
{

    public class LoadReferenceImageButton : BaseButton
    {
        public ReferenceImage ReferenceImage { get; set; }

        [SerializeField] private LocalizedString m_ErrorHelpText;


        // this is commented out and moved into ResetState() because
        // for a new image (ie., one that isn't in the cache yet), !ReferenceImage.Valid, even if the size is valid (ie less than the max size)
        // TODO: figure out if it's bad to move this into ResetState()
        public void RefreshDescription()
        {
            /* if (ReferenceImage != null)
             {

                 if (!ReferenceImage.Valid)
                 {
                     SetDescriptionText(App.ShortenForDescriptionText(ReferenceImage.FileName), ImageErrorExtraDescription());
                 }
                 else
                 {
                     SetDescriptionText(App.ShortenForDescriptionText(ReferenceImage.FileName));
                 }

             }*/
        }

        override protected void OnButtonPressed()
        {
            if (ReferenceImage == null || !ReferenceImage.Valid)
            {
                return;
            }

            if (ReferenceImage.NotLoaded)
            {
                // Load-on-demand.
                ReferenceImage.SynchronousLoad();
            }
            else
            {
                CreateWidgetCommand command = new CreateWidgetCommand(
                    WidgetManager.m_Instance.ImageWidgetPrefab, TrTransform.FromTransform(transform), null,
                    false, SelectionManager.m_Instance.SnappingGridSize, SelectionManager.m_Instance.SnappingAngle
                );
                SketchMemoryScript.m_Instance.PerformAndRecordCommand(command);
                ImageWidget widget = command.Widget as ImageWidget;
                widget.ReferenceImage = ReferenceImage;
                SketchControlsScript.m_Instance.EatGazeObjectInput();
                SelectionManager.m_Instance.RemoveFromSelection(false);
            }
        }

        override public void ResetState()
        {
            base.ResetState();

            if (ReferenceImage == null)
            {
                return;
            }

            if (!ReferenceImage.Valid)
            {
                SetDescriptionText(App.ShortenForDescriptionText(ReferenceImage.FileName), ImageErrorExtraDescription());
            }
            else
            {
                SetDescriptionText(App.ShortenForDescriptionText(ReferenceImage.FileName));
            }
        }

        public string ImageErrorExtraDescription()
        {
            return m_ErrorHelpText.GetLocalizedStringAsync().Result;
        }
    }
} // namespace TiltBrush
