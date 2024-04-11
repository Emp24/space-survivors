using UnityEngine;

public interface IObjectPool
{

    // Initializes the object pooling system by instantiating a specified number of GameObjects and adding them to a queue for future use.
    void InitializePool();

    // Retrieves an inactive GameObject from the object pool.
    //
    // Returns:
    //     The inactive GameObject retrieved from the object pool, or null if none are available.
    GameObject GetPooledObject();

    // Returns the specified GameObject into the pooledObjects queue.
    void ReturnToPool(GameObject gameObject);

}
