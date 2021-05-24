using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ReactNative;
using Microsoft.ReactNative.Managed;
using Windows.Storage.Pickers;

namespace ReactNativeFilePicker
{
    [ReactModule("ReactNativeFilePicker")]
    internal sealed class ReactNativeModule
    {
        // See https://microsoft.github.io/react-native-windows/docs/native-modules for details on writing native modules

        private ReactContext _reactContext;

        [ReactInitializer]
        public void Initialize(ReactContext reactContext)
        {
            _reactContext = reactContext;
        }

        [ReactMethod("pick")]
        public async Task<string> PickFile(string extensions)
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                if (string.IsNullOrEmpty(extensions))
                {
                    openPicker.FileTypeFilter.Add("*");
                }
                else
                {
                    var extensionsList = extensions.Split(",");
                    foreach (var extension in extensionsList)
                    {
                        openPicker.FileTypeFilter.Add(extension);
                    }
                }
                
                var file = await openPicker.PickSingleFileAsync();

                string base64Content = string.Empty;

                var fileStream = await file.OpenReadAsync();

                using (StreamReader reader = new StreamReader(fileStream.AsStream()))
                {
                    using (var memstream = new MemoryStream())
                    {
                        reader.BaseStream.CopyTo(memstream);
                        var bytes = memstream.ToArray();
                        base64Content = Convert.ToBase64String(bytes);
                    }
                }

                return base64Content;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }

        }

        [ReactMethod("pickMultipleFiles")]
        public async Task<List<string>> PickMultipleFiles(string extensions)
        {
            try
            {
                List<string> filesBase64 = new List<string>();
                
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                if (string.IsNullOrEmpty(extensions))
                {
                    openPicker.FileTypeFilter.Add("*");
                }
                else
                {
                    var extensionsList = extensions.Split(",");
                    foreach (var extension in extensionsList)
                    {
                        openPicker.FileTypeFilter.Add(extension);
                    }
                }


                var files = await openPicker.PickMultipleFilesAsync();

                foreach (var file in files)
                {
                    string base64Content = string.Empty;

                    var fileStream = await file.OpenReadAsync();

                    using (StreamReader reader = new StreamReader(fileStream.AsStream()))
                    {
                        using (var memstream = new MemoryStream())
                        {
                            reader.BaseStream.CopyTo(memstream);
                            var bytes = memstream.ToArray();
                            base64Content = Convert.ToBase64String(bytes);

                            filesBase64.Add(base64Content);
                        }
                    }
                }

                return filesBase64;
            }
            catch (Exception exc)
            {
                throw exc; 
            }
        }
    }
}
