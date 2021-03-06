// <copyright file="PortableDeviceCollectionFactory.cs" company="Microsoft">Copyright © Microsoft 2009</copyright>਍ഀ
਍ഀ
using System;਍ഀ
using Microsoft.Pex.Framework;਍ഀ
using PortableDeviceLib;਍ഀ
਍ഀ
namespace PortableDeviceLib਍ഀ
{਍ഀ
    /// <summary>A factory for PortableDeviceLib.PortableDeviceCollection instances</summary>਍ഀ
    public static partial class PortableDeviceCollectionFactory਍ഀ
    {਍ഀ
        /// <summary>A factory for PortableDeviceLib.PortableDeviceCollection instances</summary>਍ഀ
        [PexFactoryMethod(typeof(PortableDeviceCollection))]਍ഀ
        public static object Create()਍ഀ
        {਍ഀ
            return PortableDeviceCollection.Instance;਍ഀ
        }਍ഀ
    }਍ഀ
}਍ഀ
