### Your First Structured Log

In this exercise, you will experiment with your first structured logging using the Microsoft **EventSource** library. Starting with .NET 4.5, this functionality is built into the .NET Framework; if you'd like to use it with older .NET versions, you can use the NuGet package with the same name.

#### Task 1

Create a new console application targeting .NET 4.5 or later.

Create a new class called `MyEventSource` that inherits from `System.Diagnostics.Tracing.EventSource`. Then, add a couple of log methods that take a variety of payloads to the `MyEventSource` class. If you're out of ideas, here's an example:

```
public void DownloadStarted(string url, int priority)
{
    WriteEvent(1, url, priority);
}

public void DownloadProgress(string url, int progressPercent)
{
    WriteEvent(2, url, progressPercent);
}

public void DownloadCompleted(string url, bool success, string localPath)
{
    WriteEvent(3, url, success, localPath);
}
```

Now, write some code in the `Main` method to create an instance of `MyEventSource` and call its tracing methods. Again, if you want to save time, go ahead and use the following example:

```
var source = new MyEventSource();
string url = "http://example.org/important.mp4";
source.DownloadStarted(url, 100);
for (int i = 0; i <= 100; i += 10)
{
    source.DownloadProgress(url, i);
}
source.DownloadCompleted(url, true, "C:\\temp\\important.mp4");
```

Run the application and make sure there are no exceptions. However, the log isn't recorded anywhere -- our ETW provider is not enabled, and therefore it is not logging events. We could use an ETW tool (such as PerfView) to capture events, but we won't just yet.

#### Task 2

Install the **EnterpriseLibrary.SemanticLogging.TextFile** NuGet package. It will also add the **EnterpriseLibrary.SemanticLogging** package to your project.

Configure the `ConsoleLog` and `FlatFileLog` listeners to listen to `MyEventSource` events. This is done by creating an instance of each listener, and then calling the `EnableEvents` method. For example:

```
var file = Microsoft.Practices.EnterpriseLibrary.SemanticLogging.FlatFileLog.CreateListener("my.log");
file.EnableEvents(source, EventLevel.LogAlways);
```

Run the application again and make sure events are logged to the console and to a flat text file. If you'd like, you can experiment with additional listeners.

> There are also third-party listeners available, e.g. a Splunk listener and an ElasticSearch listener, which you can find on NuGet as well.
