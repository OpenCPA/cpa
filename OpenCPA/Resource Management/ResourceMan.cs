using Nancy;
using OpenCPA.Data;
using OpenCPA.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace OpenCPA
{
    /// <summary>
    /// Resource manager for the OpenCPA.
    /// </summary>
    public static class ResourceMan
    {
        private static string imageDirectory;
        private static string audioDirectory;

        /// <summary>
        /// Initializes resource folders and properties.
        /// </summary>
        public static void Initialize()
        {
            //Create the resource folders.
            string baseRes = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources");
            imageDirectory = Path.Combine(baseRes, "image");
            audioDirectory = Path.Combine(baseRes, "audio");

            Directory.CreateDirectory(baseRes);
            Directory.CreateDirectory(imageDirectory);
            Directory.CreateDirectory(audioDirectory);
        }

        /// <summary>
        /// Attempts to download a resource form a URL and store it, returning a GUID if successful.
        /// Returns null on fail.
        /// THIS SHOULD ONLY EVER RUN FOR AUTHORIZED USERS! DON'T LET THIS INTO USERLAND.
        /// </summary>
        public static string DownloadResourceFromURL(string URL, ResourceType type)
        {
            //Make a filepath for this resource.
            string guid = Guid.NewGuid().ToString();
            string filePath = GetResourceFilePath(guid, type);
            if (filePath == null) { return null; }

            //Attempt to download the resource into a file.
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(URL, filePath);
                }
            }
            catch { return null; }

            //Log this resource in the database.
            DBMan.Instance.Insert(new Resource()
            {
                GUID = guid,
                Type = type
            });

            //Return the resource GUID.
            return guid;
        }

        /// <summary>
        /// Gets the file path of any given resource with GUID, given its type.
        /// </summary>
        public static string GetResourceFilePath(string guid, ResourceType type)
        {
            if (guid == "" || guid == null) { return null; }
            switch (type)
            {
                case ResourceType.AUDIO:
                    return Path.Combine(audioDirectory, guid);
                case ResourceType.IMAGE:
                    return Path.Combine(imageDirectory, guid);
                default:
                    //Unrecognized resource type!
                    Console.WriteLine("[WARN] Unrecognized resource type attempted to be saved: " + type.ToString());
                    return null;
            }
        }

        /// <summary>
        /// Attempts to delete a resource with a given GUID.
        /// </summary>
        public static bool DeleteResource(string guid)
        {
            //Ignore invalid GUIDs.
            if (guid == null || guid == "") { return true; }

            //Get the database entry, delete from file record.
            var res = DBMan.Instance.Query<Resource>("SELECT * FROM Resources WHERE GUID=?", guid);
            if (res.Count == 0)
            {
                //Never existed.
                return true;
            }

            //Okay, get the first resource and delete.
            string file = GetResourceFilePath(res[0].GUID, res[0].Type);
            try
            {
                File.Delete(file);
            }
            catch { return false; }

            //Remove database entry.
            DBMan.Instance.Delete(res[0]);
            return true;
        }

        /// <summary>
        /// Creates a resource file from POST data.
        /// </summary>
        internal static string MakeResourceFromPOST(HttpFile file, ResourceType type)
        {
            //Get the file path.
            string guid = Guid.NewGuid().ToString();
            string filePath = GetResourceFilePath(guid, type);

            //Create and write to the file.
            try
            {
                using (var fileStream = File.Create(filePath))
                {
                    file.Value.Seek(0, SeekOrigin.Begin);
                    file.Value.CopyTo(fileStream);
                }
            }
            catch { return null; }

            //Create the resource database side.
            DBMan.Instance.Insert(new Resource()
            {
                GUID = guid,
                Type = type
            });

            return guid;
        }
    }
}