import os
from dotenv import load_dotenv

load_dotenv()

GROQ_API_KEY = os.getenv("GROQ_API_KEY", "")
GROQ_MODEL = "qwen/qwen3-32b"

ROUTING_SERVER_URL = "http://localhost:8090/MyService"
PROXY_SERVER_REST_URL = "http://localhost:9000/ProxyServiceRest"

CHROMA_PERSIST_DIR = os.path.join(os.path.dirname(__file__), "chroma_db")

LLM_SERVICE_HOST = "0.0.0.0"
LLM_SERVICE_PORT = 8000
