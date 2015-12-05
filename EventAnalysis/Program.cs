using Microsoft.Diagnostics.Symbols;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceEventSamples;

namespace EventAnalysis
{
    class Program
    {
        static void ProcessData(string dataFileName, string processName)
        {
            Out.WriteLine("**************  Creating a ETLX file for {0}", dataFileName);
            var traceLog = TraceLog.OpenOrConvert(dataFileName, new TraceLogOptions() { ConversionLog = Out });
            Out.WriteLine("**************  Done converting", Path.GetFileName(traceLog.FilePath));

            var simpleTraceLogProcess = traceLog.Processes.LastProcessWithName(processName);
            Debug.Assert(simpleTraceLogProcess != null);

            // Resolve symbols for clr and ntdll using the standard Microsoft symbol server path.  
            var symbolReader = new SymbolReader(Out, SymbolPath.MicrosoftSymbolServerPath);
            foreach (var module in simpleTraceLogProcess.LoadedModules)
            {
                if (module.Name == "clr" || module.Name == "ntdll" || module.Name == "mscorlib.ni")
                    traceLog.CodeAddresses.LookupSymbolsForModule(symbolReader, module.ModuleFile);
            }

            // Source line lookup is verbose, so we don't send it to the console but to srcLookupLog (which we currently ignore)
            var srcLookupLog = new StringWriter();
            var silentSymbolReader = new SymbolReader(srcLookupLog, SymbolPath.MicrosoftSymbolServerPath);
            silentSymbolReader.Options = SymbolReaderOptions.CacheOnly;     // don't try to look things up on the network for source 
            silentSymbolReader.SecurityCheck = (pdbPath) => true;           // for this demo we trust any pdb location.   This lets us find the PDB of the demo itself

            //Out.WriteLine("******Looking for SAMPLE PROFILE EVENTS");
            //int count = 0;
            //foreach (var sample in simpleTraceLogProcess.EventsInProcess.ByEventType<SampledProfileTraceData>())
            //{
            //    ++count;
            //    PrintStack(sample.CallStack(), silentSymbolReader);
            //}
            //Out.WriteLine("******TOTAL {0} SAMPLE PROFILE EVENTS", count);
            //Out.WriteLine();

            Out.WriteLine("******Looking for EXCEPTION EVENTS");
            foreach (var exceptionData in (simpleTraceLogProcess.EventsInProcess.ByEventType<ExceptionTraceData>()))
            {
                Out.WriteLine("Found an EXCEPTION event in SimpleTraceLog: Type: {0} Message: {1}", exceptionData.ExceptionType, exceptionData.ExceptionMessage);
                PrintStack(exceptionData.CallStack(), silentSymbolReader);
            }
            Out.WriteLine();
        }

        private static void PrintStack(TraceCallStack callStack, SymbolReader symbolReader)
        {
            Out.WriteLine("STACKTRACE:");
            while (callStack != null)
            {
                var method = callStack.CodeAddress.Method;
                var module = callStack.CodeAddress.ModuleFile;
                if (method != null)
                {
                    // see if we can get line number information
                    var lineInfo = "";
                    var sourceLocation = callStack.CodeAddress.GetSourceLine(symbolReader);
                    if (sourceLocation != null)
                        lineInfo = string.Format("  AT: {0}({1})", Path.GetFileName(sourceLocation.SourceFile.BuildTimeFilePath), sourceLocation.LineNumber);

                    Out.WriteLine("    Method: {0}!{1}{2}", module.Name, method.FullMethodName, lineInfo);
                }
                else if (module != null)
                    Out.WriteLine("    Module: {0}!0x{1:x}", module.Name, callStack.CodeAddress.Address);
                else
                    Out.WriteLine("    ?!0x{0:x}", callStack.CodeAddress.Address);

                callStack = callStack.Caller;
            }
        }

        private static TextWriter Out = Console.Out;

        static void Main(string[] args)
        {
            ProcessData(@"C:\temp\exceptions.etl", "FileExplorer");
            //SimpleTraceLog.Run();
            //ModuleLoadMonitor.Run();
            //ObserveGCEvents.Run();

            //ConcurrentDictionary<string, int> count = new ConcurrentDictionary<string, int>();
            //using (var source = new ETWTraceEventSource(@"C:\Users\Sasha\Documents\WPR Files\Trace.08-20-2014.15-07-18.etl"))
            //{
            //    source.Kernel.All += evt =>
            //    {
            //        count.AddOrUpdate(evt.EventName, 1, (k, v) => v + 1);
            //    };
            //    source.Kernel.PerfInfoSample += Kernel_PerfInfoSample;
            //    //source.UnhandledEvents += source_UnhandledEvents;
            //    source.Process();
            //}
            //foreach (var kvp in count)
            //{
            //    Console.WriteLine(kvp.Key + " " + kvp.Value);
            //}
        }

        static void Kernel_PerfInfoSample(Microsoft.Diagnostics.Tracing.Parsers.Kernel.SampledProfileTraceData obj)
        {
            
        }

        static void Kernel_All(TraceEvent obj)
        {
            Console.WriteLine(obj.EventName);
        }

        static void source_UnhandledEvents(TraceEvent obj)
        {
            Console.WriteLine(obj.EventName);
        }

        static void Kernel_FileIORead(Microsoft.Diagnostics.Tracing.Parsers.Kernel.FileIOReadWriteTraceData obj)
        {
            Console.WriteLine(obj.FileName);
        }
    }
}
