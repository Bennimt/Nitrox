﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroxModel.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitroxTest.Model.Packets
{
    [TestClass]
    public class PacketsSerializableTest
    {
        private HashSet<Type> visitedTypes = new HashSet<Type>();

        public void IsSerializable(Type t)
        {
            if (visitedTypes.Contains(t))
                return;

            Assert.IsTrue(t.IsSerializable, $"Type {t} is not serializable!");

            visitedTypes.Add(t);

            // Recursively check all properties and fields, because IsSerializable only checks if the current type is a primitive or has the [Serializable] attribute.
            t.GetProperties().Select(tt => tt.PropertyType).ForEach(IsSerializable);
            t.GetFields().Select(tt => tt.FieldType).ForEach(IsSerializable);
        }

        [TestMethod]
        public void TestAllPacketsAreSerializable()
        {
            typeof(Packet).Assembly.GetTypes()
                .Where(p => typeof(Packet).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .ForEach(IsSerializable);
        }
    }
}
