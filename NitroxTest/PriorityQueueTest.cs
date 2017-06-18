﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroxModel.DataStructures;

namespace NitroxTest
{
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void SameOrder()
        {
            PriorityQueue<String> queue = new PriorityQueue<String>();
            queue.Enqueue(0, "First");
            queue.Enqueue(0, "Second");
            queue.Enqueue(0, "Third");

            Assert.AreEqual("First", queue.Dequeue());
            Assert.AreEqual("Second", queue.Dequeue());
            Assert.AreEqual("Third", queue.Dequeue());
        }

        [TestMethod]
        public void DifferentOrder()
        {
            PriorityQueue<String> queue = new PriorityQueue<String>();
            queue.Enqueue(3, "First");
            queue.Enqueue(2, "Second");
            queue.Enqueue(1, "Third");

            Assert.AreEqual("First", queue.Dequeue());
            Assert.AreEqual("Second", queue.Dequeue());
            Assert.AreEqual("Third", queue.Dequeue());
        }

        [TestMethod]
        public void SomeAreSameOrder()
        {
            PriorityQueue<String> queue = new PriorityQueue<String>();
            queue.Enqueue(2, "First");
            queue.Enqueue(2, "Second");
            queue.Enqueue(0, "Third");

            Assert.AreEqual("First", queue.Dequeue());
            Assert.AreEqual("Second", queue.Dequeue());
            Assert.AreEqual("Third", queue.Dequeue());
        }
    }
}
