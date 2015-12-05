### Object Inspector

In this lab, you will implement a simple utility that looks for instances of a particular type in memory and then prints out the fields of these instances. This can be very useful for inspecting a production system without needing to recompile it or attach a debugger invasively. 

Create a new console application in Visual Studio 2015, and add a reference to the **Microsoft.Diagnostics.Runtime** (CLRMD) NuGet package. Note that it is still a prerelease package, so make sure to check the **Include prerelease** checkbox when searching for it.

Copy over the boilerplate code that attaches to a process (in `AttachFlag.Passive` mode) and creates a `ClrRuntime` object from the **StackDumper** project you just completed.

Add code to traverse all objects on the managed heap, and identify objects of a particular type. Specifically, `ClrRuntime.GetHeap()` will give you the managed heap object, `ClrHeap.EnumerateObjects()` will provide the addresses of all heap objects, and `ClrHeap.GetObjectType(objPtr)` will give you the type of each object.

Next, add code that prints the fields of each object you found. The `ClrType.Fields` collection has instances of `ClrField` that have a `GetValue` method that can return the field's value. Note that some fields are primitive and can be printed as is, and some fields are complex object references or structures and can't be printed directly. You can ignore complex fields for the sake of this exercise, and print only primitive and string fields. 
