using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnBig
{
    /// <summary>ResourcePool class</summary>
    /// <typeparam name="T"></typeparam>
    public class ResourcePool<T>
    {
        /// <summary>MaxResourcePoolLength data member</summary>
        private const int MaxResourcePoolLength = 10000;

        /// <summary>availableResources data member</summary>
        private Queue<T> availableResources;

        /// <summary>reservedResources data member</summary>
        private HashSet<T> reservedResources = new HashSet<T>();

        /// <summary>Dummy object that is used to ensure 
        /// releaseResourceLock executes exclusively.</summary>
        private object releaseResourceLock = new object();

        /// <summary>Initializes a new instance of the ResourcePool{T} class</summary>
        /// <param name="resources">resources to use</param>
        public ResourcePool(T[] resources)
        {
            if(resources == null)
            {
                throw new ArgumentNullException("resources");
            }
            else if(resources.Length == 0)
            {
                throw new ArgumentException("There are no entries in the resources", "resources");
            }
            else if(resources.Length > MaxResourcePoolLength)
            {
                throw new ArgumentException(
                    "The resources count is more than the maximum resources that can be added to the resource pool",
                    "resources");
            }
            
            this.availableResources = new Queue<T>(resources);            
        }

        /// <summary>Gets the resource</summary>
        /// <returns>resulting value</returns>
        public T GetResource()
        {          
            if(this.availableResources.Count == 0)
            {
                throw new InvalidOperationException("No resources are available in the pool.");
            }

            T dequeuedResource = this.availableResources.Dequeue();
            this.reservedResources.Add(dequeuedResource);
            return dequeuedResource;
        }

        /// <summary>Releases the resource</summary>
        /// <param name="resource">resource to use</param>
        public void ReleaseResource(T resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException("resource to be released cannot be null", "resource");
            }

            // This lock is to prevent the race conditions when the two threads try to release the same resource
            // at the same time, a rare but possible scenario
            lock(releaseResourceLock)
            {
                if (this.reservedResources.Contains(resource))
                {
                    this.reservedResources.Remove(resource);
                    this.availableResources.Enqueue(resource);
                }
                else
                {                    
                    throw new InvalidOperationException("The resource cannot be released.");
                }
            }
        }
    }
}
