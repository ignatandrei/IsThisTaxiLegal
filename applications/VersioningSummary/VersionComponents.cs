using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VersioningSummary
{
    public class VersionComponents
    {
        static Assembly[] allAssemblies;
        private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
        {
            return Directory.GetCurrentDirectory() == compilationLibrary.Path;
        }
        /// <summary>
        /// http://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/
        /// </summary>
        /// <returns></returns>
        static Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                //if (IsCandidateCompilationLibrary(library))
                {
                    try
                    {
                        var an = new AssemblyName(library.Name);
                        var assembly = Assembly.Load(library.Name);
                        assemblies.Add(assembly);
                    }
                    catch (System.Exception)
                    {

                        //no exception if not load
                    }
                    
                }
            }
            return assemblies.ToArray();
        }
        static VersionComponents()
        {
            allAssemblies = GetAssemblies();
            //assemblies = new List<AssemblyName>();
            //var first = Assembly.GetExecutingAssembly();
            //assemblies.Add(first.GetName());
            //foreach(var item in first.GetReferencedAssemblies())
            //{
            //    assemblies.Add(item);

            //}


        }
        private Assembly[] LoadAssFromCurrentDir()
        {
            var dlls = new DirectoryInfo(Directory.GetCurrentDirectory())
                .GetFiles("*.dll",SearchOption.AllDirectories)
                .Select(it=>it.FullName.Replace(@"\","/"))
                ;
            
            return allAssemblies.
                Where(it => 
                    dlls.FirstOrDefault(dll=>it.CodeBase.Contains(dll))
                    !=null)
                .ToArray();

            
        }
        public VersionDll[] LoadCurrentDir()
        {
            List<VersionDll> ret = new List<VersionDll>();
            var ass = LoadAssFromCurrentDir();
            var curDir = Directory.GetCurrentDirectory();
            var di = new DirectoryInfo(curDir);
            foreach (var item in ass)
            {
                var name = item.GetName().Name;
                var nameFile = $"version{name}.json";
                var file = di.GetFiles(nameFile,SearchOption.AllDirectories).FirstOrDefault();
                if (file != null)
                {
                    var vc = LoadFromFile(file.FullName);
                    if(vc?.Length>0)
                        ret.AddRange(vc);

                }
                
                
                var v = new VersionDll();
                v.Name = name;
                var version= item.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
                if (version != null)
                {
                    v.Version = new Version(version);
                    if (ret.Any(it => v.Version.ToString() == it.Version.ToString() && v.Name == it.Name))
                        continue;//exists already
                }
                
                v.DateRelease = DateTime.Now;
                
                if (File.Exists(item.Location))
                {
                    v.DateRelease = new FileInfo(item.Location).CreationTimeUtc;
                }
                ret.Add(v);
                
            }
            return ret.ToArray();
        }

        public VersionDll[] LoadFromFile(string fileName)
        {
            string val = File.ReadAllText(fileName);
            var data = JsonConvert.DeserializeObject<VersionDll[]>(val);
            return data;
        }
    }
}
