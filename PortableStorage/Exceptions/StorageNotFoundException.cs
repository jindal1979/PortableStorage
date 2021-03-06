﻿using System;
using System.IO;

namespace PortableStorage.Exceptions
{
    public class StorageNotFoundException : FileNotFoundException
    {
        public StorageNotFoundException(Uri storageUri, string entryName)
            : base($"Storage Entry not found! storageUri: {storageUri}, entryName:{entryName}")
        {
        }

        public StorageNotFoundException(Uri uri)
            : base($"Storage Entry not found! Uri: {uri}")
        {
        }

        public StorageNotFoundException()
        {
        }

        public StorageNotFoundException(string message) : base(message)
        {
        }

        public StorageNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
