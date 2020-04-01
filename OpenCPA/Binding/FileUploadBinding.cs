using Nancy;
using Nancy.ModelBinding;
using OpenCPA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace OpenCPA
{
    /// <summary>
    /// Adds a new track to the request binder.
    /// </summary>
    public class AddTrackRequestBinder : Nancy.ModelBinding.IModelBinder
    {
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var fileUploadRequest = (instance as AddTrackModel) ?? new AddTrackModel();

            var form = context.Request.Form;

            fileUploadRequest.EnglishName = form["EnglishName"];
            fileUploadRequest.NativeName = form["NativeName"];
            fileUploadRequest.Length = form["Length"];
            fileUploadRequest.File = GetFileByKey(context, "File");

            return fileUploadRequest;
        }

        private IList<string> GetTags(dynamic field)
        {
            try
            {
                var tags = (string)field;
                return tags.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                return new List<string>();
            }
        }

        private HttpFile GetFileByKey(NancyContext context, string key)
        {
            IEnumerable<HttpFile> files = context.Request.Files;
            if (files != null)
            {
                return files.FirstOrDefault(x => x.Key == key);
            }
            return null;
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(AddTrackModel);
        }
    }
}