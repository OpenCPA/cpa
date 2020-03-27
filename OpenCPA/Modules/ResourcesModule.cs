using Nancy;
using OpenCPA.Data;
using OpenCPA.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OpenCPA.Modules
{
    public class ResourcesModule : NancyModule
    {
        public ResourcesModule()
        {
            //When a resource is attempted to be grabbed.
            Get("/resources/{guid}", (req) =>
            {
                //Get the resource from the database.
                Resource resource = DBMan.Instance.FindWithQuery<Resource>("SELECT * FROM Resources WHERE GUID=?", req.guid.ToString());
                if (resource == null)
                {
                    //Invalid resource, 404.
                    return new Response
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = "That resource does not exist."
                    };
                }

                //Return file, depending on resource type.
                string path = ResourceMan.GetResourceFilePath(resource.GUID, resource.Type);
                if (path == null)
                {
                    //Invalid resource, 404.
                    return new Response
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = "That resource does not exist."
                    };
                }

                //Make the file path relative.
                var fileUri = new Uri(path);
                var referenceUri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
                string relativePath = referenceUri.MakeRelativeUri(fileUri).ToString();

                //Determine the mime type using byte examination or extension.
                string mimeType = MimeType.GetMimeType(File.ReadAllBytes(path), new FileInfo(path).Name);

                //Return response.
                return Response.AsFile(relativePath, mimeType);
            });
        }
    }
}