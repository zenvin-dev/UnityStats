# UnityStats
A stats system for Unity.

---

### WARNING
This was built on a whim and merely to test an idea. \
Especially the editor may still have some issues. But the system itself should be fairly robust.

---

### Intended Usage

#### Step 1: The `Stat`
`Stat`s are `ScriptableObject`s that identify a type of stat and tell it, what type it is. \
They can also contain custom logic to "globally" handle how changing their stat's value works.

The most simple implementation looks like this:
```csharp
using UnityEngine;
using Zenvin.Stats;

[CreateAssetMenu]
public class IntStat : Stat<int> { }
```
This is only needed because Unity cannot create or serialize instances of (constructed) generic types directly. \
So `IntStat` is required as a proxy, of sorts.

#### Step 2: The `StatInstance`
`StatInstance`s represent - as the name suggests - instances of `Stat`s. \
Their job is to hold the value of a given `Stat` for the object that they are placed on.
A `StatInstance` implementation for the `Stat` declared above, could look something like this: 
```csharp
using Zenvin.Stats;

public class InstStatInstance : StatInstance<int> { }
```

As before, the class essentially is a proxy for Unity's serialization.
Note that `StatInstance`s should be managed by a `StateContainer` component and **not** attached manually for the system to work properly.

#### Step 3: The `StatContainer`
`StatContainer`s are components that manage `StatInstance`s for the GameObject they are attached to. \
To use the system, simply attach a `StatContainer` to an object, drag a `Stat` SO onto it, and you are good to go. \
Note that if there are multiple `StatInstance` implementations matching the generic type of the given `Stat`, you will be prompted with a list of possible `StatInstance` types.

### Further notes
- As mentioned above, the editor was not optimized fully. Always make sure to remove all instances of a `StatInstance` implementation, before deleting the class from your project.
- For more advanced implementations, see the [built-in ones here](https://github.com/zenvin-dev/UnityStats/tree/main/Runtime/Implementations).