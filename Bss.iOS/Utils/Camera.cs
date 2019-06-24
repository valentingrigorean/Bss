//
// CameraUtils.cs
//
// Author:
//       Valentin Grigorean <v.grigorean@software-dep.net>
//
// Copyright (c) 2016 
//
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
using System;
using System.Collections.Generic;
using Bss.iOS.Models;
using UIKit;
using System.IO;
namespace Bss.iOS.Utils
{
    public static class Camera
    {
        private static UIViewController RootViewController => Application.RootNavigationController;
        //private static Lazy<UIImagePickerController> Instance = new Lazy<UIImagePickerController>(
        //    () => new UIImagePickerController());


        public static string Title { get; set; } = "Pick Source";
        public static string SCamera { get; set; } = "Camera";
        public static string PhotoLibrary { get; set; } = "Photo Library";
        public static bool AllowEditing { get; set; } = false;
        public static bool AllowImageEditing { get; set; } = false;

        /// <summary>
        /// Takes the picture.
        /// Needs:
        /// -NSCameraUsageDescription key set in plist for camera
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        public static void TakePicture(UIViewController parent, Action<UIImage> callback)
        {
            PickImageInternal(parent, UIImagePickerControllerSourceType.Camera, callback);
        }

        /// <summary>
        /// Takes the picture resizes, then saves it locally
        /// Needs:
        /// -NSCameraUsageDescription key set in plist for camera
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        public static void TakeAndResizePictureWithFilename(UIViewController parent, int maxSize, int index, Action<UIImage, string> callback, bool temp = false)
        {
            PickAndResizeImageInternalWithFilename(parent, maxSize, index, UIImagePickerControllerSourceType.Camera, callback, temp);
        }

        /// <summary>
        /// Selects the picture.
        /// Needs:
        /// -NSCameraUsageDescription key set in plist for library
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        public static void SelectPicture(UIViewController parent, Action<UIImage> callback)
        {
            PickImageInternal(parent, UIImagePickerControllerSourceType.PhotoLibrary, callback);
        }

        public static void SelectPicture(UIViewController parent, Action<UIImage> callback, UIView container, int maxSize = 0)
        {
            PickImageInternal(parent, UIImagePickerControllerSourceType.PhotoLibrary, callback, container, maxSize);
        }

        /// <summary>
        /// Selects the picture, resizes, then saves it locally
        /// Needs:
        /// -NSCameraUsageDescription key set in plist for library
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        public static void SelectAndResizePictureWithFilename(UIViewController parent, int maxSize, int index, Action<UIImage, string> callback, bool temp = false)
        {
            PickAndResizeImageInternalWithFilename(parent, maxSize, index, UIImagePickerControllerSourceType.PhotoLibrary, callback, temp);
        }

        /// <summary>
        /// Shows the options. to select picture from
        /// Needs:
        /// -NSCameraUsageDescription key set in plist for camera
        /// -NSPhotoLibraryUsageDescription key set in plist for library
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="actions">Actions.</param>
        public static void ShowOptions(UIViewController parent, Action<UIImage> callback, IList<ImagePicker> actions = null)
        {
            ShowImageActions(parent, action =>
            {
                if (action.SourceType == CImagePicker.Custom)
                {
                    action.Action?.Invoke(_ => callback?.Invoke(_));
                    return;
                }
                PickImageInternal(parent, (UIImagePickerControllerSourceType)action.SourceType, callback);
            }, actions);
        }

        private static void PickImageInternal(UIViewController parent, UIImagePickerControllerSourceType source, Action<UIImage> callback, UIView container = null, int maxSize = 0)
        {
            var imagePicker = new UIImagePickerCustom();
            imagePicker.AllowsEditing = AllowEditing;
            imagePicker.AllowsImageEditing = AllowImageEditing;

            EventHandler<UIImagePickerMediaPickedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                imagePicker.FinishedPickingMedia -= handler;
                imagePicker.DismissViewController(true, () => callback?.Invoke(maxSize == 0 ? e.OriginalImage : e.OriginalImage.Resize(maxSize, BContentMode.ScaleToFit)));
            };

            imagePicker.FinishedPickingMedia += handler;

            imagePicker.SourceType = source;
            imagePicker.Canceled += (sender, e) => imagePicker.DismissViewController(true, null);

            //prevent crash on iOS 9 and below
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                imagePicker.ModalPresentationStyle = UIModalPresentationStyle.Popover;
                imagePicker.PopoverPresentationController.SourceView = container;
                imagePicker.PopoverPresentationController.SourceRect = container.Bounds;
            }

            parent.PresentViewController(imagePicker, true, null);
        }


        private static void PickAndResizeImageInternalWithFilename(UIViewController parent, int maxSize, int index, UIImagePickerControllerSourceType source, Action<UIImage, string> callback, bool temp = false)
        {
            var imagePicker = new UIImagePickerController();
            imagePicker.AllowsEditing = AllowEditing;
            imagePicker.AllowsImageEditing = AllowImageEditing;

            EventHandler<UIImagePickerMediaPickedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                imagePicker.FinishedPickingMedia -= handler;
                imagePicker.DismissViewController(true, null);

                var resizedImage = e.OriginalImage.Resize(maxSize, BContentMode.ScaleToFit);

                var documentsDirectory = Environment.GetFolderPath
                                (Environment.SpecialFolder.Personal);

                var jpgFilename = System.IO.Path.Combine(documentsDirectory, "TempPhoto" + index.ToString() + ".jpg");

                if (temp == true)
                {
                    documentsDirectory = Environment.GetFolderPath
                         (Environment.SpecialFolder.MyDocuments);
                    documentsDirectory = Path.Combine(documentsDirectory, "..", "tmp");

                    jpgFilename = System.IO.Path.Combine(documentsDirectory, $"img_{DateTime.Now.ToString("yyyyMd_HHms")}" + ".jpg");
                }

                resizedImage.SaveToFile(jpgFilename, () =>
                {
                    var fileExists = File.Exists(jpgFilename);
                    parent.InvokeOnMainThread(() =>
                    {
                        callback?.Invoke(resizedImage, fileExists ? jpgFilename : "");
                    });
                }, ImageExtension.JPG);
            };

            imagePicker.FinishedPickingMedia += handler;

            imagePicker.SourceType = source;
            imagePicker.Canceled += (sender, e) => imagePicker.DismissViewController(true, null);
            parent.PresentViewController(imagePicker, true, null);
        }

        private static void ShowImageActions(UIViewController parent, Action<ImagePicker> callback,
                                    IList<ImagePicker> actions = null)
        {
            var actionSheetAlert = UIAlertController.Create(Title, "", UIAlertControllerStyle.ActionSheet);
            if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
                actionSheetAlert.AddAction(UIAlertAction.Create(SCamera, UIAlertActionStyle.Default, obj =>
                    callback(new ImagePicker
                    {
                        SourceType = CImagePicker.Camera
                    })));
            actionSheetAlert.AddAction(UIAlertAction.Create(PhotoLibrary, UIAlertActionStyle.Default, obj =>
                callback(new ImagePicker
                {
                    SourceType = CImagePicker.PhotoLibrary
                })));

            if (actions != null)
                foreach (var action in actions)
                    actionSheetAlert.AddAction(UIAlertAction.Create(action.Name, UIAlertActionStyle.Default, obj =>
                        callback(action)));

            actionSheetAlert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            parent.PresentViewController(actionSheetAlert, true, null);
        }

        private class UIImagePickerCustom : UIImagePickerController
        {
            public UIImagePickerCustom()
            {
                ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
            }

            public override bool ShouldAutorotate() => false;
            public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations() => UIInterfaceOrientationMask.All;
        }

    }
}
