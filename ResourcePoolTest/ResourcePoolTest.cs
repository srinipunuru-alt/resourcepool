using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearnBig;

namespace LearnBig
{
    [TestClass]
    public class ResourcePoolTest
    {
        private int[] resourcesSample;

        public ResourcePoolTest()
        {
            this.resourcesSample = this.GetResources(10000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ShouldThrow_WhenArgumentIsNull()
        {            
            // Act
            var resourcePool = new ResourcePool<int>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ShouldThrow_WhenResourcesArgumentContainsZeroElements()
        {
            // Act
            var resourcePool = new ResourcePool<int>(new int[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ShouldThrow_WhenResourcesArgumentContainsMoreThanMaximumNumberOfResourcePoolElements()
        {
            // Act
            var resourcePool = new ResourcePool<int>(this.GetResources(10001));
        }

        [TestMethod]
        public void GetResource_ShouldReturnResourceFromThePool()
        {
            // Arrange
            int[] resources = this.resourcesSample;
            var resourcePool = new ResourcePool<int>(resources);

            // Act
            int resource = resourcePool.GetResource();

            // Assert
            Assert.IsTrue(resources.Any(r => r == resource));
        }

        [TestMethod]
        public void GetResource_ShouldNotReturnElementWhichIsAlreadyReserved_WhenCalledMultipleTimes()
        {
            // Arrange
            int[] resources = this.resourcesSample;            
            var resourcePool = new ResourcePool<int>(resources);

            HashSet<int> resourcesReturnedAlready = new HashSet<int>();
            for(int index = 0; index < resources.Length; index++)
            {
                // Act            
                int resource = resourcePool.GetResource();
                
                // Assert
                Assert.IsFalse(resourcesReturnedAlready.Contains(resource));
            }         
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReleaseResource_ShouldThrow_WhenResourceIsNull()
        {
            // Arrange
            var resourcePool = new ResourcePool<object>(new object[4]);
            // Act
            resourcePool.ReleaseResource(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReleaseResource_ShouldThrow_WhenReleasingResourceThatIsNotReserved()
        {
            // Arrange
            int[] resources = this.resourcesSample;
            var resourcePool = new ResourcePool<int>(resources);
            
            // Act
            resourcePool.ReleaseResource(resources[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetResource_ShouldThrow_WhenAllResourcesAreReleased()
        {
            // Arrange
            int[] resources = this.resourcesSample;
            var resourcePool = new ResourcePool<int>(resources);
            for(int index = 0; index < resources.Length; index++)
            {
                try
                {
                    resourcePool.GetResource();
                }
                catch(InvalidOperationException)
                {
                    // The above GetResource method shouldn't throw InvalidOperationException
                    // If it throws then the test should fail.
                    Assert.Fail("GetResource threw InvalidOperationException at an unexpected place.");
                }
            }

            // Act
            resourcePool.GetResource();
        }

        [TestMethod]
        public void ReleaseResource_ShouldMakeResourceAvailableForOthers()
        {
            // Arrange
            int[] resources = this.resourcesSample;
            var resourcePool = new ResourcePool<int>(resources);
            int resource = resourcePool.GetResource();
            resourcePool.ReleaseResource(resource);
            bool isReleasedResourceInPool = false;
            
            // Act
            for(int index = 0; index < resources.Length; index++)
            {
                int resource2 = resourcePool.GetResource();
                if(resource == resource2)
                {
                    isReleasedResourceInPool = true;
                }
            }

            Assert.IsTrue(isReleasedResourceInPool);
        }

        private int[] GetResources(int resourceCount)
        {
            Random rnd = new Random();
            int[] retVal = new int[resourceCount];
            for(int index = 0; index < resourceCount; index ++)
            {
                retVal[index] = rnd.Next(1, resourceCount);
            }

            return retVal;
        }
    }
}
