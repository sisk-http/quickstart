using Sisk.Core.Http;
using Sisk.Core.Routing;
using System.Text;

namespace quickstart.simple_file_server;

class Program
{
    public const int LISTENING_PORT = 5555;
    public const string DIRECTORY_ROOT = "C:\\wwwroot";
    public const string TRY_FILE = "index.html";
    public const bool DIRECTORY_LISTING = true;
    public const string DEFAULT_MIME_TYPE = "application/octet-stream";

    public static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".html", "text/html" },
        { ".js", "application/javascript" },
        { ".css", "text/css" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".webp", "image/webp" }
    };

    static void Main(string[] args)
    {
        Router router = new Router();
        Route route = new Route(RouteMethod.Any, ".*", Serve);
        route.UseRegex = true;
        route.UseCors = true;
        router.SetRoute(route);

        ListeningPort lp = new ListeningPort()
        {
            Hostname = "localhost",
            Secure = false,
            Port = LISTENING_PORT
        };

        ListeningHost host = new ListeningHost();
        host.Ports = new ListeningPort[] { lp };
        host.Router = router;
        host.CrossOriginResourceSharingPolicy = new()
        {
            AllowOrigin = "*",
            AllowHeaders = new[] { "*" },
            AllowMethods = new[] { "GET", "POST", "PUT", "PATCH" }
        };

        HttpServerConfiguration configuration = new HttpServerConfiguration();
        configuration.ListeningHosts.Add(host);

        HttpServer server = new HttpServer(configuration);
        server.Start();

        Console.WriteLine("Simple HTTP file server is listening at {0}", server.ListeningPrefixes[0]);

        Thread.Sleep(-1);
    }

    static HttpResponse Serve(HttpRequest request)
    {
        string path = request.Path;
        string realpath = NormalizedCombine(DIRECTORY_ROOT, path);
        string tryfilePath = NormalizedCombine(realpath, TRY_FILE);

        if (!realpath.StartsWith(DIRECTORY_ROOT))
        {
            // is trying to serve an protected content
            return new(403);
        }

        bool isFile = File.Exists(realpath);
        bool isDir = Directory.Exists(realpath);
        bool isTryfileMatched = File.Exists(tryfilePath);
        bool any = isFile || isDir || isTryfileMatched;

        if (isTryfileMatched)
            realpath = tryfilePath;

        if (any == false)
        {
            // ins't an directory or an file
            return new(404);
        }
        else if (isFile || isTryfileMatched)
        {
            var response = request.GetResponseStream();
            string extension = Path.GetExtension(realpath);
            string? mimeType;
            if (!MimeTypes.TryGetValue(extension, out mimeType))
            {
                mimeType = DEFAULT_MIME_TYPE;
            }

            response.SendChunked = true;
            response.SetStatus(System.Net.HttpStatusCode.OK);
            response.SetHeader("Content-Type", mimeType);

            using FileStream fs = File.OpenRead(realpath);
            fs.CopyTo(response.ResponseStream);

            return response.Close();
        }
        else //if (isDir)
        {
            if (!DIRECTORY_LISTING)
                return new(403);

            StringBuilder content = new StringBuilder();
            content.Append("<html><head><title>Directory listing</title></head><body>");
            string[] files = Directory.GetFiles(realpath);
            content.Append($"<h1>Directory listing of {Path.GetFileNameWithoutExtension(realpath)}</h1><ul>");

            content.Append($"""
                    <li>
                        <a href="../">
                            ..
                        </a>
                    </li>
                    """);

            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                content.Append($"""
                    <li>
                        <a href="./{filename}">
                            {filename}
                        </a>
                    </li>
                    """);
            }
            content.Append("</ul></body></html>");

            return new HttpResponse(200)
                .WithContent(new HtmlContent(content.ToString()))
                .WithStatus(200);
        }
    }

    // -> https://cy.proj.pw/#/blog-post?link=normalizando-path-combine.md
    public static string NormalizedCombine(params string[] paths)
    {
        if (paths.Length == 0) return "";

        bool startsWithSepChar = paths[0].StartsWith("/") || paths[0].StartsWith("\\");
        char environmentPathChar = Path.DirectorySeparatorChar;
        List<string> tokens = new List<string>();

        for (int ip = 0; ip < paths.Length; ip++)
        {
            string path = paths[ip]
                ?? throw new ArgumentNullException($"The path string at index {ip} is null.");

            string normalizedPath = path
                .Replace('/', environmentPathChar)
                .Replace('\\', environmentPathChar)
                .Trim(environmentPathChar);

            string[] pathIdentities = normalizedPath.Split(
                environmentPathChar,
                StringSplitOptions.RemoveEmptyEntries
            );

            tokens.AddRange(pathIdentities);
        }

        Stack<int> insertedIndexes = new Stack<int>();
        StringBuilder pathBuilder = new StringBuilder();
        foreach (string token in tokens)
        {
            if (token == ".")
            {
                continue;
            }
            else if (token == "..")
            {
                pathBuilder.Length = insertedIndexes.Pop();
            }
            else
            {
                insertedIndexes.Push(pathBuilder.Length);
                pathBuilder.Append(token);
                pathBuilder.Append(environmentPathChar);
            }
        }

        if (startsWithSepChar)
            pathBuilder.Insert(0, environmentPathChar);

        return pathBuilder.ToString().TrimEnd(environmentPathChar);
    }
}
