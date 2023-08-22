using HandlebarsDotNet;
using System.Reflection;
using System.Text;

namespace quickstart.handlebars_example;

internal class Templater
{
    private static Dictionary<string, string> AssetFilenames = new();
    private static Dictionary<string, string> RawTemplates = new();
    private static Dictionary<string, HandlebarsTemplate<object, object>> CompiledTemplates = new();

    public static string Render(string templateName, object data)
    {
        templateName = NormalizePath(templateName);
        if (CompiledTemplates.TryGetValue(templateName, out var compiledFunction))
        {
            return compiledFunction(data);
        }
        else if (RawTemplates.TryGetValue(templateName, out var rawTemplate))
        {
            var compiled = Handlebars.Compile(rawTemplate);
            CompiledTemplates.Add(templateName, compiled);
            return compiled(data);
        }
        else
        {
            throw new ArgumentException(nameof(templateName));
        }
    }

    public static string Render(string templateName)
    {
        templateName = NormalizePath(templateName);
        return Render(templateName, new { });
    }

    /*
     * gets an pointer to the resource stream from an localizable name
     */
    public static Stream? GetAssetStream(string filename)
    {
        filename = NormalizePath(filename);
        if (AssetFilenames.TryGetValue(filename, out var resourceName))
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
        else
        {
            return null;
        }
    }

    /*
     * here you can register the mime types for your application
     */
    public static string GetContentMimeType(string path)
    {
        return
            path.EndsWith(".css") ? "text/css; charset=utf-8" :
            path.EndsWith(".js") ? "application/javascript; charset=utf-8" :
            "application/octet-stream";
    }

    public static void InitializeTemplater()
    {
        string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        string? refPath = Assembly.GetExecutingAssembly().GetName().Name + ".";

        foreach (string resource in resources)
        {
            // normalize the file name removing the assembly name
            // part and making it lowercase
            string fileName = resource
                .Substring(refPath.Length, resource.Length - refPath.Length)
                .ToLower();

            if (fileName.EndsWith(".hbs"))
            {
                // on .hbs files we aren't going to include the extension
                // just view.<filename>
                fileName = fileName.Substring(0, fileName.Length - ".hbs".Length);

                string contents = ReadAssemblyFile(resource, Encoding.UTF8);
                Handlebars.RegisterTemplate(fileName, contents);
                RawTemplates.Add(fileName, contents);
            }
            else if (fileName.StartsWith("assets."))
            {
                // here we include an localizable file name which points to the resource
                // filename
                AssetFilenames.Add(fileName, resource);
            }
        }
    }

    private static string NormalizePath(string path)
        => path.Replace('/', '.').ToLower();

    private static string ReadAssemblyFile(string path, Encoding encoder)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        using (Stream? assemblyStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
        {
            if (assemblyStream == null) throw new FileNotFoundException(nameof(path));

            assemblyStream.CopyTo(memoryStream);
            byte[] contents = memoryStream.ToArray();
            return encoder.GetString(contents);
        }
    }
}
