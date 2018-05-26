import http.server
import socketserver


handle = http.server.SimpleHTTPRequestHandler

with socketserver.TCPServer(("", 8000), handle) as httpd:
	print("Listening")
	httpd.serve_forever()