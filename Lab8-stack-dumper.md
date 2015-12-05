### Stack Dumper

In this lab, you will implement a simple utility that connects to a running process and prints out the stacks of all its threads. This can be very useful for production monitoring, checking on the state of a specific component, or even logging system activity over time.

#### Task 1

Open the **StackDumper\StackDumper.sln** solution in Visual Studio. It already has a reference to the ClrMD library, and some code to parse the first command-line argument and determine if it is a valid process id. All you need to do is implement the `DumpStacks` method, which should use the ClrMD API to attach to the process, retrieve all CLR threads, and print out their stacks.

The API you need is described in the slides. Some pointers:
* `DataTarget.AttachToProcess` -- note that you should use the overload that takes an `AttachFlag` value, and use `AttachFlag.NonInvasive` or `AttachFlag.Passive`
* `DataTarget.ClrVersions[0].CreateRuntime`
* `ClrRuntime.Threads`
* `ClrThread.StackTrace`

#### Task 2 (Optional)

Implement an extra feature: when passed a method name as the second command-line argument, StackDumper will print out only threads that have that method on their call stack. This makes it easy to identify waiting threads, WCF threads, ASP.NET threads, and so on.
