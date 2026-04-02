import http from "node:http";
import fs from "node:fs";
import path from "node:path";

const resolveRoot = () => {
  const buildRootArg = process.argv[2] || "Demo/Build/WebGLParity";
  return path.resolve(process.cwd(), buildRootArg);
};

const resolvePort = () => {
  const rawPort = process.argv[3] || process.env.PORT || "8123";
  const parsed = Number.parseInt(rawPort, 10);
  return Number.isFinite(parsed) ? parsed : 8123;
};

const buildRoot = resolveRoot();
const port = resolvePort();

const mimeType = (filePath) => {
  if (filePath.endsWith(".html")) return "text/html; charset=UTF-8";
  if (filePath.endsWith(".css")) return "text/css; charset=UTF-8";
  if (filePath.endsWith(".js")) return "application/javascript; charset=UTF-8";
  if (filePath.endsWith(".wasm")) return "application/wasm";
  if (filePath.endsWith(".json")) return "application/json; charset=UTF-8";
  if (filePath.endsWith(".png")) return "image/png";
  if (filePath.endsWith(".jpg") || filePath.endsWith(".jpeg")) return "image/jpeg";
  if (filePath.endsWith(".svg")) return "image/svg+xml";
  if (filePath.endsWith(".ico")) return "image/x-icon";
  return "application/octet-stream";
};

const sendNotFound = (response) => {
  response.writeHead(404, { "Content-Type": "text/plain; charset=UTF-8" });
  response.end("Not found");
};

const sendForbidden = (response) => {
  response.writeHead(403, { "Content-Type": "text/plain; charset=UTF-8" });
  response.end("Forbidden");
};

const buildHeaders = (filePath) => {
  let contentPath = filePath;
  const headers = {
    "Cache-Control": "no-cache, no-store, must-revalidate"
  };

  if (filePath.endsWith(".br")) {
    headers["Content-Encoding"] = "br";
    contentPath = filePath.slice(0, -3);
  }

  headers["Content-Type"] = mimeType(contentPath);
  return headers;
};

const resolveRequestPath = (requestUrl) => {
  const pathname = decodeURIComponent(new URL(requestUrl, "http://localhost").pathname);
  const normalized = pathname === "/" ? "/index.html" : pathname;
  const absolutePath = path.resolve(buildRoot, `.${normalized}`);

  if (!absolutePath.startsWith(buildRoot)) {
    return null;
  }

  return absolutePath;
};

if (!fs.existsSync(buildRoot)) {
  console.error(`Build directory does not exist: ${buildRoot}`);
  process.exit(1);
}

if (!fs.existsSync(path.join(buildRoot, "index.html"))) {
  console.error(`index.html was not found in build directory: ${buildRoot}`);
  console.error("Run the Unity WebGL parity build first.");
  process.exit(1);
}

const server = http.createServer((request, response) => {
  const filePath = resolveRequestPath(request.url || "/");

  if (!filePath) {
    sendForbidden(response);
    return;
  }

  fs.stat(filePath, (error, stats) => {
    if (error || !stats.isFile()) {
      sendNotFound(response);
      return;
    }

    response.writeHead(200, buildHeaders(filePath));
    fs.createReadStream(filePath).pipe(response);
  });
});

server.listen(port, () => {
  console.log(`Serving Unity WebGL build`);
  console.log(`Root: ${buildRoot}`);
  console.log(`URL:  http://localhost:${port}/`);
});
