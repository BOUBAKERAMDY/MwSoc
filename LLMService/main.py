import uvicorn
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from config import LLM_SERVICE_HOST, LLM_SERVICE_PORT
from routers import health, chat, ws
from services.rag_service import initialize_rag

app = FastAPI(title="LetsGoBiking LLM Assistant")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(health.router)
app.include_router(chat.router)
app.include_router(ws.router)


@app.on_event("startup")
async def startup():
    initialize_rag()
    print("LLM Assistant service started")


if __name__ == "__main__":
    uvicorn.run(app, host=LLM_SERVICE_HOST, port=LLM_SERVICE_PORT)
