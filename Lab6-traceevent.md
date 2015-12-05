### Live ETW Trace Analysis

In this lab, you will experiment with real-time ETW monitoring using the Microsoft **TraceEvent** library. It is free to use and available as a NuGet package.

#### Task 1

Open the **EventAnalysis\EventAnalysis.sln** solution. It was created by installing the **TraceEvent** and **TraceEvent Samples** NuGet packages. Under the **TraceEventSamples** solution folder, you can find a number of sample scenarios for doing ETW monitoring and analysis.

Add the following code to the `Main` method and run the application:

```
ObserveGCEvents.Run();
```

The app now monitors GC and allocation events across the entire system for 60 seconds before shutting down. When an event occurs, basic details are printed to the console. When you're done, explore the code in the **TraceEventSamples\20_ObserveGCEvents.cs** file to see how this was done.

#### Task 2

Replace the code in the `Main` method with the following:

```
KernelAndClrMonitorWin7.Run();
```

Run the application again and monitor the events that are printed to the console. This time, there should be printouts for all CLR events, and for basic kernel events such as process starts, DLL loads, etc. Again, explore the code in the **TraceEventSamples\33_KernelAndClrMonitorWin7.cs** file to see how this was done.
