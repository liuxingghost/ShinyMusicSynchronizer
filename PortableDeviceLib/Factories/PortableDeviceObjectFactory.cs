﻿using System;
using System.Collections.Generic;
using PortableDeviceApiLib;
using PortableDeviceLib.Model;

namespace PortableDeviceLib.Factories
{
    /// <summary>
    ///     Represent a factory that can build <see cref="PortableDeviceObject" />
    ///     You can register new method to handle new object types
    /// </summary>
    public class PortableDeviceObjectFactory
    {
        public delegate PortableDeviceObject FactoryMethodType(IPortableDeviceValues values);

        private static PortableDeviceObjectFactory instance;
        private readonly Dictionary<Guid, FactoryMethodType> factoryMethods;

        /// <summary>
        ///     Initialize a new instance of the <see cref="PortableDeviceObjectFactory" /> class
        ///     This is a private constructor to support Singleton pattern
        /// </summary>
        private PortableDeviceObjectFactory()
        {
            factoryMethods = new Dictionary<Guid, FactoryMethodType>();

            // Register know object type
            RegisterNewFactoryMethod(PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER, CreateFolderObject);
            RegisterNewFactoryMethod(PortableDeviceGuids.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT, CreateFunctionalObject);
        }

        #region Properties

        /// <summary>
        ///     Gets the unique instance of factory
        /// </summary>
        public static PortableDeviceObjectFactory Instance
        {
            get { return instance ?? (instance = new PortableDeviceObjectFactory()); }
        }

        #endregion

        #region Public functions

        /// <summary>
        ///     Register a new factory method that enable create new object type
        /// </summary>
        /// <param name="handledType"></param>
        /// <param name="method"></param>
        public void RegisterNewFactoryMethod(Guid handledType, FactoryMethodType method)
        {
            if (handledType == Guid.Empty)
                throw new ArgumentException("handledType cann't be Guid.Empty", "handledType");
            if (method == null)
                throw new ArgumentNullException("method");

            if (factoryMethods.ContainsKey(handledType))
                throw new ArgumentException(string.Format("Guid {0} is already registered", handledType), "handledType");

            factoryMethods.Add(handledType, method);
        }

        /// <summary>
        ///     Create a new instance of a portableDeviceObject or derived from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PortableDeviceObject CreateInstance(Guid type, IPortableDeviceValues values)
        {
            if (factoryMethods.ContainsKey(type))
                return factoryMethods[type](values);
            else
                return CreateGenericObject(values);
        }

        #endregion

        #region Private functions

        private static string GetObjectId(IPortableDeviceValues values)
        {
            string value;
            values.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_ID, out value);
            return value;
        }

        private PortableDeviceObject CreateFunctionalObject(IPortableDeviceValues values)
        {
            var obj = CreateObject<PortableDeviceFunctionalObject>(values);

            Guid category;
            values.GetGuidValue(ref PortableDevicePKeys.WPD_FUNCTIONAL_OBJECT_CATEGORY, out category);
            obj.Category = category;

            return obj;
        }

        private PortableDeviceObject CreateFolderObject(IPortableDeviceValues values)
        {
            var obj = CreateObject<PortableDeviceFolderObject>(values);
            return obj;
        }

        private T CreateObject<T>(IPortableDeviceValues values) where T : PortableDeviceObject
        {
            var obj = (T) Activator.CreateInstance(typeof (T), GetObjectId(values));
            InitializeInstance(obj, values);
            return obj;
        }

        private PortableDeviceObject CreateGenericObject(IPortableDeviceValues values)
        {
            string filename;
            int size;
            try
            {
                values.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, out filename);
                values.GetSignedIntegerValue(ref PortableDevicePKeys.WPD_OBJECT_SIZE, out size);
            }
            catch
            {
                return CreateObject<PortableDeviceObject>(values);
            }

            var obj = CreateObject<PortableDeviceFileObject>(values);
            obj.FileName = filename;
            obj.Size = size;
            return obj;
        }

        private static void InitializeInstance(PortableDeviceObject obj, IPortableDeviceValues values)
        {
            string name;
            values.GetStringValue(ref PortableDevicePKeys.WPD_OBJECT_NAME, out name);

            Guid guid;
            values.GetGuidValue(ref PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, out guid);
            string contentType = PortableDeviceHelpers.GetKeyNameFromGuid(guid);

            values.GetGuidValue(ref PortableDevicePKeys.WPD_OBJECT_FORMAT, out guid);
            string formatType = PortableDeviceHelpers.GetKeyNameFromGuid(guid);

            obj.Name = name;
            obj.ContentType = contentType;
            obj.Format = formatType;
        }

        #endregion
    }
}